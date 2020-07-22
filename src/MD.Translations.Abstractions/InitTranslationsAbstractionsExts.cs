using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Text;

namespace MD.Translations.Abstractions
{
    public static class InitTranslationsAbstractionsExts
    {
        public static void InitTranslations(this IServiceCollection services)
        {
            services.AddScoped<ITranslationService, TranslationService>();
        }

    }
}
