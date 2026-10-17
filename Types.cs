namespace EasyTransfer.Types;

public struct Header {
    public int index { get; set; }
    public string filename { get; set; }
    public long length { get; set; }
    public bool next { get; set; }
}

public struct Status {
    public string message { get; set; }
}