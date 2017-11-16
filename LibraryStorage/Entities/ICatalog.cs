using System;
using System.Collections.Generic;

namespace LibraryStorage.Entities
{
    public interface ICatalog
    {
        string Library { get; set; }

        DateTime RegistrationDate { get; set; }

        IEnumerable<ICatalogEntity> Entities { get; }
    }
}