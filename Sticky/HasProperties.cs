using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sticky
{
    interface HasProperties
    {
        Dictionary<string, HasValue> PropertyByName { get; }
    }
}
