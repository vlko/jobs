﻿using System;

namespace vlko.core.ValidationAtribute
{
    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Property, AllowMultiple = false, Inherited = true)]
    public class AntiXssHtmlTextAttribute : Attribute
    {
    }
}
