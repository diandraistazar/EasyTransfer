using System.Net.Sockets;

using EasyTransfer.Core.Models.Transfer;
using EasyTransfer.Core.Models;
using EasyTransfer.Core.Logging;

namespace EasyTransfer.Core.Services;

public sealed class TransferManagerService
{
    private DataContext Context { get; init; }
    private ServerNetworkService Server { get; init; }
    private ClientNetworkService Client { get; init; }

    public TransferManagerService(DataContext context)
    {
        Context = context;

        Server = new ServerNetworkService(Context);
        Client = new ClientNetworkService(Context);
    }

    public bool Send()
    {
        TcpListener? server = default;
        TcpClient? client = default;

        try
        {
            if(!Server.Setup(out server))
            {
                Log.Error("Setup the server: FAILED");
                return false;
            }

            Log.Succ("Setup the server: SUCCESSFULLY");

            if(!Server.Listening(server, out client))
            {
                Log.Error("Listen the socket: FAILED");
                return false;
            }

            if(!Server.SendInitialInfo(client))
            {
                Log.Error($"Send the InitialInfo to the connected client: FAILED");
                return false;
            }

            Log.Succ("Send the InitialInfo to the connected client: SUCCESSFULLY");

            if(!Server.SendFileList(client))
            {
                Log.Error("Send the list of files: FAILED");
                return false;
            }

            Log.Succ("Send the list of files: SUCCESSFULLY");
            return true;
        }
        finally
        {
            client?.Close();
            server?.Stop();
        }
    }

    public bool Recv()
    {
        TcpClient? client = default;

        try
        {
            if(!Client.Setup(out client))
            {
                Log.Error("Setup the client: FAILED");
                return false;
            }

            Log.Succ("Setup the client: SUCCESSFULLY");

            if(!Client.RecvInitialInfo(client, out TransferData? transferData))
            {
                Log.Error("Recv the InitialInfo from the server: FAILED");
                return false;
            }
        
            Log.Succ("Recv the InitialInfo from the server: SUCCESSFULLY");

            if(!Client.RecvFileList(client, transferData))
            {
                Log.Error("Recv the list of files from the server: FAILED");
                return false;
            }
        
            Log.Succ("Recv the list of files from the server: SUCCESSFULLY");
            return true;
        }
        finally
        {
            client?.Close();
        }
    }
}