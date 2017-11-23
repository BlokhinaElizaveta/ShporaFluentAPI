using System;
using System.Reflection;

namespace ObjectPrinting
{
    public interface ISerializeConfig<TOwner, TPropType>
    {
        PrintingConfig<TOwner> PrintingConfig { get; }
        PropertyInfo Property { get; }
        Type Type { get; }
    }
}
