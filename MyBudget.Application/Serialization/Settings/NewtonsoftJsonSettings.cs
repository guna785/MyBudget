using MyBudget.Application.Interfaces.Serialization.Settings;
using Newtonsoft.Json;

namespace MyBudget.Application.Serialization.Settings
{
    public class NewtonsoftJsonSettings : IJsonSerializerSettings
    {
        public JsonSerializerSettings JsonSerializerSettings { get; } = new();
    }
}
