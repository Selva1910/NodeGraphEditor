using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace NodeGraph
{
    public class ExposedPropertyAttribute : Attribute
    {

        public ExposedPropertyAttribute()
        {

        }

    }

    public class DisplayNameAttribute : Attribute
    {
        string displayName;

        public string Name => displayName;
        public DisplayNameAttribute(string name)
        {
            displayName = name;
        }

    }
}
