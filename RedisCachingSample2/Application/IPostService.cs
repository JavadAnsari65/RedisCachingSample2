using RedisCachingSample2.Extensions;
using RedisCachingSample2.Infrastructure.Entities;

namespace RedisCachingSample2.Application
{
    public interface IPostService
    {
        public Task<ApiResponse<Post>> AddPostAsync(Post newPost);
        public Task<ApiResponse<Post>> UpdatePostAsync(int postId, Post updatedPost);

        public Task<ApiResponse<Post>> DeletePostAsync(int postId);

        public Task<ApiResponse<Post>> GetPostAsync(int postId);

        public Task<ApiResponse<List<Post>>> GetAllPostAsync();

    }
}
