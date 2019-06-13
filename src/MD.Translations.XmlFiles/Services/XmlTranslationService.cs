using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace MD.Translations
{
    public class XmlTranslationService : ITranslationService
    {
        //    //            if (context == null && req != null) context = $"[{req.Method}] {req.Path}";
        //    //            context = context ?? "none";

        //    //var requestDebugPath = GetRequestDebugPath(req);

        private readonly IEnumerable<XmlTranslationsConfig> _Configs;
        private readonly IMemoryCache _Cache;
        private readonly IActionContextAccessor _ActionContextAccessor;
        private readonly ILogger<XmlTranslationService> _Logger;

        public XmlTranslationService(
            IEnumerable<XmlTranslationsConfig> configs,
            IMemoryCache cache,
            IActionContextAccessor actionContextAccessor,
            ILogger<XmlTranslationService> logger
            )
        {
            _Configs = configs;
            _Cache = cache;
            _ActionContextAccessor = actionContextAccessor;
            _Logger = logger;
        }

        #region In-memory cache storage for translations

        /// <summary>Key used to store translations in memory cache</summary>
        static string GetMemoryKey(string lang) => $"XmlTranslations_{lang}";

        /// <summary>Get or load translations from memory cache for a specific lang</summary>
        List<XmlTranslationRoot> GetAllLangTranslations(string lang)
            => _Cache.GetOrCreate(GetMemoryKey(lang), (cacheEntry) => { cacheEntry.SlidingExpiration = TimeSpan.FromHours(1); return InitTranslations(lang); });

        #endregion In-memory cache storage for translations

        #region Loading files

        // Assumes that files are well named, and one language per file
        private List<XmlTranslationRoot> InitTranslations(string lang)
        {
            var xtrs = new List<XmlTranslationRoot>();
            foreach (var c in _Configs.Where(c => c.Langs == null || c.Langs.Count == 0 || c.Langs.Contains(lang)))
                foreach (var d in c.Folders)
                    xtrs.AddRange(XmlTranslationRoot.LoadAll(d, pattern: $"*.{lang}.xml", includeSubdirectories: true));
            return xtrs;
        }

        #endregion Loading files

        #region ITranslationService implementation

        //static string[] _HandledLangs = new[] { "es", "it", "de", "nl", "ru", "pl", "pt" };
        public bool CanTranslate(string lang, string area, string context = null) => _Configs.Any(l=>l.Langs.Contains(lang)) || lang == "zz"; //_HandledLangs.Contains(lang) 

        public async Task<string> GetTranslationAsync(string key, string lang,
                                                string area = null, string context = null,
                                                string defaultText = null)
        {
            // Let's be lazy...
            var texts = new Dictionary<string, string>();
            texts[key] = defaultText;
            await GetTranslationsAsync(texts, lang, area, context);
            return texts[key];
        }

        public async Task GetTranslationsAsync(Dictionary<string, string> texts, string lang, string area = null, string context = null)
        {
            if (lang == "zz")
            {
                await GetTranslationsZZAsync(texts, lang, area, context);
                return;
            }

            var translationsRoots = GetAllLangTranslations(lang);

            // ensure that the language is defined in files
            if (translationsRoots == null || translationsRoots.Count == 0)
                throw new Exception($"{lang} files not provided");

            // look for the relevant context
            var validContext = (from r in translationsRoots
                                from al in r
                                    // filter the area
                                where al.Lang == lang && al.Area == area
                                from c in al.Contexts
                                where c.Key == context
                                select new { Root = r, Context = c.Value }).FirstOrDefault();

            // If no relevant translation entry has been found
            //   save the missing keys for future translation
            //   and keep the existing default texts (default language)
            if (validContext == null)
            {
                await SaveTranslationContextMissing(texts, lang, area, context);
                return;
                // No need to break the user
                //    throw new Exception(
                //        $"Could not find translations for area '{area}' context '{context}'");
            }

            // Translations exist, however ensure that none of the keys are missing
            var (validKeys, missingKeys) = await CheckIfKeysAreMissing(texts, validContext.Context, validContext.Root, lang, area, context);

            // Translate all available texts
            foreach (var k in validKeys)
                texts[k] = validContext.Context.Entries[k];
        }

        #endregion ITranslationService implementation

        #region Missing translations handling

        /// <summary>
        /// Get a relevant folder to store missing translations
        /// </summary>
        string GetMissingFolder(string lang)
        {
            var missingFolder = _Configs.Where(c => c.Langs == null || c.Langs.Count() == 0 || c.Langs.Contains(lang))
                .OrderByDescending(c => c.Langs != null && c.Langs.Contains(lang) ? 1 : 0)
                .Select(c => c.MissingFolder)
                .Where(c => !c.IsNullOrEmpty())
                .FirstOrDefault();
            if (_Logger != null)
            {
                if (missingFolder.IsNullOrEmpty())
                    _Logger.LogWarning($"Could not find a folder to store missing translations for {lang}");
                else
                    _Logger.LogInformation($"Using {missingFolder} folder to store missing translations for {lang}");
            }
            return missingFolder;
        }

        /// <summary>
        /// Check if some translations are missing from a context
        /// </summary>
        private async Task<(IEnumerable<string> ValidKeys, IEnumerable<string> MissingKeys)> CheckIfKeysAreMissing(Dictionary<string, string> texts, XmlTranslationAreaContext validContext, XmlTranslationRoot root, string lang, string area, string context)
        {
            // Check if missing keys in translation context
            var missingKeys = texts.Keys.Except(validContext.Entries.Keys);
            if (missingKeys.Count() > 0)
            {
                var texts2 = new Dictionary<string, string>();
                foreach (var k in missingKeys)
                    texts2[k] = texts[k];
                await SaveTranslationContextMissing(texts2, lang, area, context);
                /*
                // Check if missing keys have not been saved already
                var missing = missingKeys.Except(context.Missing.Select(k => k.Key));
                // If any missing, then save the missing keys into the appropriate section of the translation file
                if (missing.Count() > 0)
                {
                    foreach (var m in missing)
                    {
                        // For each missing key, save the key and the default translation text
                        context.Missing.Add(new XmlTranslationKeyText() { Key = m, Text = texts[m] });
                    }
                    // Save the translation file as it has been modified
                    await root.Save();
                }*/
            }

            var validKeys = texts.Keys.Except(missingKeys).ToList();

            return (validKeys, missingKeys);
        }


        private (string missingFolder, string fileName) GetMissingFile(string lang, string area, string context)
        {
            // file named missing.area.lang.xml
            var fileName = "missing";
            if (!area.IsNullOrEmpty()) fileName += $".{area}";
            else if (_Configs.Any(c => c.SplitMissingByArea.HasValue && c.SplitMissingByArea.Value))
            {
                //// Provides a by-area split of translation files
                var paths = context.TrimStart('/').Split('/').Where(a => !a.IsNullOrEmpty()).ToList();
                if (paths.Count() > 0) paths.RemoveAt(paths.Count - 1);
                paths = paths.Select(p => p.ReplaceDiacritics().RemoveNonAlphaNumChar()).Where(a => !a.IsNullOrEmpty()).ToList();
                var area2 = String.Join("_", paths.Take(2));
                if(area2.Length>0)
                    fileName += $".{area2}";
            }
            fileName += $".{lang}.xml";

            var missingFolder = GetMissingFolder(lang);
            return (missingFolder, fileName);
        }

        public async Task<XmlTranslationRoot> GetMissingTranslationRootAsync(string lang, string area, string context)
        {
            var (missingFolder, fileName) = GetMissingFile(lang, area, context);
            var missingFile = Path.Combine(missingFolder, fileName);

            // Build a translation root object. 
            //   If the file exists, load it
            //   Otherwise create it
            XmlTranslationRoot root;
            if (File.Exists(missingFile))
                root = XmlTranslationRoot.Load(missingFile);
            else
            {
                root = new XmlTranslationRoot();
                root.FilePath = missingFile;
                await root.SaveNew(missingFolder, fileName);
            }
            return root;
        }

        /// <summary>
        /// Save translation context as missing in a new file in case it is missing
        /// </summary>
        private async Task SaveTranslationContextMissing(Dictionary<string, string> texts, string lang, string area, string context)
        {
            XmlTranslationRoot root = await GetMissingTranslationRootAsync(lang, area, context);


            // Check if the area already exists in the root object, otherwise create it
            var areaNode = root.FirstOrDefault(al => al.Lang == lang && al.Area == area);
            if (areaNode == null) { areaNode = new XmlTranslationAreaLang() { Area = area, Lang = lang }; root.Add(areaNode); }

            // Check if the context already exists in the area node, otherwise create it
            var contextNode = areaNode.Contexts.ContainsKey(context) ? areaNode.Contexts[context] : null;
            if (contextNode == null) { contextNode = new XmlTranslationAreaContext() { Name = context }; areaNode.Contexts.Add(context, contextNode); }

            // Update the context, if needed and save the modified file
            var alreadySet = contextNode.Missing.Select(k => k.Key);
            var alreadySet2 = contextNode.Entries.Select(k => k.Key);
            var keysToAdd = texts.Keys.Except(alreadySet).Except(alreadySet2).ToList();
            if (keysToAdd.Count() > 0)
            {
                foreach (var t in keysToAdd)
                {
                    contextNode.Missing.Add(new XmlTranslationKeyText() { Key = t, Text = texts[t] });
                }

                // if some changed happened, save the request origin
                var infos = GetRouteInfo();
                contextNode.MissingInfo.Add($"[{infos.Method}] {infos.RouteTemplate} {infos.RouteName} {infos.RequestUrl}");

                // Keep log size reasonable
                if (contextNode.MissingInfo.Count > 150)
                    contextNode.MissingInfo = contextNode.MissingInfo.Distinct().Take(100).ToList();

                await root.Save();
            }
        }

        /// <summary>
        /// Get route information from context
        /// </summary>
        (string Method, string RouteTemplate, string RouteName, string RequestUrl) GetRouteInfo()
        {
            var actionContext = _ActionContextAccessor.ActionContext;
            string method = null;
            string url = null;
            try
            {
                method = actionContext.HttpContext?.Request?.Method;
            }
            catch { }
            try
            {
                url = _ActionContextAccessor.ActionContext?.HttpContext?.Request?.GetAbsoluteUri().ToString();
            }
            catch { }
            try
            {
                var routeTemplate = _ActionContextAccessor.ActionContext?.ActionDescriptor?.AttributeRouteInfo?.Template;
                var routeName = _ActionContextAccessor.ActionContext?.ActionDescriptor?.AttributeRouteInfo?.Name;
                return (method, routeTemplate, routeName, url);
            }
            catch
            {
                try
                {
                    var path = actionContext.HttpContext?.Request?.Path;
                    return (method, path, path, url);
                }
                catch
                {
                    // Should never happen
                    return (null, "failed to retreive route data", null, url);
                }
            }
        }

        #endregion Missing translations handling

        /// <summary>
        /// zz lang will always generate a translation missing file
        ///   to ensure that it is 
        /// </summary>
        private async Task GetTranslationsZZAsync(Dictionary<string, string> texts, string lang, string area, string context)
        {
            try
            {
                var folder = GetMissingFolder(lang);

                if (folder.IsNullOrEmpty())
                    throw new Exception($"{lang} not configured");

                // generate the generic keys file with default texts
                //   if already existing, it will just be updated with new keys if needed
                await SaveTranslationContextMissing(texts, lang, area, context);
            }
            finally
            {
                // replace each text by its key
                foreach (var t in texts.Keys.ToList()) texts[t] = t;
            }
        }

        //string GetRequestDebugPath(HttpRequest req) => req == null ? null : "[" + req.Method + "] " + req.Path + req.QueryString;

        //public string AreaLangToString(XmlTranslationAreaLang areaLang)
        //{
        //    var ser = new XmlSerializer(typeof(XmlTranslationAreaLang));
        //    var sb = new StringWriter();
        //    ser.Serialize(sb, areaLang);
        //    var str = sb.ToString();
        //    return str;
        //}
    }
}
