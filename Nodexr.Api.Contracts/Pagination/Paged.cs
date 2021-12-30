namespace Nodexr.Api.Contracts.Pagination;

public class Paged<T>
{
    public Paged(List<T> contents, int totalSize, int start, int? limit)
    {
        Contents = contents;
        TotalSize = totalSize;
        Start = start;
        Limit = limit;
    }

    public List<T> Contents { get; }
    public int TotalSize { get; set; }
    public int Start { get; set; }
    public int? Limit { get; set; }
}

