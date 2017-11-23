using System;
using System.Collections.Generic;
using System.Reflection;

namespace ObjectPrinting
{
    public interface IPrintingConfig
    {
        Dictionary<Type, Delegate> TypeSerializers { get; }
        Dictionary<PropertyInfo, Delegate> PropertySerializers { get; }
    }
}
