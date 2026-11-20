using System.Net.Sockets;

using EasyTransfer.Core.Models.Transfer;
using EasyTransfer.Core.Logging;

namespace EasyTransfer.Core.Services;

public sealed class TransferManager : TransferModelBase
{
    private ServerNetwork Server { get; init; }
    private ClientNetwork Client { get; init; }

    public TransferManager()
    {
        Server = new ServerNetwork();
        Client = new ClientNetwork();
    }

    public bool Send(TransferContext context)
    {
        try
        {
            Context = context;
            Server.Context = Context;
            return SendInternal(); // it doesn't mean that return immediately exit from this method,
                                    // the return value stored first, then the finally block code is executed
                                    // and finally return with the stored return value
        }
        finally
        {
            Context = default;
            Server.Context = default;
        }
    }

    public bool Recv(TransferContext context)
    {
        try
        {
            Context = context;
            Client.Context = Context;
            return RecvInternal();
        }
        finally
        {
            Context = default;
            Client.Context = default;
        }
    }

    private bool SendInternal()
    {
        if(!Server.Setup(out TcpListener? server))
        {
            Log.Error("Setup the server: FAILED");
            return false;
        }

        Log.Succ("Setup the server: SUCCESSFULLY");

        if(!Server.Listening(server, out TcpClient? client))
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

        server.Stop();
        client.Close();

        Log.Debug("SendInternal is gonna return");
        return true;
    }

    private bool RecvInternal()
    {
        if(!Client.Setup(out TcpClient? client))
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
        
        client.Close();

        Log.Debug("RecvInternal is gonna return");
        return true;
    }
}