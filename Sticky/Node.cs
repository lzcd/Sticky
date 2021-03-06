﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sticky
{
    class Node : HasLabel, HasProperties
    {
        public string Label { get; set; }
        
        public Dictionary<string, HasValue> PropertyByName { get; } = new Dictionary<string, HasValue>();

        public List<Relationship> IncomingRelationships { get; } = new List<Relationship>();
        public List<Relationship> OutgoingRelationships { get; } = new List<Relationship>();

    }
}
