using System.ComponentModel.DataAnnotations;

namespace RedisCachingSample2.Infrastructure.Entities
{
    public class Post
    {
        [Key]
        public int Id { get; set; }
        public string Title { get; set; }
        public string Username { get; set; }
        public DateTime CreateAt { get; set; }
    }
}
