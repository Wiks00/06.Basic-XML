using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Schema;
using System.Xml.Serialization;

namespace LibraryStorage.Entities
{
    [XmlRoot(Namespace = "http://LibraryStorage.Catalog/1.0.0.0", IsNullable = false)]
    public class Catalog : ICatalog
    {
        [XmlAttribute]
        public string Library { get; set; }

        [XmlAttribute]
        public DateTime RegistrationDate { get; set; }

        [XmlElement(ElementName = "Book", Namespace = "http://LibraryStorage.Book/1.0.0.0")]
        public List<Book> Books { get; set; }

        [XmlElement(typeof(Magazine), ElementName = "Magazine", Namespace = "http://LibraryStorage.Magazine/1.0.0.0")]
        public List<Magazine> Magazines { get; set; }

        [XmlElement(typeof(Patent), ElementName = "Patent", Namespace = "http://LibraryStorage.Patent/1.0.0.0")]
        public List<Patent> Patents { get; set; }

        [XmlIgnore]
        IEnumerable<ICatalogEntity> ICatalog.Entities => Books.Union(Magazines.Union(Patents.Cast<ICatalogEntity>()));
    }
}
