using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sticky
{
    class Number : HasValue
    {
       public decimal Value { get; set; }

        public override int GetHashCode()
        {
            return Value.GetHashCode();
        }

        public override bool Equals(object obj)
        {
            var other = obj as Number;
            if (obj == null)
            {
                return false;
            }

            return other.Value == Value;
        }

        public override string ToString()
        {
            return Value.ToString();
        }
    }
}
