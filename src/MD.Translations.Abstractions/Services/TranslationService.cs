using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MD.Translations
{
    public class TranslationService
    {
        private readonly ISupportedLangsService _SupportedLangsService;
        private readonly IEnumerable<ITranslationService> _Translators;

        public TranslationService(
            ISupportedLangsService supportedLangsService,
            IEnumerable<ITranslationService> translators)
        {
            _SupportedLangsService = supportedLangsService;
            _Translators = translators;
        }

        /// <summary>
        /// Translate the requested texts
        /// </summary>
        /// <returns>true if a the translation has been done or is not required</returns>
        public async Task<bool> TranslateAsync(Dictionary<string, string> texts, string lang, HttpRequest req, string context, string area)
        {
            // Check whether the lang is valid and supported
            //   if not, find the best alternative
            if (!_SupportedLangsService.IsSupported(lang))
                lang = _SupportedLangsService.GetValidLang(lang);

            // If the lang is supposed to be ignored, no need to translate.
            if (_SupportedLangsService.LangsToIgnore.Contains(lang))
                return true;

            // We suppose that there is only one translator intance by couple {lang, area}
            var translator = _Translators?.FirstOrDefault((t) => t.CanTranslate(lang, area));

            if (translator == null)
            {
                // If no translator found, let the default texts and return.
                // Should notify the admin about the issue.
                // TODO: build a missing-translation service that will populate when needed and alert the admin
                return false;
                // 
                throw new Exception($"Translator for {lang} {area} not found");
            }

            // translate our texts
            await translator.GetTranslationsAsync(texts, lang, area: area, context: context);
            return true;
        }
    }
}
