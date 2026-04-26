namespace AihrlyApi.DTOs
{
public class PagedResult<T>
{
    public List<T> items { get; set; }
    public int totalCount { get; set; }
}
}