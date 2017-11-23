using System;
using System.Reflection;

namespace ObjectPrinting
{
    public class SerializeConfig<TOwner, TPropType> : PrintingConfig<TOwner>, ISerializeConfig<TOwner, TPropType>
    {
        private readonly PrintingConfig<TOwner> printingConfig;
        private readonly Type type;
        private readonly PropertyInfo property;

        PrintingConfig<TOwner> ISerializeConfig<TOwner, TPropType>.PrintingConfig => printingConfig;
        PropertyInfo ISerializeConfig<TOwner, TPropType>.Property => property;
        Type ISerializeConfig<TOwner, TPropType>.Type => type;

        public SerializeConfig(PrintingConfig<TOwner> printingConfig)
        {
            this.printingConfig = printingConfig;  
        }

        public SerializeConfig(PrintingConfig<TOwner> printingConfig, Type type)
        {
            this.printingConfig = printingConfig;
            this.type = type;
        }

        public SerializeConfig(PrintingConfig<TOwner> printingConfig, PropertyInfo property)
        {
            this.printingConfig = printingConfig;
            this.property = property;
        }

        public PrintingConfig<TOwner> Using(Func<TPropType, string> func)
        {    
            ChangeAlternativeSerialization(func);
            ChangeSerializationForProperty(func);
            return printingConfig;
        }

        private void ChangeAlternativeSerialization(Func<TPropType, string> func)
        {
            if (type != null)
                ((IPrintingConfig)printingConfig).TypeSerializers[type] = func;
        }

        private void ChangeSerializationForProperty(Func<TPropType, string> func)
        {
            if (property != null)
                ((IPrintingConfig)printingConfig).PropertySerializers[property] = func;
        }
    }
}
