using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Schema;
using System.Xml.Serialization;

namespace LibraryStorage.Entities
{
    [XmlRoot(Namespace = "http://LibraryStorage.Book/1.0.0.0", IsNullable = false)]
    public class Book : ICatalogEntity
    {
        [XmlAttribute("ISBN")]
        public string Id { get; set; }

        [XmlElement(Form = XmlSchemaForm.Unqualified)]
        public string Title { get; set; }

        [XmlArray("Authors", Form = XmlSchemaForm.Unqualified)]
        [XmlArrayItem("Author", typeof(string), IsNullable = false, Form = XmlSchemaForm.Unqualified)]
        public List<string> Authors { get; set; }

        [XmlElement(Form = XmlSchemaForm.Unqualified)]
        public string Publication { get;set; }

        [XmlElement(Form = XmlSchemaForm.Unqualified)]
        public string Publisher { get; set; }

        [XmlElement(DataType = "date", Form = XmlSchemaForm.Unqualified)]
        public DateTime Edition { get; set; }

        [XmlElement(Form = XmlSchemaForm.Unqualified)]
        public int Pages { get; set; }

        [XmlElement(Form = XmlSchemaForm.Unqualified)]
        public string Remark { get; set; }
    }
}
