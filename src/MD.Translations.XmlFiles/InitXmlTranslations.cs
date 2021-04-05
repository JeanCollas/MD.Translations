using MD.Translations.Abstractions;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System.Collections.Generic;

namespace MD.Translations
{
    public static class InitTranslationsExts
    {
        /// <summary>
        /// Maps a ConfigSection to a XmlTranslationsConfig object
        /// </summary>
        public static XmlTranslationsConfig GetXmlTranslationsConfig(this IConfigurationSection config)
        {
            var conf = new XmlTranslationsConfig();
            config.Bind(conf);
            return conf;
        }

        /// <summary>
        /// Maps a ConfigSection to a List&lt;XmlTranslationsConfig&gt; object
        /// </summary>
        public static List<XmlTranslationsConfig> GetXmlTranslationsConfigs(this IConfigurationSection config)
        {
            var conf = new List<XmlTranslationsConfig>();
            config.Bind(conf);
            return conf;
        }

        public static void InitXmlTranslations(this IServiceCollection services, IConfigurationSection config, ILogger logger = null)
            => InitXmlTranslations(services, config.GetXmlTranslationsConfig(), logger);

        /// <summary>
        /// Initialize Xml Translations
        ///
        /// Using this service requires to register
        ///   IActionContextAccessor service
        ///   ISupportedLangsService service
        ///   IMemoryCache service
        /// </summary>
        public static void InitXmlTranslations(this IServiceCollection services, XmlTranslationsConfig config, ILogger logger = null)
            => services.InitXmlTranslations(new[] { config }, logger);

        /// <summary>
        /// Initialize Xml Translations
        ///
        /// Using this service requires to register
        ///   ISupportedLangsService service
        /// </summary>
        public static void InitXmlTranslations(this IServiceCollection services, IEnumerable<XmlTranslationsConfig> configs, ILogger logger = null)
        {
            services.InitTranslations();

            foreach (var conf in configs)
            {
                // Ensure that files/folders exist
                conf.CheckConfig();

                // Log what config is loaded
                if (logger != null)
                    conf.LogConfig(logger);

                // Register for the service config
                services.AddSingleton(conf);
            }

            //services.AddActionContextAccessor();
            services.AddSingleton<IActionContextAccessor, ActionContextAccessor>();
            services.AddMemoryCache();
            services.AddSingleton<ILangTranslationService, XmlTranslationService>();
            //services.AddSingleton<XmlTranslationIOService, XmlTranslationIOService>();
        }
    }
}
