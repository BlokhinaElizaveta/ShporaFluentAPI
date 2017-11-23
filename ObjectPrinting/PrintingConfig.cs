using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;

namespace ObjectPrinting
{
    public class PrintingConfig<TOwner> :IPrintingConfig
    {
        private readonly HashSet<Type> excludingTypes;
        private readonly HashSet<PropertyInfo> excludingProperties;
        private readonly Dictionary<Type, Delegate> typeSerializers;
        private readonly Dictionary<PropertyInfo, Delegate> propertySerializers;

        Dictionary<Type, Delegate> IPrintingConfig.TypeSerializers => typeSerializers;
        Dictionary<PropertyInfo, Delegate> IPrintingConfig.PropertySerializers => propertySerializers;

        public PrintingConfig()
        {
            excludingTypes = new HashSet<Type>(); 
            excludingProperties = new HashSet<PropertyInfo>();
            typeSerializers = new Dictionary<Type, Delegate>();
            propertySerializers = new Dictionary<PropertyInfo, Delegate>();
        }

        public PrintingConfig<TOwner> Excluding<T>()
        {
            excludingTypes.Add(typeof(T));
            return this;
        }

        public PrintingConfig<TOwner> Excluding<TPropType>(Expression<Func<TOwner, TPropType>> memberSelector)
        {
            excludingProperties.Add(memberSelector.GetProperty());
            return this;
        }

        public SerializeConfig<TOwner, TPropType> Printing<TPropType>(Expression<Func<TOwner, TPropType>> memberSelector)
        {  
            return new SerializeConfig<TOwner, TPropType>(this, memberSelector.GetProperty());
        }

        public SerializeConfig<TOwner, TPropType> Printing<TPropType>()
        {
            return new SerializeConfig<TOwner, TPropType>(this, typeof(TPropType));
        }

        public string PrintToString(TOwner obj)
        {
            return PrintToString(obj, 0);
        }

        private string PrintToString(object obj, int nestingLevel)
        {
            //TODO apply configurations
            if (obj == null)
                return "null" + Environment.NewLine;

            var finalTypes = new[]
            {
                typeof(int), typeof(double), typeof(float), typeof(string),
                typeof(DateTime), typeof(TimeSpan)
            };
            if (finalTypes.Contains(obj.GetType()))
                return obj + Environment.NewLine;

            var identation = new string('\t', nestingLevel + 1);
            var sb = new StringBuilder();
            var type = obj.GetType();
            sb.AppendLine(type.Name);
            foreach (var propertyInfo in type.GetProperties())
            {
                var typeProperty = propertyInfo.PropertyType;
                if (excludingTypes.Contains(typeProperty) || excludingProperties.Contains(propertyInfo))
                    continue;

                var value = propertyInfo.GetValue(obj);           

                if (typeSerializers.ContainsKey(typeProperty))
                    value = typeSerializers[typeProperty].DynamicInvoke(value);

                if (propertySerializers.ContainsKey(propertyInfo))
                    value = propertySerializers[propertyInfo].DynamicInvoke(value);

                sb.Append(identation + propertyInfo.Name + " = " +
                          PrintToString(value, nestingLevel + 1));
            }
            return sb.ToString();
        }
    }
}