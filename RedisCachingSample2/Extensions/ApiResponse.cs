namespace RedisCachingSample2.Extensions
{
    public class ApiResponse<TEntity> where TEntity : class
    {
        public bool Result { get; set; }
        public TEntity Data { get; set; }
        public string ErrorMessage { get; set; }
    }
}
