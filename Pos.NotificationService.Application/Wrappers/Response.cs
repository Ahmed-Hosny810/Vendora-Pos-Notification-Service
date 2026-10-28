namespace Pos.NotificationService.Application.Wrappers;
public class Response<T>
{
    public bool Succeeded { get; set; }
    public string? Message { get; set; }
    public List<string> Errors { get; set; } = new();
    public T? Data { get; set; }
    public Response() { }
    public Response(T data, string? message = null) { Succeeded = true; Data = data; Message = message; }
}
public class PagedResponse<T> : Response<T>
{
    public int PageNumber { get; set; }
    public int PageSize { get; set; }
    public int TotalCount { get; set; }
    public PagedResponse(T data, int pageNumber, int pageSize, int totalCount) : base(data)
    { PageNumber = pageNumber; PageSize = pageSize; TotalCount = totalCount; }
}
