using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Schema;
using System.Xml.Serialization;

namespace LibraryStorage.Entities
{
    [XmlRoot(Namespace = "http://LibraryStorage.Patent/1.0.0.0", IsNullable = false)]
    public class Patent : ICatalogEntity
    {
        [XmlAttribute("RegistrationNumber")]
        public string Id { get; set; }

        [XmlElement(Form = XmlSchemaForm.Unqualified)]
        public string Title { get; set; }

        [XmlArray("Inventors")]
        [XmlArrayItem("Inventor", typeof(string), IsNullable = false, Form = XmlSchemaForm.Unqualified)]
        public List<string> Inventors { get; set; }

        [XmlElement(Form = XmlSchemaForm.Unqualified)]
        public string City { get; set; }

        [XmlElement("ApplyingDate", Form = XmlSchemaForm.Unqualified)]
        public DateTime Applying { get; set; }

        [XmlElement("PublicationDate", Form = XmlSchemaForm.Unqualified)]
        public DateTime Publication { get; set; }

        [XmlElement(Form = XmlSchemaForm.Unqualified)]
        public int Pages { get; set; }

        [XmlElement(Form = XmlSchemaForm.Unqualified)]
        public string Remark { get; set; }
    }
}
