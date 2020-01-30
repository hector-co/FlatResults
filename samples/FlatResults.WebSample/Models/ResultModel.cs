namespace FlatResults.WebSample.Models
{
    public class ResultModel<T>
    {
        public T Data { get; set; }
        public int TotalCount { get; set; }
    }
}
