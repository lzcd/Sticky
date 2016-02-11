using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sticky
{
    class Text : HasValue
    {
        public string Value { get; set; }

        public override int GetHashCode()
        {
            return Value.GetHashCode();
        }

        public override bool Equals(object obj)
        {
            var other = obj as Text;
            if (obj == null )
            {
                return false;
            }

            return other.Value == Value;
        }
    }
}
