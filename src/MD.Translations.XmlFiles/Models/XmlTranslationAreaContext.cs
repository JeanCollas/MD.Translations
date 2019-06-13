using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Xml.Serialization;

namespace MD.Translations
{
    /// <summary>Context of translations</summary>
    [XmlType("Context")]
    public class XmlTranslationAreaContext
    {
        /// <summary>Name of the context, may be empty</summary>
        [XmlAttribute]
        public string Name { get; set; }

        [XmlIgnore]
        // Accessed property
        public Dictionary<string, string> Entries { get; set; } = new Dictionary<string, string>();

        [XmlArray(nameof(Entries))]
        // Serialized property
        public ObservableCollection<XmlTranslationKeyText> XmlEntries
        {
            get
            {
                var list = new ObservableCollection<XmlTranslationKeyText>((from kvp in Entries
                                                     select new XmlTranslationKeyText() { Key = kvp.Key, Text = kvp.Value }));
                list.CollectionChanged += List_CollectionChanged;
                return list;
            }
            set
            {
                foreach (var v in value)
                    Entries.Add(v.Key, v.Text);
            }
        }

        private void List_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            if (e.OldItems != null)
                foreach (var i in e.OldItems)
                    Entries.Remove(((XmlTranslationKeyText)i).Key);
            if (e.NewItems != null)
                foreach (var i in e.NewItems)
                {
                    var item = (XmlTranslationKeyText)i;
                    Entries[item.Key] = item.Text;
                }

        }

        /// <summary>Missing keys saved so that translation can be completed</summary>
        [XmlArray("Missing")]
        public List<XmlTranslationKeyText> Missing { get; set; } = new List<XmlTranslationKeyText>();

        /// <summary>This is only information data to understand where the missing keys are from</summary>
        [XmlArray("MissingData")]
        public List<string> MissingInfo { get; set; } = new List<string>();

        [XmlIgnore]
        public XmlTranslationAreaContext MissingContext { get => new XmlTranslationAreaContext() { Name = Name, Missing = Missing }; }
    }
}
