using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Linq;
using System.Xml.Schema;
using System.Xml.Serialization;
using LibraryStorage;
using LibraryStorage.Entities;
using LibraryStorage.Interfaces;

namespace LibraryService
{
    public class XmlLibraryService : IEnumerable<ICatalogEntity>, IDisposable
    {
        private XmlLibraryStorage storage;
        private Stream stream;
        private XmlReader xmlReader;

        public string FilePath => storage.FilePath;

        public XmlReaderSettings ReaderSettings { get; } = new XmlReaderSettings
        {
            Async = true,
            IgnoreWhitespace = true
        };

        public XmlWriterSettings WriterSettings { get; } = new XmlWriterSettings
        {
            Indent = true,
            OmitXmlDeclaration = true
        };

        private readonly Dictionary<Type, XmlSerializer> serializers;
        private readonly Dictionary<Type, XmlSerializerNamespaces> namepsaces;

        private readonly XmlSchemaSet xmlSchemaSet;

        private string catalogCloseElement = "</ctl:Catalog>";

        public XmlLibraryService() : this(new Catalog())
        {
        }

        public XmlLibraryService(Catalog catalog) : this(null, catalog)
        {
        }

        public XmlLibraryService(string xmlFilePath) : this(xmlFilePath, null)
        {
        }

        public XmlLibraryService(string xmlFilePath, Catalog catalog)
        {
            XmlQualifiedName catalogName = new XmlQualifiedName("ctl", "http://LibraryStorage.Catalog/1.0.0.0");
            XmlQualifiedName bookName = new XmlQualifiedName("bk", "http://LibraryStorage.Book/1.0.0.0");
            XmlQualifiedName magazineName = new XmlQualifiedName("mgz", "http://LibraryStorage.Magazine/1.0.0.0");
            XmlQualifiedName patentName = new XmlQualifiedName("ptn", "http://LibraryStorage.Patent/1.0.0.0");

            namepsaces = new Dictionary<Type, XmlSerializerNamespaces>
            {
                {
                    typeof(Catalog), new XmlSerializerNamespaces(new[] { catalogName, bookName, magazineName, patentName })
                },
                {
                    typeof(Book), new XmlSerializerNamespaces(new[] { bookName })
                },
                {
                    typeof(Patent), new XmlSerializerNamespaces(new[] { patentName })
                },
                {
                    typeof(Magazine), new XmlSerializerNamespaces(new[] { magazineName })
                }
            };

            serializers = new Dictionary<Type, XmlSerializer>
            {
                {
                    typeof(Catalog), new XmlSerializer(typeof(Catalog))
                },
                {
                    typeof(Book), new XmlSerializer(typeof(Book))
                },
                {
                    typeof(Patent), new XmlSerializer(typeof(Patent))
                },
                {
                    typeof(Magazine), new XmlSerializer(typeof(Magazine))
                }
            };

            xmlSchemaSet = new XmlSchemaSet();
            xmlSchemaSet.Add($"{catalogName.Namespace}", @"...\...\...\LibraryService\Schemas\CatalogSchema.xsd");
            xmlSchemaSet.Add($"{bookName.Namespace}", @"...\...\...\LibraryService\Schemas\BookSchema.xsd");
            xmlSchemaSet.Add($"{magazineName.Namespace}", @"...\...\...\LibraryService\Schemas\MagazineSchema.xsd");
            xmlSchemaSet.Add($"{patentName.Namespace}", @"...\...\...\LibraryService\Schemas\PatentSchema.xsd");

            storage = ReferenceEquals(xmlFilePath, null) ? new XmlLibraryStorage(namepsaces.Values.First()) : new XmlLibraryStorage(xmlFilePath, namepsaces.Values.First());
            
            if (!ReferenceEquals(catalog, null))
            {
                storage.Archive(catalog);
            }

            stream = storage.LoadCatalog();

            Reset();
        }

