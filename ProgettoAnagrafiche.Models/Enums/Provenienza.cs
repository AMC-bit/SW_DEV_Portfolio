using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProgettoAnagrafiche.Models.Enums
    {
    public enum Provenienza
        {
        // no 0 for none because I don't want unspecified to ever be valid
        // 0 should be not set and forbidden
        LinkedIn = 1,
        SitoWeb = 2,
        Pubblicita = 3
        }
    }
