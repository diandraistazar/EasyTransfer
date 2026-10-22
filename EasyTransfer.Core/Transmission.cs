namespace EasyTransfer.Core;

using System.Net.Sockets;
using System.Threading;

public class Transmission {
    public static long GetAvailableData(Socket socket, long max, int attempts, int delay) {
        long available = 0;

        while(available < max && attempts > 0) {
            Thread.Sleep(delay);
            
            if(socket.Available > available) {
                available = socket.Available;
            } else {
                attempts--;
            }
        }

        return available;
    }

    public static void SendData(NetworkStream stream, int offset, int length, byte[] bytes) {
        stream.Write(bytes, offset, length);
    }

    public static int ReceiveData(NetworkStream stream, int offset, int length, byte[] bytes) {
        int recv = 0;

        while(recv < length) {
            int readBytes = stream.Read(bytes, offset + recv, length - recv);
            if(readBytes < 1) {
                break;
            }

            recv += readBytes;
        }    

        return recv;
    }
}