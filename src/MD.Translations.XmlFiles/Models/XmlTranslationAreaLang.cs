using Newtonsoft.Json;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Xml;
using System.Xml.Schema;
using System.Xml.Serialization;

namespace MD.Translations
{
    [XmlType("Area")]
    public class XmlTranslationAreaLang //: IXmlSerializable
    {
        internal XmlTranslationAreaLang() { }

        [XmlAttribute]
        public string Lang { get; set; }
        [XmlAttribute]
        public string Area { get; set; }

        // We do not serialize this property
        [XmlIgnore]
        // We initialize the dictionary so it is never null, 
        //    then we do not need any setter
        public Dictionary<string, XmlTranslationAreaContext> Contexts { get; set; } = new Dictionary<string, XmlTranslationAreaContext>();

        // This property is the one serialized, however it is not used externally
        // We serialize the property by specifying its serialization name
        [XmlArray(nameof(Contexts))]
        /// <summary>List of all contexts for this lang|area</summary>
        public ObservableCollection<XmlTranslationAreaContext> XmlContexts
        {
            // If need to restore the list, it reconstructs it from the dictionary
            get
            {
                var list = new ObservableCollection<XmlTranslationAreaContext>(Contexts.Values);
                list.CollectionChanged += List_CollectionChanged;
                return list;
            }
            // When deserializing the values, it populates the dictionary
            set { foreach (var v in value) Contexts.Add(v.Name, v); }
        }

        private void List_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            if (e.OldItems != null)
                foreach (var i in e.OldItems)
                    Contexts.Remove(((XmlTranslationAreaContext)i).Name);
            if (e.NewItems != null)
                foreach (var i in e.NewItems)
                {
                    var item = i as XmlTranslationAreaContext;
                    Contexts[item.Name] = item;
                }
        }

        /// <summary>Helper to extract all missings from an area</summary>
        [XmlIgnore]
        [JsonIgnore]
        public Dictionary<string, XmlTranslationAreaContext> Missing
        {
            get
            {
                var dic = new Dictionary<string, XmlTranslationAreaContext>();
                foreach (var kvp in Contexts ?? new Dictionary<string, XmlTranslationAreaContext>())
                {
                    dic.Add(kvp.Key, kvp.Value.MissingContext);
                    //var c = kvp.Value;
                    //if (c.Missing != null && c.Missing.Count > 0)
                    //{
                    //    var cont = new List<XmlTranslationAreaContext>();
                    //    dic.Add(kvp.Key, cont);
                    //    foreach (var k in c.Missing)
                    //    {
                    //        cont.Add(k);
                    //    }
                    //}
                }
                return dic;
            }
        }

        //#region Serializer
        //public XmlSchema GetSchema()
        //{
        //    throw new System.NotImplementedException();
        //    return null;
        //}

        //public void ReadXml(XmlReader reader)
        //{
        //    //throw new System.NotImplementedException();
        //}

        //public void WriteXml(XmlWriter writer)
        //{
        //    //throw new System.NotImplementedException();
        //}
        //#endregion Serializer
    }
}
