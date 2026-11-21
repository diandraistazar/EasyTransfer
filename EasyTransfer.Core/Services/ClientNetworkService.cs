using System.Net.Sockets;

using EasyTransfer.Core.Models.Filesystem;
using EasyTransfer.Core.Models.Transfer;
using EasyTransfer.Core.Helpers;
using EasyTransfer.Core.Logging;
using System.Text;

namespace EasyTransfer.Core.Services;

internal sealed class ClientNetworkService : TransferModelBase
{ 
    private TransmissionService Transmission { get; init; }

    public ClientNetworkService()
    {   
        Transmission = new TransmissionService();
    }

    public bool Setup(out TcpClient? client)
    {
        client = default;
        
        try
        {
            client = new TcpClient(Address, Port);
        }
        catch
        {
            return false;
        }

        string local = client?.Client.LocalEndPoint.ToString() ?? "UNKNOWN";
        string remote = client?.Client.RemoteEndPoint.ToString() ?? "UNKNOWN";

        Log.Info($"This machine {local} connected to {remote}");

        return true;
    }

    public bool RecvInitialInfo(TcpClient client, out TransferData? info)
    {
        info = default;

        NetworkStream stream = client.GetStream();

        if(!Transmission.ReceiveData(stream, 4096, out byte[]? bytes, out long size))
        {
            Log.Error($"Received the InitialInfo bytes from the server: FAILED");
            return false;
        }

        if(!SerializerHelper.Deserialize(bytes, out info))
        {
            Log.Error("Deserialize the InitialInfo bytes into object: FAILED");
            Log.Debug($"{Encoding.Default.GetString(bytes)}");
            return false;
        }

        if(!SerializerHelper.Serialize(new Status(StatusCode.I_GOT_IT), out bytes))
        {
            Log.Error("Serialize the Status into bytes: FAILED");
            return false;
        }

        if(!Transmission.SendData(stream, bytes))
        {
            Log.Error("Send the Status bytes: FAILED");
            return false;
        }

        return true;
    }

    public bool RecvFileList(TcpClient client, TransferData transferData)
    {
        foreach(SimpleFileData data in transferData.FileList)
        {
            FileData fileData = new(
                data.Name, Path.Combine(DestDirectory.FullPath, data.Name),
                DestDirectory.FullPath, data.Size
            );

            if(!Transmission.ReceiveFile(client.GetStream(), fileData, out long recvBytes))
            {
                Log.Error($"Received \"{fileData.Name}\" ({StringHelper.BytesFormatIn(recvBytes, "KB")}): FAILED");
                return false;
            }
            
            FileList.Add(fileData);
            Log.Succ($"Received \"{fileData.Name}\" ({StringHelper.BytesFormatIn(recvBytes, "KB")}): SUCCESSFULLY");
        }

        return true;
    }
}