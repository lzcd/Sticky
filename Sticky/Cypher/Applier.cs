﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sticky.Cypher
{
    interface Applier
    {
        ApplierResult Apply(List<Node> nodes);
    }
}
