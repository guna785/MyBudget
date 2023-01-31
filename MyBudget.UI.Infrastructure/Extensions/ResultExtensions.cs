﻿using MyBudget.Shared.Wrapper;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace MyBudget.UI.Infrastructure.Extensions
{
    public static class ResultExtensions
    {
        public static async Task<IResult<T>> ToResult<T>(this HttpResponseMessage response)
        {
            string responseAsString = await response.Content.ReadAsStringAsync();
            Result<T>? responseObject = JsonSerializer.Deserialize<Result<T>>(responseAsString, new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true,
                ReferenceHandler = ReferenceHandler.Preserve
            });
            return responseObject!;
        }

        public static async Task<IResult> ToResult(this HttpResponseMessage response)
        {
            string responseAsString = await response.Content.ReadAsStringAsync();
            Result? responseObject = JsonSerializer.Deserialize<Result>(responseAsString, new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true,
                ReferenceHandler = ReferenceHandler.Preserve
            });
            return responseObject;
        }

        public static async Task<PaginatedResult<T>> ToPaginatedResult<T>(this HttpResponseMessage response)
        {
            string responseAsString = await response.Content.ReadAsStringAsync();
            PaginatedResult<T>? responseObject = JsonSerializer.Deserialize<PaginatedResult<T>>(responseAsString, new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            });
            return responseObject!;
        }
    }
}
