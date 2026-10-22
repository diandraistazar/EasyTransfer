namespace EasyTransfer.API;

using EasyTransfer.Core;
using System;
using System.IO;
using System.Net;
using System.Net.Sockets;

public class Network {
    public static TcpListener SetupServer(IPAddress IP, int port) {
        TcpListener serverTcp = new (IP, port);
        serverTcp.Start();

        return serverTcp;
    }

    public static TcpClient SetupClient(IPAddress destIP, int destPort) {
        TcpClient clientTcp = new (destIP.ToString(), destPort);

        return clientTcp;
    }

    public static void SendData(TcpClient client, byte[] bytes) {
        NetworkStream stream = client.GetStream();
        
        Transmission.SendData(stream, 0, bytes.Length, bytes);
    }

    public static void SendFile(TcpClient client, string filePath) {
        NetworkStream stream = client.GetStream();
        FileInfo file = new (filePath);
        FileStream fileStream = new (filePath, new FileStreamOptions() {
            Access = FileAccess.Read,
            Mode = FileMode.Open,
            Options = 0
        });
        byte[] buffer = new byte[Math.Min(file.Length, 512)];
        int send = 0;

        while(send < file.Length) {
            int bytes = fileStream.Read(buffer, 0, buffer.Length);

            Transmission.SendData(stream, 0, bytes, buffer);
            send += bytes;
        }
    }

    public static int ReceiveData(TcpClient client, int max, out byte[] bytes) {
        while(client.Available < 1) {
            Thread.Sleep(300);
        }

        bytes = new byte[Transmission.GetAvailableData(client.Client, max, 3, 300)];
        return Transmission.ReceiveData(client.GetStream(), 0, bytes.Length, bytes);
    }

    public static int ReceiveFile(TcpClient client, int fileLength, string destPath) {
        NetworkStream stream = client.GetStream();
        byte[] buffer = new byte[Math.Min(fileLength, 512)];
        int written = 0;

        while(written < fileLength) {
            int bytes = Transmission.ReceiveData(stream, 0, Math.Min(buffer.Length, fileLength - written), buffer);
            if(bytes > 0) {
                File.AppendAllBytes(destPath, buffer.Take(bytes).ToArray());
                written += bytes;
            } else {
                break;
            }
        }

        return written;
    }
}