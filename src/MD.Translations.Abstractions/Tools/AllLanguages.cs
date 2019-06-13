using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;

namespace MD.Translations
{
    /// <summary>
    /// Tool class to manage the app languages
    /// </summary>
    public class AllLanguages
    {
        static AllLanguages _I;
        public static AllLanguages I => _I ?? (_I = new AllLanguages());

        /// <summary>
        /// Returns the language name in native language of a code (2-iso, culture, ...), 
        ///   based on the framework knowledge
        /// </summary>
        public string NativeLanguageName(string code)
        {
            try
            {
                var culture = new System.Globalization.CultureInfo(code);
                return culture.NativeName.IfNullOrEmpty(culture.EnglishName).IfNullOrEmpty(code);
            }
            catch
            {
                return code;
            }
        }

        /// <summary>
        /// Try to get the language name in native language
        /// </summary>
        /// <returns>language name, or iso code if not found</returns>
        public static string TryGetNativeLanguage(string code, string defaultValue = null) => NativeLanguages.ContainsKey(code) ? NativeLanguages[code] : defaultValue ?? code;
        /// <summary>
        /// Try to get the language name in english
        /// </summary>
        /// <returns>language name, or iso code if not found</returns>
        public static string TryGetEnglishLanguage(string code) => EnglishLanguages.ContainsKey(code) ? EnglishLanguages[code] : code;

