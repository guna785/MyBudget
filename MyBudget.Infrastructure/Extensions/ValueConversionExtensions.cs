using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using MyBudget.Application.Interfaces.Serialization.Serializers;

namespace MyBudget.Infrastructure.Extensions
{
    public static class ValueConversionExtensions
    {
        public static PropertyBuilder<T> HasJsonConversion<T>(this PropertyBuilder<T> propertyBuilder, IJsonSerializer serializer) where T : class//, new()
        {
            ValueConverter<T, string> converter = new(
                v => serializer.Serialize(v),
                v => serializer.Deserialize<T>(v) // ?? new T()
            );

            ValueComparer<T> comparer = new(
                (l, r) => serializer.Serialize(l) == serializer.Serialize(r),
                v => v == null ? 0 : serializer.Serialize(v).GetHashCode(),
                v => serializer.Deserialize<T>(serializer.Serialize(v))
            );

            _ = propertyBuilder.HasConversion(converter);
            propertyBuilder.Metadata.SetValueConverter(converter);
            propertyBuilder.Metadata.SetValueComparer(comparer);
            _ = propertyBuilder.HasColumnType("nvarchar(max)");

            return propertyBuilder;
        }
    }
}
