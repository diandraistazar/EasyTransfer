using Avalonia.Platform.Storage;

using EasyTransfer.Core.Models.Filesystem;
using EasyTransfer.Core.Logging;

namespace EasyTransfer.Core.Services;

public sealed class FilePickerService
{
    private IStorageProvider StorageProvider { get; init; }

    public FilePickerService(IStorageProvider storageProvider)
    {
        StorageProvider = storageProvider;
    }

    public async Task<FileData[]?> PickFilesAsync()
    {
        if(!StorageProvider.CanOpen)
        {
            Log.Warn("Cannot open a file manager for picking files");
            return default;
        }

        var itemList = await StorageProvider.OpenFilePickerAsync(new FilePickerOpenOptions()
        {
            Title = "Pick Files",
            AllowMultiple = true
        });

        if(itemList.Count < 1)
        {
            Log.Warn("No files selected");
            return default;
        }

        List<FileData> list = new();

        foreach(var x in itemList)
            list.Add(new FileData(
               x.Name, x.Path.LocalPath,
               new Uri(x.Path, ".").LocalPath,
               new FileInfo(x.Path.LocalPath).Length
            ));

        return list.ToArray();
    }

    public async Task<DirectoryData?> PickDirectoryAsync()
    {
        if(!StorageProvider.CanPickFolder)
        {
            Log.Warn("Cannot open a file manager for picking a directory");
            return default;
        }

        var directory = await StorageProvider.OpenFolderPickerAsync(new FolderPickerOpenOptions()
        {
            Title = "Pick a Directory",
            AllowMultiple = false
        });

        if(directory.Count < 1)
        {
            Log.Warn("No directory selected");
            return default;
        }
        
        var dir = directory[0];

        return new DirectoryData(dir.Name, dir.Path.LocalPath, new Uri(dir.Path, "..").LocalPath);
    }
}