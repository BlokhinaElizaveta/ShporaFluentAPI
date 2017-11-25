using System;
using System.Globalization;

namespace ObjectPrinting
{
    public static class SerializeExtension
    {
        public static PrintingConfig<TOwner> Using<TOwner>(this SerializeConfig<TOwner, double> config, CultureInfo culture)
        {
            var printingConfig = (config as ISerializeConfig<TOwner, double>).PrintingConfig;
            (printingConfig as IPrintingConfig).TypeSerializers.Add(typeof(double), new Func<double, string>(x => x.ToString(culture)));
            return printingConfig;
        }

        public static PrintingConfig<TOwner> TrimmedString<TOwner>(this SerializeConfig<TOwner, string> config,
            int length)
        {
            var printingConfig = (config as ISerializeConfig<TOwner, string>).PrintingConfig;
            var property = (config as ISerializeConfig<TOwner, string>).Property;
            (printingConfig as IPrintingConfig).PropertySerializers.Add(property, new Func<string, string>(v => v?.Substring(0, length) ?? "<null>"));
            return printingConfig;
        }

        public static string PrintToString<T>(this T obj, Func<PrintingConfig<T>, PrintingConfig<T>> config)
        {
            return config(ObjectPrinter.For<T>()).PrintToString(obj);
        }

    }
}
