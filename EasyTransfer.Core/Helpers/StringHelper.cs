namespace EasyTransfer.Core.Helpers;

public static class StringHelper
{
    // private static char[] alphabet = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz".ToArray();
    private static readonly char[] numbers = "0123456789".ToCharArray();

    public static string? RemoveNonNumeric(string? s, char[]? except)
    {
        string temp = string.Empty;

        if(s == null)
            return temp;

        foreach(char c in s)
        {
            if(except.Contains(c) || numbers.Contains(c))
                temp = string.Concat(temp, c);
        }

        return temp;
    }

    public static string BytesFormatIn(long bytes, string format)
    {
        switch(format)
        {
            case "B":
                return $"{bytes} Bytes";
            case "KB":
                return $"{bytes / 1000} KB";
            case "MB":
                return $"{bytes / 1000000} MB";
            case "GB":
                return $"{bytes / 1000000000} GB";
        }

        return string.Empty;
    }
}
