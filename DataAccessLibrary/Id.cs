using System;
using Slapper;

namespace TobyMeehan.Com.Data;

public record struct Id<T>(string Value);

public class IdConverter : AutoMapper.Configuration.ITypeConverter
{
    public object Convert(object value, Type type)
    {
        var conversionType = Nullable.GetUnderlyingType(type) ?? type;

        if (value.GetType() == conversionType)
        {
            return value;
        }
        
        return Activator.CreateInstance(conversionType, args: new object[] {value as string});
    }

    public bool CanConvert(object value, Type type)
    {
        var conversionType = Nullable.GetUnderlyingType(type) ?? type;
        
        return conversionType == typeof(Id<>) && value is string;
    }

    public int Order => 110;
}