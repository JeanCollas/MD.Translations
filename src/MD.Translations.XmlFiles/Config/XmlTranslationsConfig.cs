using System;
using System.Collections.Generic;
using System.IO;
using Microsoft.Extensions.Logging;

namespace MD.Translations
{
    public class XmlTranslationsConfig
    {
        #region Config object from JSON
        /// <summary>
        /// Folders containing translation files
        /// 
        /// Translation files should be named using the following pattern:
        ///   area-name.lang.xml
        /// </summary>
        public List<string> Folders { get; set; } = new List<string>();

        /// <summary>
        /// Additional translation files path
        /// 
        /// Translation files should be named using the following pattern:
        ///   area-name.lang.xml
        /// </summary>
        public List<string> Files { get; set; } = new List<string>();

        /// <summary>
        /// Translation langs
        /// If not empty, only specified langs will be loaded from the files/folders
        /// 
        /// Translation files should be named using the following pattern:
        ///   area-name.lang.xml
        /// </summary>
        public List<string> Langs { get; set; } = new List<string>();

        /// <summary>
        /// Folder used to save missing area translations
        /// 
        /// Missing translation files will be named using the following pattern:
        ///   {MissingFolder}/missing.{area-name}.{lang}.xml
        /// </summary>
        public string MissingFolder { get; set; }
        public bool? SplitMissingByArea { get; set; } = null;

        #endregion Config object from JSON

        /// <summary>
        /// Check that the config is correct
        /// Throw an exception if folders or files do not exist
        /// </summary>
        public void CheckConfig()
        {
            // Ensure that types are not null
            Folders = Folders ?? new List<string>();
            Files = Files ?? new List<string>();
            Langs = Langs ?? new List<string>();

            // Check that all specified folders exist
            foreach (var p in Folders)
            {
                if (!Directory.Exists(p))
                    throw new Exception($"Translations folder '{p}' does not exist. Current directory: '{Directory.GetCurrentDirectory()}'");
                //var files = Directory.EnumerateFileSystemEntries(p, "*.xml");
                //Files.AddRange(from f in files select (Path.GetFileNameWithoutExtension(f), f));
            }

            // Check that all specified files exist
            foreach (var f in Files)
            {
                if (!File.Exists(f))
                    throw new Exception($"Translations file '{f}' does not exist. Current directory: '{Directory.GetCurrentDirectory()}'");
            }

            // Check that the missing translations folder exists
            if (!String.IsNullOrEmpty(MissingFolder) && !Directory.Exists(MissingFolder))
            {
                try
                {
                    Directory.CreateDirectory(MissingFolder);
                }
                catch
                {
                    throw new Exception($"Folder for missing translations '{MissingFolder}' does not exist, cannot create it. Current directory: '{Directory.GetCurrentDirectory()}'");
                }
            }
        }

        const string LOG_PREFIX = "XmlTranslations: ";

        /// <summary>
        /// Simple log to understand what will be loaded
        /// </summary>
        /// <param name="logger"></param>
        public void LogConfig(ILogger logger)
        {
            if (logger == null) return;

            // Log langs found in config object
            logger.LogInformation($"{LOG_PREFIX}langs in the config object: {(Langs == null || Langs.Count == 0 ? "no lang specified (take all)" : $"'{String.Join("', '", Langs)}'")}");

            // Log all folders found in config object
            foreach (var p in Folders)
                logger.LogInformation($"{LOG_PREFIX}folder added '{p}'");

            // Log all files found in config object
            foreach (var f in Files)
                logger.LogInformation($"{LOG_PREFIX}file found '{f}'");

            // Warn if no folder or file found in config object
            if (Folders.Count == 0 && Files.Count == 0) logger.LogWarning($"{LOG_PREFIX}NO FILE OR FOLDER FOUND FROM CONFIG OBJECT");
        }
    }
}
