using Microsoft.AspNetCore.Http;
using System.Text.Json;

namespace MedicalStore.Models
{
    public static class SessionExtensions
    {
        private static readonly JsonSerializerOptions _jsonOptions = new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true,
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase
        };

        // Lưu object vào session dưới dạng chuỗi JSON
        public static void SetObject<T>(this ISession session, string key, T value)
        {
            var json = JsonSerializer.Serialize(value, _jsonOptions);
            session.SetString(key, json);
        }

        // Lấy object từ session, chuyển lại về object gốc
        public static T? GetObject<T>(this ISession session, string key)
        {
            var json = session.GetString(key);
            return json == null ? default : JsonSerializer.Deserialize<T>(json, _jsonOptions);
        }
    }
}
