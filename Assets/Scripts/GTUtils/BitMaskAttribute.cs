﻿using System;

using UnityEngine;

// Have to be defined somewhere in a runtime script file

namespace GTUtils
{
    public class BitMaskAttribute : PropertyAttribute
    {
        public Type PropType;

        public BitMaskAttribute(Type aType)
        {
            PropType = aType;
        }
    }
}