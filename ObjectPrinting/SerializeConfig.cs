using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace ObjectPrinting
{
    public class SerializeConfig<TOwner, TPropType> : PrintingConfig<TOwner>, ISerializeConfig<TOwner, TPropType>
    {
        private PrintingConfig<TOwner> printingConfig;
        private Type type;
        private PropertyInfo property;
        
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
                printingConfig.SerializationForType[type] = func;
        }

        private void ChangeSerializationForProperty(Func<TPropType, string> func)
        {
            if (property != null)
                printingConfig.SerializationForProperty[property] = func;
        }


        PrintingConfig<TOwner> ISerializeConfig<TOwner, TPropType>.PrintingConfig => printingConfig;
        PropertyInfo ISerializeConfig<TOwner, TPropType>.Property => property;
    }

    public interface ISerializeConfig<TOwner, TPropType>
    {
        PrintingConfig<TOwner> PrintingConfig { get; }
        PropertyInfo Property { get; }
    }
}
