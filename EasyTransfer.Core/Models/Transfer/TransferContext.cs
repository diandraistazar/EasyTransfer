using System.Collections.ObjectModel;

using EasyTransfer.Core.Models.Filesystem;

namespace EasyTransfer.Core.Models.Transfer;

public record class TransferContext(
    (Func<string> Get, Action<string> Set) Address,
    (Func<int> Get, Action<int> Set) Port,
    (Func<ObservableCollection<FileData>> Get, Action<ObservableCollection<FileData>> Set) FileList,
    (Func<DirectoryData> Get, Action<DirectoryData> Set) DestDirectory,
    (Func<int> Get, Action<int> Set) Progress
);