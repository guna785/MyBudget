using MyBudget.Application.Interfaces.Serialization.Options;
using System.Text.Json;

namespace MyBudget.Application.Serialization.Options
{
    public class SystemTextJsonOptions : IJsonSerializerOptions
    {
        public JsonSerializerOptions JsonSerializerOptions { get; } = new();
    }
}
