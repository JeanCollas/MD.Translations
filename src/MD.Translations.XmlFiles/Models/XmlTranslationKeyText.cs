using System.Xml.Serialization;

namespace MD.Translations
{
    /// <summary>
    /// Basic key/value translation
    /// </summary>
    [XmlType("Entry")]
    public struct XmlTranslationKeyText
    {
        [XmlAttribute]
        public string Key { get; set; }
        [XmlAttribute]
        public string Text { get; set; }
    }
}
