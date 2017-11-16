using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Linq;
using System.Xml.Serialization;
using LibraryStorage.Entities;

namespace Library.ConsoleTest
{
    class Program
    {
        static void Main(string[] args)
        {
            //var lib = new LibraryService.XmlLibraryService("test.xml");

            var lib = new LibraryService.XmlLibraryService("test.xml",
                new Catalog
                {
                    Books =
                        new List<Book>
                        {
                            new Book {Id = "book - s8a6ns987nwar", Title = "a", Authors = new List<string> {"f", "fg"}},
                            new Book {Id = "book - a-5ca-wc", Title = "b", Authors = new List<string> {"o"}}
                        }
                })
                {
                    new Book {Id = "book - 5-4c3c", Title = "y", Authors = new List<string> {"fffdfdf"}},
                    new Magazine {Id = "Magazine - warwafd54", Title = "y", Date = DateTime.Today},
                    new Patent {Id = "Patent - 5-4c3c", Title = "y", Inventors = new List<string> {"1"}},
                    new Magazine {Id = "Magazine - warwafd54", Title = "y", Date = DateTime.Today},
                    new Book {Id = "book - 5-4c3c", Title = "y", Authors = new List<string> {"fffdfdf"}}
                };

            foreach (var item in lib.Where(item => item is Book && item.Id != "book - 5-4c3c"))
            {
                Console.WriteLine(item.Id);

                break;
            }

            foreach (var item in lib.Skip(2).Take(2))
            {
                Console.WriteLine(item.Id);
            }

            Console.ReadKey();
        }
    }
}
