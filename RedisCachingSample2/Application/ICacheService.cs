using RedisCachingSample2.Extensions;
using RedisCachingSample2.Infrastructure.Entities;

namespace RedisCachingSample2.Application
{
    public interface ICacheService
    {
        public Task<ApiResponse<List<Post>>> GetAllPostAsync();

        public Task<ApiResponse<Post>> GetPostAsync(string key, int id);

        public bool RemovePost(string key, int id);

        public Task<ApiResponse<Post>> SetPost(string key, Post post);

    }
}
