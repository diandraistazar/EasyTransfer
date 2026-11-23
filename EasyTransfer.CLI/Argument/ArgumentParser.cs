using System.Data;

using EasyTransfer.Core.Models.Filesystem;
using EasyTransfer.Core.Models;

namespace EasyTransfer.CLI.Argument;

public static class ArgumentParser
{
    public static void Parsing(string[] argv, DataContext context)
    {
        if(argv.Length < 3)
            throw new ArgumentException($"Program needs at least 3 arguments, {argv.Length} passed");

        context.Command = argv[0];
        if(context.Command != "send" && context.Command != "recv")
            throw new ArgumentException($"Command is only either send or recv, {context.Command} passed");

        string[] address = argv[1].Split(':');
        if(address.Length < 2)
            throw new ArgumentException($"Address must be ip:port (e.g 192.168.1.1:8080), {argv[1]} passed");

        context.Address = address[0];
        if(!int.TryParse(address[1], out int port))
            throw new ArgumentException($"The specified port is invalid");

        context.Port = port;

        if(context.Command == "send")
        {
            for(int i = 2; i < argv.Length; i++)
            {
                FileInfo file = new(argv[i]);

                if(!file.Exists)
                    throw new FileNotFoundException($"The specified file \"{argv[i]}\" doesn't exists");

                context.FileList?.Add(new FileData(
                    file.Name, file.FullName, file.Directory?.FullName, file.Length
                ));
            }
        }
        else
        {
            DirectoryInfo dir = new(argv[2]);
            
            if(!dir.Exists)
               throw new DirectoryNotFoundException($"The specified directory \"{argv[2]}\" doesn't exists");

            context.DestDirectory = new DirectoryData(dir.Name, dir.FullName, dir.Parent?.FullName);
        }  
    }
}