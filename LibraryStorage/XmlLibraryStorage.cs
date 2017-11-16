using System;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Serialization;
using LibraryStorage.Entities;
using LibraryStorage.Interfaces;

namespace LibraryStorage
{
    public class XmlLibraryStorage : ILibraryStorage
    {
        public string FilePath { get; }

        public XmlSerializerNamespaces CatalogNamespaces { get; set; }

        public XmlLibraryStorage() : this(Path.GetTempFileName())
        {
        }

        public XmlLibraryStorage(string filePath) : this(filePath, new XmlSerializerNamespaces())
        {
        }

        public XmlLibraryStorage(XmlSerializerNamespaces catalogNamespaces) : this(Path.GetTempFileName(), catalogNamespaces)
        {
        }

        public XmlLibraryStorage(string filePath, XmlSerializerNamespaces catalogNamespaces)
        {
            if (string.IsNullOrEmpty(filePath) || ReferenceEquals(catalogNamespaces, null))
                throw new ArgumentNullException();

            this.CatalogNamespaces = catalogNamespaces;

            FilePath = filePath;
        }

        public void Archive(ICatalog catalog)
        {
            try
            {
                using (var fileStream = new FileStream(FilePath, FileMode.Create, FileAccess.Write))
                {
                    new XmlSerializer(typeof(Catalog)).Serialize(fileStream, catalog, CatalogNamespaces);
                }
            }
            catch (IOException ex)
            {
                throw new Exception("Can't save data", ex);
            }
        }

        public void Archive(Stream catalog)
        {
            try
            {
                using (var fileStream = new FileStream(FilePath, FileMode.Create, FileAccess.Write))
                {
                    catalog.Seek(0, SeekOrigin.Begin);
                    catalog.CopyTo(fileStream);
                }
            }
            catch (IOException ex)
            {
                throw new Exception("Can't save data", ex);
            }
        }

        public Stream LoadCatalog()
        {
            var stream = new FileStream(FilePath, FileMode.Open, FileAccess.ReadWrite);

            stream.Flush(true);

            return stream;
        }
    }
}
