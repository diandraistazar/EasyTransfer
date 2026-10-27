namespace EasyTransfer.Logging;

using System;
using System.Text;

public enum LogLevel {
    Print,
    Debug,
    Info,
    Warning,
    Success,
    Error
}

public class Log {
    public static void Print(string message, char end = '\n', ConsoleColor foregroundColor = ConsoleColor.White) { 
        ConsoleColor previous = Console.ForegroundColor;
        StringBuilder entry = new();
        
        Console.ForegroundColor = foregroundColor;
        
        entry.Append(message);
        entry.Append(end);
        Console.Write(entry);

        Console.ForegroundColor = previous;
    }
    public static void Debug(string message, char end = '\n') { 
        Write(LogLevel.Debug, message, end);
    }

    public static void Info(string message, char end = '\n') { 
        Write(LogLevel.Info, message, end);
    }

    public static void Warn(string message, char end = '\n') { 
        Write(LogLevel.Warning, message, end);
    }

    public static void Succ(string message, char end = '\n') {
        Write(LogLevel.Success, message, end);
    }

    public static void Error(string message, char end = '\n') {
        Write(LogLevel.Error, message, end);
    }

    private static void Write(LogLevel level, string message, char end) {
        ConsoleColor previous = Console.ForegroundColor;
        StringBuilder entry = new(" ");

        switch(level) {
            case LogLevel.Debug:
                entry.Append("DEBUG");
                Console.ForegroundColor = ConsoleColor.DarkGray;
                break;
            case LogLevel.Info:
                entry.Append(" INFO");
                Console.ForegroundColor = ConsoleColor.White;
                break;
            case LogLevel.Success:
                entry.Append(" DONE");
                Console.ForegroundColor = ConsoleColor.Green;
                break;
            case LogLevel.Warning:
                entry.Append(" WARN");
                Console.ForegroundColor = ConsoleColor.Yellow;
                break;
            case LogLevel.Error:
                entry.Append("ERROR");
                Console.ForegroundColor = ConsoleColor.Red;
                break;
        }
        Console.Write(entry);
        
        entry.Clear();
        entry.Append(": ");
        entry.Append(message);
        entry.Append(end);

        Console.ForegroundColor = previous;
        Console.Write(entry);
    }
}