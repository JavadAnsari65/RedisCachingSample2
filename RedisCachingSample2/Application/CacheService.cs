using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Microsoft.Extensions.Caching.Distributed;
using RedisCachingSample2.Extensions;
using RedisCachingSample2.Infrastructure.Entities;
using StackExchange.Redis;
using System.Text.Json;

namespace RedisCachingSample2.Application
{
    public class CacheService:ICacheService
    {
        private readonly IConnectionMultiplexer _redis;
        private readonly IDatabase _database;
        public CacheService(IConnectionMultiplexer redis)
        {
            _redis = redis;
            _database = _redis.GetDatabase();
        }


        public async Task<ApiResponse<List<Post>>> GetAllPostAsync()
        {
            try
            {
                var value = _database.HashGetAll("posts");
                if (value.Length > 0)
                {
                    //Reset ExpireTime of 'post' key
                    _database.KeyExpire("posts", TimeSpan.FromHours(1));

                    var obj = Array.ConvertAll(value, val => JsonSerializer.Deserialize<Post>(val.Value)).ToList();

                    return await Task.FromResult(new ApiResponse<List<Post>>
                    {
                        Result = true,
                        Data = obj
                    });
                }
                else
                {
                    return await Task.FromResult(new ApiResponse<List<Post>>
                    {
                        Result = false,
                        ErrorMessage = "CacheMemory Doesn't have this request!"
                    });
                }
            }
            catch (Exception ex)
            {
                return await Task.FromResult(new ApiResponse<List<Post>>
                {
                    Result = false,
                    ErrorMessage = ex.Message
                });
            }
        }

        public async Task<ApiResponse<Post>> GetPostAsync(string key, int id)
        {
            try
            {
                var value = _database.HashGet(key, id);
                if (!value.IsNullOrEmpty)
                {
                    //Reset ExpireTime of 'post' key
                    _database.KeyExpire("posts", TimeSpan.FromHours(1));

                    var obj = JsonSerializer.Deserialize<Post>(value);

                    return await Task.FromResult(new ApiResponse<Post>
                    {
                        Result = true,
                        Data = obj
                    });
                }
                else
                {
                    return await Task.FromResult(new ApiResponse<Post>
                    {
                        Result = false,
                        ErrorMessage = "CacheMemory Doesn't have this request!"
                    });
                }
            }
            catch (Exception ex)
            {
                return await Task.FromResult(new ApiResponse<Post>
                {
                    Result = false,
                    ErrorMessage = ex.Message
                });
            }
        }

        public bool RemovePost(string key, int id)
        {
            bool isDeleted = _database.HashDelete(key, id);
            return isDeleted;
        }

        public async Task<ApiResponse<Post>> SetPost(string key, Post post)
        {
            try
            {
                //var expirationTime = DateTimeOffset.Now.AddMinutes(60);
                //var expiration = expirationTime.DateTime.Subtract(DateTime.Now);

                var expiration = TimeSpan.FromHours(1);
                var serializedObject = JsonSerializer.Serialize(post);

                _database.HashSet(key, new HashEntry[]{
                    new HashEntry(post.Id, serializedObject)
                });
                _database.KeyExpire(key, expiration);

                return await Task.FromResult(new ApiResponse<Post>
                {
                    Result = true,
                    Data = post
                });
            }
            catch (Exception ex)
            {
                return await Task.FromResult(new ApiResponse<Post>
                {
                    Result = false,
                    ErrorMessage = ex.Message
                });
            }
        }

    }
}
