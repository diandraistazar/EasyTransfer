using System.Net;

namespace EasyTransfer.Core.Helpers;

public static class NetworkHelper
{
    public static bool ParseIP(string? address, out IPAddress? buffer)
    {
        if(address == null || !IPAddress.TryParse(address, out buffer))
        {
            buffer = default;
            return false;
        }

        return true;
    }
}