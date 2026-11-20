using EasyTransfer.Core.Models.Filesystem;

namespace EasyTransfer.Core.Models.Transfer;

public enum StatusCode { I_GOT_IT };

public record class Status(StatusCode Code);

public record class TransferData
{
    public TransferData() {} // parameter-less constructor for serialization

    public TransferData(FileData[] FileList)
    {
        this.FileList = new SimpleFileData[FileList.Length];
        TotalSize = 0;

        for(int i = 0; i < FileList.Length; i++)
        {
            FileData x = FileList[i];
            SimpleFileData y = new(x.Name, x.Size);

            this.FileList[i] = y;
            TotalSize += y.Size;
        }

        Count = this.FileList.Length;
    }

    public SimpleFileData[] FileList { get; init; }
    public long TotalSize { get; init; }
    public int Count { get; init; }
}