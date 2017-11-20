using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ObjectPrinting
{
    public static class SerializeExtension
    {
        public static PrintingConfig<TOwner> Using<TOwner>(this SerializeConfig<TOwner, double> config, CultureInfo culture)
        {
            var printingConfig = ((ISerializeConfig<TOwner, double>) config).PrintingConfig;
            printingConfig.SerializationForType.Add(typeof(double), new Func<double, string>(x => x.ToString(culture)));
            return printingConfig;
        }

        public static PrintingConfig<TOwner> TrimmedString<TOwner>(this SerializeConfig<TOwner, string> config,
            int length)
        {
            var printingConfig = ((ISerializeConfig<TOwner, string>)config).PrintingConfig;
            var property = ((ISerializeConfig<TOwner, string>) config).Property;
            printingConfig.SerializationForProperty.Add(property, new Func<string, string>(v => v.ToString().Substring(0, length)));
            return printingConfig;
        }

        public static string PrintToString<T>(this T obj, Func<PrintingConfig<T>, PrintingConfig<T>> config)
        {
            return config(ObjectPrinter.For<T>()).PrintToString(obj);
        }

    }
}
