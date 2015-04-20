using System;
using System.Collections.Generic;

namespace ObjectDiff
{
    public class DiffOptions
    {
        public DiffOptions()
        {
            Reset();
        }

        public static DiffOptions Default() { return new DiffOptions(); }

        public List<Type> IgnoreMemberAttributes;

        public void Reset()
        {
            IgnoreMemberAttributes = new List<Type>();
        }
    }
}
