using System;

namespace SafeWarehouseApp.Extensions
{
    public static class ByteArrayExtensions
    {
        public static string GetDataUrl(this byte[] buffer, string contentType)
        {
            var base64 = Convert.ToBase64String(buffer);
            return $"data:{contentType};base64,{base64}";
        }
    }
}