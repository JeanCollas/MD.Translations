using System.Collections.Generic;
using System.Threading.Tasks;

namespace MD.Translations
{
    public interface ILangTranslationService
    {
        /// <summary>
        /// Check whether the implemented service can translate 
        /// for the following constraints
        /// <param name="lang">the targetted language code</param>
        /// <param name="area">the area constraint</param>
        /// <param name="context">the context constraint</param>
        /// </summary>
        bool CanTranslate(string lang, string area, string context = null);

        /// <summary>
        /// Get the translation for a single key
        /// <param name="key">the text key</param>
        /// <param name="lang">the targetted language code</param>
        /// <param name="area">the area constraint</param>
        /// <param name="context">the context constraint</param>
        /// <param name="defaultText">the default text if no translation is found</param>
        /// </summary>
        Task<string> GetTranslationAsync(string key, string lang, string area = null, string context = null, string defaultText = null);

        /// <summary>
        /// Translates a dictionary of (key/value)
        /// <param name="key">the text key</param>
        /// <param name="lang">the targetted language code</param>
        /// <param name="area">the area constraint</param>
        /// <param name="context">the context constraint</param>
        /// </summary>
        Task GetTranslationsAsync(Dictionary<string, string> texts, string lang, string area = null, string context = null);
    }
}
