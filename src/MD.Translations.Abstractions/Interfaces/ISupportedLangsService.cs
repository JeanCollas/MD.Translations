using System.Collections.Generic;

namespace MD.Translations
{
    /// <summary>zz is used to display string codes</summary>
    public interface ISupportedLangsService
    {
        /// <summary>Default site language, use this one if no other suits</summary>
        string DefaultLang { get; }

        /// <summary>Hardcoded langs, that do not require dynamic translations</summary>
        string[] LangsToIgnore { get; }

        /// <summary>Check a lang value, to know if it is supported</summary>
        /// <param name="lang">2 char iso code or culture code</param>
        /// <param name="ensureValue">lang should not be null or empty</param>
        bool IsSupported(string lang, bool ensureValue = false);

        /// <summary>
        /// Get a list of all registered languages
        /// May be used to display all available languages on the website
        /// </summary>
        /// <param name="includeZZ">zz is the code used to display text keys instead of strings</param>
        List<string> GetSupportedLangs(bool includeZZ = false);

        /// <summary>
        /// May adapt the provided to the best match to the provided input 
        /// cleaned of any attack attempt
        /// (e.g. culture converted to language iso code, switch to a similar language)
        /// </summary>
        /// <param name="lang">lang input</param>
        /// <returns></returns>
        string GetValidLang(string lang);

        /// <summary>
        /// Allows to customize supported languages codes
        /// </summary>
        void SetSupportedLangs(IEnumerable<string> langs);
    }
}