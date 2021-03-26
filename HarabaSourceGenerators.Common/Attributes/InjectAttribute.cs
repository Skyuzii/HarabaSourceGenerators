using System;
using System.Collections.Generic;
using System.Text;

namespace HarabaSourceGenerators.Common.Attributes
{
    public class InjectAttribute : Attribute
    {
        public InjectAttribute()
        {
        }

        public InjectAttribute(int index)
        {
            Index = index;
        }
        
        public int Index { get; set; }
    }
}