        public IEnumerator<ICatalogEntity> GetEnumerator()
        {
            return new Enumerator(this);
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        public void Add(ICatalogEntity entity, bool validate = false)
        {
            stream.Position = stream.Length - catalogCloseElement.Length;

            using (var xmlWriter = XmlWriter.Create(stream, WriterSettings))
            {
                serializers[entity.GetType()].Serialize(xmlWriter, entity, namepsaces[entity.GetType()]);
            }

            var sw = new StreamWriter(stream);

            sw.Write($"\n{catalogCloseElement}");
            sw.Flush();

            stream.Seek(0, SeekOrigin.Begin);
        }

        public void Validate(ICatalogEntity entity)
        {
            XDocument doc;

            using (var stream = new MemoryStream())
            {
                serializers[entity.GetType()].Serialize(stream, entity, namepsaces[entity.GetType()]);

                stream.Seek(0, SeekOrigin.Begin);

                doc = XDocument.Load(stream);
            }

            StringBuilder errors = new StringBuilder();

            doc.Validate(xmlSchemaSet, (sender, e) =>
            {
                XElement badElement = sender as XElement;

                errors.AppendLine($"{e.Severity} in <{badElement?.Name.LocalName ?? "~element not found~"}> element at line {e.Exception.LineNumber} in position {e.Exception.LinePosition} \"{e.Exception.InnerException?.Message ?? e.Message}\"");
            });

            if (errors.Length > 1)
            {
                throw new ValidationException(errors.ToString());
            }
        }
        public void Save()
        {
            stream?.Dispose();
            xmlReader?.Dispose();
        }

        public void Dispose()
        {
            Save();
        }

        private void Reset()
        {

            stream.Seek(0, SeekOrigin.Begin);

            xmlReader = XmlReader.Create(stream, ReaderSettings);

            xmlReader.ReadToFollowing("Catalog", "http://LibraryStorage.Catalog/1.0.0.0");
            xmlReader.Read();
        }

        #region Enumerator

        private struct Enumerator : IEnumerator<ICatalogEntity>, IDisposable, IEnumerator
        {
            private readonly XmlLibraryService service;
            private XmlReader xmlReader;
            private readonly MemoryStream stream;

            public ICatalogEntity Current
            {
                get
                {
                    xmlReader.ReadSubtree();

                    ICatalogEntity entity = null;

                    switch (xmlReader.LocalName)
                    {
                        case "Book":

                            entity = (ICatalogEntity)service.serializers[typeof(Book)].Deserialize(xmlReader);
                            break;
                        case "Magazine":

                            entity = (ICatalogEntity)service.serializers[typeof(Magazine)].Deserialize(xmlReader);
                            break;
                        case "Patent":

                            entity = (ICatalogEntity)service.serializers[typeof(Patent)].Deserialize(xmlReader);
                            break;
                    }

                    service.Validate(entity);

                    return entity;
                }
            }

            object IEnumerator.Current => Current;

            internal Enumerator(XmlLibraryService service)
            {
                stream = new MemoryStream();

                long save = service.stream.Position;

                service.stream.Seek(0, SeekOrigin.Begin);
                service.stream.CopyTo(stream);
                service.stream.Position = save;

                this.service = service;

                xmlReader = null;

                Reset();
            }

            public void Dispose()
            {
                stream.Dispose();
                xmlReader.Dispose();
            }

            public bool MoveNext()
            {
                while (!xmlReader.EOF)
                {
                    if (xmlReader.NodeType == XmlNodeType.Element &&
                        (xmlReader.LocalName == "Book" || xmlReader.LocalName == "Patent" ||
                         xmlReader.LocalName == "Magazine"))
                    {
                        return true;
                    }
                    if (xmlReader.NodeType == XmlNodeType.EndElement && xmlReader.LocalName == "Catalog")
                    {
                        return false;
                    }
                }

                return false;
            }

            public void Reset()
            {
                stream.Seek(0, SeekOrigin.Begin);

                xmlReader = XmlReader.Create(stream, service.ReaderSettings);

                xmlReader.ReadToFollowing("Catalog", "http://LibraryStorage.Catalog/1.0.0.0");
                xmlReader.Read();
            }
        }
        #endregion
    }
}
