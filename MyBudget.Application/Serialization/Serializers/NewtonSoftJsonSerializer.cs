using Microsoft.Extensions.Options;
using MyBudget.Application.Interfaces.Serialization.Serializers;
using MyBudget.Application.Serialization.Settings;
using Newtonsoft.Json;

namespace MyBudget.Application.Serialization.Serializers
{
    public class NewtonSoftJsonSerializer : IJsonSerializer
    {
        private readonly JsonSerializerSettings _settings;

        public NewtonSoftJsonSerializer(IOptions<NewtonsoftJsonSettings> settings)
        {
            _settings = settings.Value.JsonSerializerSettings;
        }

        public T Deserialize<T>(string text)
        {
            return JsonConvert.DeserializeObject<T>(text, _settings)!;
        }

        public string Serialize<T>(T obj)
        {
            return JsonConvert.SerializeObject(obj, _settings);
        }
    }
}
