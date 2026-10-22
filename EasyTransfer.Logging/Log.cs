namespace EasyTransfer.Logging;

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
        Console.ForegroundColor = foregroundColor;
        
        Console.Write(message + end);
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
        ConsoleColor foregroundColorBak = Console.ForegroundColor;
        string logString = "";

        switch(level) {
            case LogLevel.Debug:
                logString = "DEBUG";
                Console.ForegroundColor = ConsoleColor.DarkGray;
                break;
            case LogLevel.Success:
                logString = "DONE";
                Console.ForegroundColor = ConsoleColor.Green;
                break;
            case LogLevel.Info:
                logString = "INFO";
                Console.ForegroundColor = ConsoleColor.White;
                break;
            case LogLevel.Warning:
                logString = "WARN";
                Console.ForegroundColor = ConsoleColor.Yellow;
                break;
            case LogLevel.Error:
                logString = "ERROR";
                Console.ForegroundColor = ConsoleColor.Red;
                break;
        }

        Console.Write($" {logString, 5}: {message}{end}");
        Console.ForegroundColor = foregroundColorBak;
    }
}