using Microsoft.Extensions.Options;
using MyBudget.Application.Interfaces.Serialization.Serializers;
using MyBudget.Application.Serialization.Options;
using System.Text.Json;

namespace MyBudget.Application.Serialization.Serializers
{
    public class SystemTextJsonSerializer : IJsonSerializer
    {
        private readonly JsonSerializerOptions _options;

        public SystemTextJsonSerializer(IOptions<SystemTextJsonOptions> options)
        {
            _options = options.Value.JsonSerializerOptions;
        }

        public T Deserialize<T>(string data)
        {
            return JsonSerializer.Deserialize<T>(data, _options)!;
        }

        public string Serialize<T>(T data)
        {
            return JsonSerializer.Serialize(data, _options);
        }
    }
}