        // List ISO639-1
        // https://en.wikipedia.org/wiki/List_of_ISO_639-1_codes
        // another list with translated names
        // https://www.loc.gov/standards/iso639-2/php/code_list.php
        /// <summary>List of all languages in english, indexed by their 2-char iso code</summary>
        public static Dictionary<string, string> EnglishLanguages = new Dictionary<string, string>()
        {
            ["ab"] = "Abkhazian",
            ["aa"] = "Afar",
            ["af"] = "Afrikaans",
            ["ak"] = "Akan",
            ["sq"] = "Albanian",
            ["am"] = "Amharic",
            ["ar"] = "Arabic",
            ["an"] = "Aragonese",
            ["hy"] = "Armenian",
            ["as"] = "Assamese",
            ["av"] = "Avaric",
            ["ae"] = "Avestan",
            ["ay"] = "Aymara",
            ["az"] = "Azerbaijani",
            ["bm"] = "Bambara",
            ["ba"] = "Bashkir",
            ["eu"] = "Basque",
            ["be"] = "Belarusian",
            ["bn"] = "Bengali",
            ["bh"] = "Bihari languages",
            ["bi"] = "Bislama",
            ["bs"] = "Bosnian",
            ["br"] = "Breton",
            ["bg"] = "Bulgarian",
            ["my"] = "Burmese",
            ["ca"] = "Catalan, Valencian",
            ["ch"] = "Chamorro",
            ["ce"] = "Chechen",
            ["ny"] = "Chichewa, Chewa, Nyanja",
            ["zh"] = "Chinese",
            ["cv"] = "Chuvash",
            ["kw"] = "Cornish",
            ["co"] = "Corsican",
            ["cr"] = "Cree",
            ["hr"] = "Croatian",
            ["cs"] = "Czech",
            ["da"] = "Danish",
            ["dv"] = "Divehi, Dhivehi, Maldivian",
            ["nl"] = "Dutch, Flemish",
            ["dz"] = "Dzongkha",
            ["en"] = "English",
            ["eo"] = "Esperanto",
            ["et"] = "Estonian",
            ["ee"] = "Ewe",
            ["fo"] = "Faroese",
            ["fj"] = "Fijian",
            ["fi"] = "Finnish",
            ["fr"] = "French",
            ["ff"] = "Fulah",
            ["gl"] = "Galician",
            ["ka"] = "Georgian",
            ["de"] = "German",
            ["el"] = "Greek, Modern (1453-)",
            ["gn"] = "Guarani",
            ["gu"] = "Gujarati",
            ["ht"] = "Haitian, Haitian Creole",
            ["ha"] = "Hausa",
            ["he"] = "Hebrew",
            ["hz"] = "Herero",
            ["hi"] = "Hindi",
            ["ho"] = "Hiri Motu",
            ["hu"] = "Hungarian",
            ["ia"] = "Interlingua(International Auxiliary Language Association)",
            ["id"] = "Indonesian",
            ["ie"] = "Interlingue, Occidental",
            ["ga"] = "Irish",
            ["ig"] = "Igbo",
            ["ik"] = "Inupiaq",
            ["io"] = "Ido",
            ["is"] = "Icelandic",
            ["it"] = "Italian",
            ["iu"] = "Inuktitut",
            ["ja"] = "Japanese",
            ["jv"] = "Javanese",
            ["kl"] = "Kalaallisut, Greenlandic",
            ["kn"] = "Kannada",
            ["kr"] = "Kanuri",
            ["ks"] = "Kashmiri",
            ["kk"] = "Kazakh",
            ["km"] = "Central Khmer",
            ["ki"] = "Kikuyu, Gikuyu",
            ["rw"] = "Kinyarwanda",
            ["ky"] = "Kirghiz, Kyrgyz",
            ["kv"] = "Komi",
            ["kg"] = "Kongo",
            ["ko"] = "Korean",
            ["ku"] = "Kurdish",
            ["kj"] = "Kuanyama, Kwanyama",
            ["la"] = "Latin",
            ["lb"] = "Luxembourgish, Letzeburgesch",
            ["lg"] = "Ganda",
            ["li"] = "Limburgan, Limburger, Limburgish",
            ["ln"] = "Lingala",
            ["lo"] = "Lao",
            ["lt"] = "Lithuanian",
            ["lu"] = "Luba-Katanga",
            ["lv"] = "Latvian",
            ["gv"] = "Manx",
            ["mk"] = "Macedonian",
            ["mg"] = "Malagasy",
            ["ms"] = "Malay",
            ["ml"] = "Malayalam",
            ["mt"] = "Maltese",
            ["mi"] = "Maori",
            ["mr"] = "Marathi",
            ["mh"] = "Marshallese",
            ["mn"] = "Mongolian",
            ["na"] = "Nauru",
            ["nv"] = "Navajo, Navaho",
            ["nd"] = "North Ndebele",
            ["ne"] = "Nepali",
            ["ng"] = "Ndonga",
            ["nb"] = "Norwegian Bokmål",
            ["nn"] = "Norwegian Nynorsk",
            ["no"] = "Norwegian",
            ["ii"] = "Sichuan Yi, Nuosu",
            ["nr"] = "South Ndebele",
            ["oc"] = "Occitan",
            ["oj"] = "Ojibwa",
            ["cu"] = "Church Slavic, Old Slavonic, Church Slavonic, Old Bulgarian, Old Church Slavonic",
            ["om"] = "Oromo",
            ["or"] = "Oriya",
            ["os"] = "Ossetian, Ossetic",
            ["pa"] = "Panjabi, Punjabi",
            ["pi"] = "Pali",
            ["fa"] = "Persian",
            ["pl"] = "Polish",
            ["ps"] = "Pashto, Pushto",
            ["pt"] = "Portuguese",
            ["qu"] = "Quechua",
            ["rm"] = "Romansh",
            ["rn"] = "Rundi",
            ["ro"] = "Romanian, Moldavian, Moldovan",
            ["ru"] = "Russian",
            ["sa"] = "Sanskrit",
            ["sc"] = "Sardinian",
            ["sd"] = "Sindhi",
            ["se"] = "Northern Sami",
            ["sm"] = "Samoan",
            ["sg"] = "Sango",
            ["sr"] = "Serbian",
            ["gd"] = "Gaelic, Scottish Gaelic",
            ["sn"] = "Shona",
            ["si"] = "Sinhala, Sinhalese",
            ["sk"] = "Slovak",
            ["sl"] = "Slovenian",
            ["so"] = "Somali",
            ["st"] = "Southern Sotho",
            ["es"] = "Spanish, Castilian",
            ["su"] = "Sundanese",
            ["sw"] = "Swahili",
            ["ss"] = "Swati",
            ["sv"] = "Swedish",
            ["ta"] = "Tamil",
            ["te"] = "Telugu",
            ["tg"] = "Tajik",
            ["th"] = "Thai",
            ["ti"] = "Tigrinya",
            ["bo"] = "Tibetan",
            ["tk"] = "Turkmen",
            ["tl"] = "Tagalog",
            ["tn"] = "Tswana",
            ["to"] = "Tonga (Tonga Islands)",
            ["tr"] = "Turkish",
            ["ts"] = "Tsonga",
            ["tt"] = "Tatar",
            ["tw"] = "Twi",
            ["ty"] = "Tahitian",
            ["ug"] = "Uighur, Uyghur",
            ["uk"] = "Ukrainian",
            ["ur"] = "Urdu",
            ["uz"] = "Uzbek",
            ["ve"] = "Venda",
            ["vi"] = "Vietnamese",
            ["vo"] = "Volapük",
            ["wa"] = "Walloon",
            ["cy"] = "Welsh",
            ["wo"] = "Wolof",
            ["fy"] = "Western Frisian",
            ["xh"] = "Xhosa",
            ["yi"] = "Yiddish",
            ["yo"] = "Yoruba",
            ["za"] = "Zhuang, Chuang",
            ["zu"] = "Zulu",
        };

