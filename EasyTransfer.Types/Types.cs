namespace EasyTransfer.Types;

using System.Text.Json;
using System.Text.Json.Serialization;

[JsonSerializable(typeof(FileHeader))]
[JsonSerializable(typeof(Status))]
internal partial class JsonContext : JsonSerializerContext {}

public struct FileHeader {
    public int Index { get; set; }
    public string Filename { get; set; }
    public long Length { get; set; }
    public bool Next { get; set; }

    public static byte[] Serialize(FileHeader header) {
        return JsonSerializer.SerializeToUtf8Bytes(header, typeof(FileHeader), JsonContext.Default);
    }

    public static FileHeader Deserialize(byte[] bytes) {
        return (FileHeader)JsonSerializer.Deserialize(bytes, typeof(FileHeader), JsonContext.Default);
    }
}

public struct Status {
    public static string iGot = "#DONT_WORRY_I_GOT_IT#";
    public static string resendPls = "#RESEND_PLS_I_MISSED_SOME#";

    public string Message { get; set; } 

    public static byte[] Serialize(Status status) {
        return JsonSerializer.SerializeToUtf8Bytes(status, typeof(Status), JsonContext.Default);
    }

    public static Status Deserialize(byte[] bytes) {
        return (Status)JsonSerializer.Deserialize(bytes, typeof(Status), JsonContext.Default);
    }
}