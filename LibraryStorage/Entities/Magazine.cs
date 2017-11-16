using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Schema;
using System.Xml.Serialization;

namespace LibraryStorage.Entities
{
    [XmlRoot(Namespace = "http://LibraryStorage.Magazine/1.0.0.0", IsNullable = false)]
    public class Magazine : ICatalogEntity
    {
        [XmlAttribute("ISSN")]
        public string Id { get; set; }

        [XmlElement(Form = XmlSchemaForm.Unqualified)]
        public string Title { get; set; }

        [XmlElement(Form = XmlSchemaForm.Unqualified)]
        public string PublicationCity { get; set; }

        [XmlElement(Form = XmlSchemaForm.Unqualified)]
        public string Publisher { get; set; }

        [XmlElement(DataType = "date", Form = XmlSchemaForm.Unqualified)]
        public DateTime Edition { get; set; }

        [XmlElement(Form = XmlSchemaForm.Unqualified)]
        public int Pages { get; set; }

        [XmlElement(Form = XmlSchemaForm.Unqualified)]
        public string Remark { get; set; }

        [XmlElement(Form = XmlSchemaForm.Unqualified)]
        public int Number { get; set; }

        [XmlElement(Form = XmlSchemaForm.Unqualified)]
        public DateTime Date { get; set; }
    }
}
