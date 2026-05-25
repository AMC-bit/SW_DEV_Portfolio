using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProgettoAnagrafiche.Models
    {
    public abstract class BaseEntity
        {
        // abstract: cannot be instantiated, must be inherited, the class that inherits this must implement Validate and Normalize
        // not using virtual because I set no default
        public abstract List<string> Validate();
        public abstract void Normalize();
        }
    }
