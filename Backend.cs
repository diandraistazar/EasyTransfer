namespace EasyTransfer.Backend;

using EasyTransfer.Types;

using System.Text.Json;
using System.Text.Json.Serialization;
using System.Net;
using System.Net.Sockets;
using System.Text;

[JsonSerializable(typeof(Header))]
[JsonSerializable(typeof(Status))]
internal partial class JsonContext : JsonSerializerContext {}

class HeaderParsing {
    public static byte[] Serialize(Header header) {
        return JsonSerializer.SerializeToUtf8Bytes(header, typeof(Header), JsonContext.Default);
    }
    public static byte[] Serialize(Status header) {
        return JsonSerializer.SerializeToUtf8Bytes(header, typeof(Status), JsonContext.Default);
    }
    public static dynamic Deserialize(byte[] bytes, Type type) {
        if(type == typeof(Header)) {
            return JsonSerializer.Deserialize(bytes, type, JsonContext.Default);
        }

        return JsonSerializer.Deserialize(bytes, type, JsonContext.Default);
    }
}

class Network {
    private static string iGot = "#DONT_WORRY_I_GOT_IT#";
    private static string resendPls = "#RESEND_PLS_I_MISSED_SOME#";

    private static TcpClient? clientTcp;
    private static TcpListener? serverTcp;

    private static byte[] ReceiveData() {
        NetworkStream stream = clientTcp.GetStream();
        byte[] data = new byte[512];

        int bytes = stream.Read(data, 0, data.Length);
        return data.Take(bytes).ToArray();
    }

    private static int ReceiveFile(Header header) {
        string fullpath = Program.dirPath + "/" + header.filename;
        NetworkStream stream = clientTcp.GetStream();
        byte [] buffer = new byte[512];
        int written = 0;
        
        while(written < header.length) {
            int bytes = stream.Read(buffer, 0, buffer.Length);
            if(bytes > 0) {
                File.AppendAllBytes(fullpath, buffer.Take(bytes).ToArray());
                written += bytes;
            } else {
                Log.Debug("Bytes is less than 1");
                break;
            }
        }

        Log.Debug($"Received {written} bytes file");
        return written;
    }

    public static void SendData(byte[] data) {
        NetworkStream stream = clientTcp?.GetStream();

        stream?.Write(data);
    }

    public static void SendFile(string filepath) {
        clientTcp.Client.SendFile(filepath);
    }

    public static bool SetupServer() {
        serverTcp = new (Program.ipAddress, Program.Port);
        serverTcp.Start();

        return true;
    }

    public static bool SetupClient() {
        clientTcp = new(Program.ipAddress.ToString(), Program.Port);

        return true;
    }

    public static void CloseSocket() {
        clientTcp?.Close();
        serverTcp?.Stop();
    }

    public static bool Send() {
        Log.Info("Waiting the client...");

        clientTcp = serverTcp.AcceptTcpClient();
        Log.Info("A client connected");

        for(int i = 0; i < Program.listOfFiles.Length; i++) {
            FileInfo file = new(Program.listOfFiles[i]);
            Header header = new() {
                index = i,
                length = file.Length,
                filename = file.Name,
                next = ( i == Program.listOfFiles.Length-1 ? false : true )
            };
            Status? status;

            SendData(HeaderParsing.Serialize(header));
            Log.Debug($"Send header");

            status = HeaderParsing.Deserialize(ReceiveData(), typeof(Status));
            if(status?.message != iGot) {
                i =- 1;
                continue;
            } else {
                Log.Succ($"Send \"{file.Name}\" to the connected client");
            }

            SendFile(file.FullName);
            status = HeaderParsing.Deserialize(ReceiveData(), typeof(Status));
            if(status?.message != iGot) {
                i =- 1;
                continue;
            }
        }

        return true;
    }

    public static bool Recv() {
        bool looping = true;

        while(looping) {
            Status status = new() { message = iGot };
            Header header = HeaderParsing.Deserialize(ReceiveData(), typeof(Header));

            if(!header.next) {
                looping = false;
            }


            SendData(HeaderParsing.Serialize(status));
            int bytes = ReceiveFile(header);
            if(bytes != header.length) {
                status.message = resendPls;
                SendData(HeaderParsing.Serialize(status));
                continue;
            } else {
                Log.Info($"Received \"{header.filename}\" ({bytes} bytes), stored to \"{Program.dirPath}\"");

                status.message = iGot;
                SendData(HeaderParsing.Serialize(status));
            }
        }

        return true;
    }
}