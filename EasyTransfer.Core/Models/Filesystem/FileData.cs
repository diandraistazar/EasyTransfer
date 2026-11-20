namespace EasyTransfer.Core.Models.Filesystem;

public record class SimpleFileData(string Name, long Size);

public record class FileData(string Name, string FullPath, string DirParent, long Size)
{
    private FileInfo Info { get; init; } = new(FullPath);

    public bool Delete()
    {
        try
        {
            if(Info.Exists)
                Info.Delete();   
        }
        catch
        {
            return false;
        }
        

        return true;
    }
}