        /// <summary>Languages translated by the .Net Core Framework</summary>
        /// other option to get all supported cultures: http://www.csharp-examples.net/culture-names/
        /// see next func
        public static Dictionary<string, string> NativeLanguages = new Dictionary<string, string>()
        {
            //            ["ab"] = "Unknown Language (ab)",
            ["aa"] = "Qafar",
            ["af"] = "Afrikaans",
            ["ak"] = "Akan",
            ["sq"] = "shqip",
            ["am"] = "አማርኛ",
            ["ar"] = "العربية",
            //            ["an"] = "Unknown Language (an)",
            ["hy"] = "Հայերեն",
            ["as"] = "অসমীয়া",
            //["av"] = "Unknown Language (av)",
            //["ae"] = "Unknown Language (ae)",
            //["ay"] = "Unknown Language (ay)",
            ["az"] = "azərbaycan",
            ["bm"] = "bamanakan",
            ["ba"] = "Башҡорт",
            ["eu"] = "euskara",
            ["be"] = "Беларуская",
            ["bn"] = "বাংলা",
            //["bh"] = "Unknown Language (bh)",
            //["bi"] = "Unknown Language (bi)",
            ["bs"] = "bosanski",
            ["br"] = "brezhoneg",
            ["bg"] = "български",
            ["my"] = "ဗမာ",
            ["ca"] = "català",
            //["ch"] = "Unknown Language (ch)",
            ["ce"] = "нохчийн",
            //["ny"] = "Unknown Language (ny)",
            ["zh"] = "中文",
            //["cv"] = "Unknown Language (cv)",
            ["kw"] = "kernewek",
            ["co"] = "Corsu",
            //["cr"] = "Unknown Language (cr)",
            ["hr"] = "hrvatski",
            ["cs"] = "čeština",
            ["da"] = "dansk",
            ["dv"] = "ދިވެހިބަސް",
            ["nl"] = "Nederlands",
            ["dz"] = "རྫོང་ཁ",
            ["en"] = "English",
            ["eo"] = "esperanto",
            ["et"] = "eesti",
            ["ee"] = "Eʋegbe",
            ["fo"] = "føroyskt",
            //["fj"] = "Unknown Language (fj)",
            ["fi"] = "suomi",
            ["fr"] = "français",
            ["ff"] = "Fulah",
            ["gl"] = "galego",
            ["ka"] = "ქართული",
            ["de"] = "Deutsch",
            ["el"] = "Ελληνικά",
            ["gn"] = "Avañe’ẽ",
            ["gu"] = "ગુજરાતી",
            //["ht"] = "Unknown Language (ht)",
            ["ha"] = "Hausa",
            ["he"] = "עברית",
            //["hz"] = "Unknown Language (hz)",
            ["hi"] = "हिन्दी",
            //["ho"] = "Unknown Language (ho)",
            ["hu"] = "magyar",
            ["ia"] = "interlingua",
            ["id"] = "Indonesia",
            //["ie"] = "Unknown Language (ie)",
            ["ga"] = "Gaeilge",
            ["ig"] = "Igbo",
            //["ik"] = "Unknown Language (ik)",
            //["io"] = "Unknown Language (io)",
            ["is"] = "íslenska",
            ["it"] = "italiano",
            ["iu"] = "Inuktitut",
            ["ja"] = "日本語",
            ["jv"] = "Basa Jawa",
            ["kl"] = "kalaallisut",
            ["kn"] = "ಕನ್ನಡ",
            ["kr"] = "Kanuri",
            ["ks"] = "کٲشُر",
            ["kk"] = "қазақ тілі",
            ["km"] = "ភាសាខ្មែរ",
            ["ki"] = "Gikuyu",
            ["rw"] = "Kinyarwanda",
            ["ky"] = "Кыргыз",
            //["kv"] = "Unknown Language (kv)",
            //["kg"] = "Unknown Language (kg)",
            ["ko"] = "한국어",
            ["ku"] = "کوردیی ناوەڕاست",
            //["kj"] = "Unknown Language (kj)",
            ["la"] = "lingua latīna", // ancient
            ["lb"] = "Lëtzebuergesch",
            ["lg"] = "Luganda",
            //["li"] = "Unknown Language (li)",
            ["ln"] = "lingála",
            ["lo"] = "ລາວ",
            ["lt"] = "lietuvių",
            ["lu"] = "Tshiluba",
            ["lv"] = "latviešu",
            ["gv"] = "Gaelg",
            ["mk"] = "македонски",
            ["mg"] = "Malagasy",
            ["ms"] = "Melayu",
            ["ml"] = "മലയാളം",
            ["mt"] = "Malti",
            ["mi"] = "Reo Māori",
            ["mr"] = "मराठी",
            //["mh"] = "Unknown Language (mh)",
            ["mn"] = "монгол",
            //["na"] = "Unknown Language (na)",
            //["nv"] = "Unknown Language (nv)",
            ["nd"] = "isiNdebele",
            ["ne"] = "नेपाली",
            //["ng"] = "Unknown Language (ng)",
            ["nb"] = "norsk bokmål",
            ["nn"] = "nynorsk",
            ["no"] = "norsk",
            ["ii"] = "ꆈꌠꁱꂷ",
            ["nr"] = "isiNdebele",
            ["oc"] = "Occitan",
            //["oj"] = "Unknown Language (oj)",
            ["cu"] = "церковнослове́нскїй", // ancient, in use by Orthodox Church
            ["om"] = "Oromoo",
            ["or"] = "ଓଡ଼ିଆ",
            ["os"] = "ирон",
            ["pa"] = "ਪੰਜਾਬੀ",
            //["pi"] = "Unknown Language (pi)", // Pali
            ["fa"] = "فارسی",
            ["pl"] = "polski",
            ["ps"] = "پښتو",
            ["pt"] = "português",
            ["qu"] = "Runasimi",
            ["rm"] = "rumantsch",
            ["rn"] = "Ikirundi",
            ["ro"] = "română",
            ["ru"] = "русский",
            //["sa"] = "संस्कृत", // Sanskri
            //["sc"] = "Unknown Language (sc)",
            ["sd"] = "سنڌي",
            ["se"] = "davvisámegiella",
            //["sm"] = "Unknown Language (sm)",
            ["sg"] = "Sängö",
            ["sr"] = "srpski",
            ["gd"] = "Gàidhlig",
            ["sn"] = "chiShona",
            ["si"] = "සිංහල",
            ["sk"] = "slovenčina",
            ["sl"] = "slovenščina",
            ["so"] = "Soomaali",
            ["st"] = "Sesotho",
            ["es"] = "español",
            //["su"] = "Unknown Language (su)",
            ["sw"] = "Kiswahili",
            ["ss"] = "Siswati",
            ["sv"] = "svenska",
            ["ta"] = "தமிழ்",
            ["te"] = "తెలుగు",
            ["tg"] = "Тоҷикӣ",
            ["th"] = "ไทย",
            ["ti"] = "ትግርኛ",
            ["bo"] = "བོད་ཡིག",
            ["tk"] = "Türkmen dili",
            //["tl"] = "Unknown Language (tl)",
            ["tn"] = "Setswana",
            ["to"] = "lea fakatonga",
            ["tr"] = "Türkçe",
            ["ts"] = "Xitsonga",
            ["tt"] = "Татар",
            //["tw"] = "Unknown Language (tw)",
            //["ty"] = "Unknown Language (ty)",
            ["ug"] = "ئۇيغۇرچە",
            ["uk"] = "українська",
            ["ur"] = "اُردو",
            ["uz"] = "o‘zbek",
            ["ve"] = "Tshivenḓa",
            ["vi"] = "Tiếng Việt",
            ["vo"] = "Volapük",
            //["wa"] = "Unknown Language (wa)",
            ["cy"] = "Cymraeg",
            ["wo"] = "Wolof",
            ["fy"] = "Frysk",
            ["xh"] = "isiXhosa",
            ["yi"] = "ייִדיש",
            ["yo"] = "Èdè Yorùbá",
            //["za"] = "Unknown Language (za)",
            ["zu"] = "isiZulu",
        };

