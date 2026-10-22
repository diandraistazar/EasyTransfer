namespace EasyTransfer;

using EasyTransfer.Logging;
using EasyTransfer.UI;

class Program {
    public static void Main(string[] args) {
        ValueTuple<int, int> cursorPos = Console.GetCursorPosition();

        if((cursorPos.Item1 | cursorPos.Item2) != 0) {
            Log.Info("CLI session running");
            CLI.Run(args);
        } else {
            Log.Info("GUI session running");
            GUI.Run();
        }
    }
}