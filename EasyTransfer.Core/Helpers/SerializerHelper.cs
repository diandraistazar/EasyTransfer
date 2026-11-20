using System.Text.Json;
using System.Text.Json.Serialization;
using EasyTransfer.Core.Logging;
using EasyTransfer.Core.Models.Filesystem;
using EasyTransfer.Core.Models.Transfer;

namespace EasyTransfer.Core.Helpers;

[JsonSerializable(typeof(TransferData))]
[JsonSerializable(typeof(SimpleFileData))]
[JsonSerializable(typeof(Status))]
internal partial class JsonContext : JsonSerializerContext {}

public static class SerializerHelper
{
    public static bool Serialize<T>(T obj, out byte[]? buffer)
    {
        buffer = default;
        
        try
        {
            buffer = JsonSerializer.SerializeToUtf8Bytes(obj, typeof(T), JsonContext.Default);
        }
        catch
        {
            return false;
        }
        
        return true;
    }

    public static bool Deserialize<T>(byte[] bytes, out T? buffer)
    {
        buffer = default;
        
        try
        {
            object? obj = JsonSerializer.Deserialize(bytes, typeof(T), JsonContext.Default);

            if(obj != null)
                buffer = (T)obj;
            else
                new NullReferenceException();
        }
        catch
        {
            return false;
        }

        return true;
    }
}