        // Could be used to generate a list above
        static void GetAllSupportedCultures()
        {
            // get culture names
            List<string> list = new List<string>();
            foreach (CultureInfo ci in CultureInfo.GetCultures(CultureTypes.AllCultures))
            {
                string specName = "(none)";
                try { specName = CultureInfo.CreateSpecificCulture(ci.Name).Name; } catch { }
                list.Add(String.Format("{0,-12}{1,-12}{2}", ci.Name, specName, ci.EnglishName));
            }

            list.Sort();  // sort by name

            // write to console
            Console.WriteLine("CULTURE   SPEC.CULTURE  ENGLISH NAME");
            Console.WriteLine("--------------------------------------------------------------");
            foreach (string str in list)
                Console.WriteLine(str);

            // outputs
            //CULTURE   SPEC.CULTURE  ENGLISH NAME
            //--------------------------------------------------------------
            //                        Invariant Language (Invariant Country)
            //af          af-ZA       Afrikaans
            //af-ZA       af-ZA       Afrikaans (South Africa)
            //ar          ar-SA       Arabic
            //ar-AE       ar-AE       Arabic (U.A.E.)
        }

        /// <summary>Indexed list to get both english and native language names</summary>
        public static Dictionary<string, (string English, string Native)> EnglishNativeLanguages
        {
            get
            {
                var dic = new Dictionary<string, (string English, string Native)>();
                foreach (var k in NativeLanguages.Keys)
                    dic.Add(k, (EnglishLanguages[k], NativeLanguages[k]));
                return dic;
            }
        }
        /// <summary>List to get both english and native language names</summary>
        public static Dictionary<string, string[]> EnglishNativeLanguages2
        {
            get
            {
                var dic = new Dictionary<string, string[]>();
                foreach (var k in NativeLanguages.Keys)
                    dic.Add(k, new[] { EnglishLanguages[k], NativeLanguages[k] });
                return dic;
            }
        }
    }
    internal static class StringExtensions
    {
        public static string IfNullOrEmpty(this string src, string substitute)
        {
            if (String.IsNullOrEmpty(src)) return substitute;
            return src;
        }
    }
}
