using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace MD.Translations
{
    /// <summary>
    /// Root of an XML Translation file
    ///   Made of a list of Lang/Area translations
    /// </summary>
    [XmlType("Translations")]
    public class XmlTranslationRoot : List<XmlTranslationAreaLang>
    {
        static ConcurrentDictionary<string, string> _Locks = new ConcurrentDictionary<string, string>();

        // Keep the loaded file path
        [XmlIgnore]
        public string FilePath { get; set; }
        [XmlIgnore]
        public string FileName { get => Path.GetFileName(FilePath); }

        /// <summary>Load a complete folder of XML translations and potentially its subfolders</summary>
        public static List<XmlTranslationRoot> LoadAll(string folder, string pattern = "*.xml", bool includeSubdirectories = true)
        {
            var fileNames = Directory.GetFiles(folder, pattern, includeSubdirectories ? SearchOption.AllDirectories : SearchOption.TopDirectoryOnly);
            var l = new List<XmlTranslationRoot>();
            foreach (var f in fileNames)
                l.Add(Load(f));
            return l;
        }

        /// <summary>Load a single XML Translation file</summary>
        public static XmlTranslationRoot Load(string filePath)
        {
            XmlTranslationRoot xtr;
            using (var file = File.OpenText(filePath))
            {
                var ser = new XmlSerializer(typeof(XmlTranslationRoot));
                //var sb = new StringWriter();
                xtr = ser.Deserialize(file) as XmlTranslationRoot;
                xtr.FilePath = filePath;
            }
            return xtr;
        }

        /// <summary>
        /// Save an existing translation file
        /// </summary>
        public Task Save(string filePath = null)
        {
            filePath = filePath ?? FilePath;

            if (filePath == null) return Task.CompletedTask;
            if (!_Locks.ContainsKey(filePath))
                _Locks[filePath] = filePath;
            var lockObj = _Locks[filePath];
            if(!Monitor.TryEnter(lockObj))
            {
                if (!Monitor.Wait(lockObj, 5) || ! Monitor.IsEntered(lockObj))
                    return Task.CompletedTask;
            }
            try 
            {
                var ser = new XmlSerializer(typeof(XmlTranslationRoot));
                using (var sb = new StringWriter())
                {
                    ser.Serialize(sb, this);
                    var str = sb.ToString();

                    using (var file = File.OpenWrite(filePath))
                    {
                        var toWrite = str.ToByteArray();
                        file.SetLength(0);
                        file.Write(toWrite, 0, toWrite.Length);
                    }
                }
            }
            finally
            {
                Monitor.Exit(lockObj);
            }
            return Task.CompletedTask;
        }

        /// <summary>
        /// Save a new XML translation file, returns the full file path
        /// </summary>
        public async Task<string> SaveNew(string folder, string fileName = null)
        {
            if (fileName == null)
            {
                fileName = "";
                if (this.Count > 0)
                {
                    var areas = this.Select(a => a.Area?.ToUrlPrefix().Trim('-')).Distinct();
                    fileName = String.Join("_", areas);
                }
                if (fileName.Length == 0) fileName = "empty";
                var langs = this.Select(a => a.Lang).Distinct();
                if (langs.Count() == 1)
                    fileName += $".{langs.First()}";
                fileName += ".xml";
            }
            var filePath = Path.Combine(folder, fileName);
            FilePath = filePath;
            await Save(filePath);
            return FilePath;
        }

        // Not used
        public void AddMissingTranslation(string area, string lang, string context, string key, string defaultText = null)
        {
            var areaLang = this.FirstOrDefault(t => t.Area == area && t.Lang == lang);
            if (areaLang == null) { areaLang = new XmlTranslationAreaLang() { Area = area, Lang = lang }; }
            if (!areaLang.Contexts.TryGetValue(context, out var ctxt))
            {
                ctxt = new XmlTranslationAreaContext() { Name = context };
                ctxt.Missing.Add(new XmlTranslationKeyText() { Key = key, Text = defaultText ?? key });
            }
            else
            {
                var exists = ctxt.Entries.ContainsKey(key);
                if (exists) return;
                exists = ctxt.Missing.Any(kt => kt.Key == key);
                if (exists) return;
                ctxt.Missing.Add(new XmlTranslationKeyText() { Key = key, Text = defaultText ?? key });
            }
        }

        // Not used
        public bool TranslationExists(string area, string lang, string context, string key)
        {
            XmlTranslationAreaContext ctxt = null;
            var areaLangExists = this.FirstOrDefault(t => t.Area == area && t.Lang == lang)?.Contexts.TryGetValue(context, out ctxt) ?? false;
            if (!areaLangExists || ctxt == null) return false;
            return ctxt.Entries.ContainsKey(key);
        }
    }
}
