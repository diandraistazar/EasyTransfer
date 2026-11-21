using System.Net.Sockets;

using EasyTransfer.Core.Models.Filesystem;
using EasyTransfer.Core.Logging;

namespace EasyTransfer.Core.Services;

internal sealed class TransmissionService
{
    public bool SendData(NetworkStream stream, byte[] bytes)
    {
        try
        {
            stream.Write(bytes, 0, bytes.Length);
        }
        catch
        {
            return false;
        }

        return true;
    }

    public bool ReceiveData(NetworkStream stream, long max, out byte[]? bytes, out long size)
    {
        bytes = default;
        size = default;

        List<byte> temp = new(0);

        while(!stream.DataAvailable)
            Thread.Sleep(300);

        try
        {
            byte[] buffer = new byte[512];
            long recv = 0;
           
            while (stream.DataAvailable && recv < max)
            {
                int readBytes = stream.Read(buffer, 0, buffer.Length);
                if (readBytes < 1)
                    break;

                temp.AddRange(buffer.Take(readBytes));
                recv += readBytes;
            }
        }
        catch
        {
            return false;
        }
        
        bytes = temp.ToArray();
        size = bytes.Length;
        return true;
    }

    public bool SendFile(NetworkStream stream, FileData file)
    {
        FileStream? fileStream = default;
        FileStreamOptions opts = new()
        {
            Access = FileAccess.Read,
            Mode = FileMode.Open,
            Options = 0
        };

        try
        {
            fileStream = new(file.FullPath, opts);

            SendFileInternal(stream, fileStream, file);
        }
        catch
        {
            return false;
        }
        finally
        {
            fileStream?.Dispose();
        }

        return true;
    }

    public bool ReceiveFile(NetworkStream stream, FileData fileData, out long recvBytes)
    {
        recvBytes = 0;

        long tempRecvBytes = 0;

        try
        {
            ReceiveFileInternal(stream, fileData, x => tempRecvBytes += x);
            return true;
        }
        catch
        {
            fileData.Delete();

            return false;
        }
        finally
        {
            recvBytes = tempRecvBytes;
        }
    }

    // ------------------------------------------------------ private

    private void SendFileInternal(NetworkStream stream, FileStream fileStream, FileData fileData)
    {
        byte[] buffer = new byte[Math.Min(fileData.Size, 512)];
        long send = 0;

        while(send < fileData.Size)
        {
            int bytes = fileStream.Read(buffer, 0, buffer.Length);
            if(bytes < 1)
                break;

            SendData(stream, buffer);
            send += bytes;
        }
    }

    private void ReceiveFileInternal(NetworkStream stream, FileData fileData, Action<long> recvBytesAdd)
    {
        long written = 0;

        while (written < fileData.Size)
        {
            bool rvalue = ReceiveData(stream, Math.Min(fileData.Size - written, 8192), out byte[]? buffer, out long size);

            if(rvalue)
                recvBytesAdd(size);
            else
                throw new Exception("ReceiveData throwed an exception i guess");
            
            File.AppendAllBytes(fileData.FullPath, buffer);
            written += size;
        }
    }
}