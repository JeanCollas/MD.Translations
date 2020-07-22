using System.Collections.Generic;
using System.Threading.Tasks;

namespace MD.Translations
{
    public interface ITranslationService
    {
        Task<bool> TranslateAsync(Dictionary<string, string> texts, string lang, string context, string area);
    }
}