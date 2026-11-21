using System.Net;
using System.Net.Sockets;

using EasyTransfer.Core.Models.Transfer;
using EasyTransfer.Core.Models.Filesystem;
using EasyTransfer.Core.Helpers;
using EasyTransfer.Core.Logging;

namespace EasyTransfer.Core.Services;

internal sealed class ServerNetworkService : TransferModelBase
{
    private TransmissionService Transmission { get; init; }

    public ServerNetworkService()
    {   
        Transmission = new TransmissionService();
    }

    public bool Setup(out TcpListener? server)
    {
        server = default;
        
        if(!NetworkHelper.ParseIP(Address, out IPAddress? ip))
        {
            Log.Error($"The specified IP address is invalid");
            return false;
        }

        try
        {
            server = new TcpListener(ip, Port);
            server.Start();
        }
        catch
        {
            return false;
        }

        return true;
    }

    public bool Listening(TcpListener server, out TcpClient? client)
    {
        client = default;

        Log.Info($"{Address}:{Port} listening ... ", '\0');
        
        try
        {
            client = server.AcceptTcpClient();
        }
        catch
        {
            return false;
        }
        
        Log.Print($"{client.Client.RemoteEndPoint} connected");

        return true;
    }

    public bool SendInitialInfo(TcpClient client)
    {
        NetworkStream stream = client.GetStream();
        TransferData? transferData = new(FileList.ToArray());

        if(!SerializerHelper.Serialize(transferData, out byte[]? bytes))
        {
            Log.Error("Serializer the InitialInfo into bytes: FAILED");
            return false;
        }

        if(!Transmission.SendData(stream, bytes))
        {
            Log.Error("Send the InitialInfo bytes: FAILED");
            return false;
        }

        if(!Transmission.ReceiveData(stream, 4096, out bytes, out long size))
        {
            Log.Error($"Recv the Status bytes: FAILED");
            return false;
        }

        if(!SerializerHelper.Deserialize(bytes, out Status? status))
        {
            Log.Error("Deserialize the Status bytes into object: FAILED");
            return false;
        }

        if(status?.Code != StatusCode.I_GOT_IT)
        {
            Log.Error($"The obtained Status code: {status?.Code}");
            return false;
        }

        return true;
    }

    public bool SendFileList(TcpClient client)
    {
        foreach(FileData x in FileList)
            if(!Transmission.SendFile(client.GetStream(), x))
            {
                Log.Error($"Send \"{x.Name}\" ({StringHelper.BytesFormatIn(x.Size, "KB")}): FAILED");
                return false;
            }
            else
                Log.Succ($"Send \"{x.Name}\" ({StringHelper.BytesFormatIn(x.Size, "KB")}): SUCCESSFULLY");

        return true;
    }
}