using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LibraryStorage.Entities;

namespace LibraryStorage.Interfaces
{
    public interface ILibraryStorage
    {
        void Archive(ICatalog catalog);

        void Archive(Stream catalog);

        Stream LoadCatalog();
    }
}
