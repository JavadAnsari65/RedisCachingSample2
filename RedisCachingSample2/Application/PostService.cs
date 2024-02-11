using RedisCachingSample2.Extensions;
using RedisCachingSample2.Infrastructure.Configuration;
using RedisCachingSample2.Infrastructure.Entities;
using RedisCachingSample2.Infrastructure.Repository;

namespace RedisCachingSample2.Application
{
    public class PostService : IPostService
    {
        private readonly CRUDRepo<Post> _crudRepo;
        private readonly ICacheService _cacheService;
        private static object _lock = new Object();
        public PostService(CRUDRepo<Post> crudRepo, ICacheService cacheService)
        {
            _crudRepo = crudRepo;
            _cacheService = cacheService;

        }

        public async Task<ApiResponse<List<Post>>> GetAllPostAsync()
        {
            try
            {
                var getResult = _crudRepo.GetAll();

                //Add Data to Cache
                lock (_lock)
                {
                    foreach(var post in getResult.Result.Data)
                    {
                        _cacheService.SetPost("posts", post);
                    }
                }

                return await Task.FromResult(new ApiResponse<List<Post>>
                {
                    Result = getResult.Result.Result,
                    Data = getResult.Result.Data,
                    ErrorMessage = getResult.Result.ErrorMessage
                });
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

        public async Task<ApiResponse<Post>> GetPostAsync(int postId)
        {
            try
            {
                var getResult = _crudRepo.Get(postId);

                return await Task.FromResult(new ApiResponse<Post>
                {
                    Result = getResult.Result.Result,
                    Data = getResult.Result.Data,
                    ErrorMessage = getResult.Result.ErrorMessage
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

        public async Task<ApiResponse<Post>> AddPostAsync(Post newPost)
        {
            try
            {
                if(newPost is not null)
                {
                    var addResult = _crudRepo.AddAsync(newPost);

                    if (addResult.Result.Result)
                    {
                        // Set new to the redis instance
                        lock (_lock)
                        {
                            _cacheService.SetPost("posts", addResult.Result.Data);
                        }

                        return await Task.FromResult(new ApiResponse<Post>
                        {
                            Result = addResult.Result.Result,
                            Data = addResult.Result.Data
                        });
                    }
                    else
                    {
                        return await Task.FromResult(new ApiResponse<Post>
                        {
                            Result = addResult.Result.Result,
                            ErrorMessage = addResult.Result.ErrorMessage
                        });
                    }
                }
                else
                {
                    return await Task.FromResult(new ApiResponse<Post>
                    {
                        Result = false,
                        ErrorMessage = "The NewItem is null!!!"
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

        public async Task<ApiResponse<Post>> UpdatePostAsync(int postId, Post updatedPost)
        {
            try
            {
                if (updatedPost.Id == postId && updatedPost is not null)
                {
                    var updateResult = _crudRepo.UpdateAsync(updatedPost);

                    if (updateResult.Result.Result)
                    {
                        // Set new to the redis instance
                        lock (_lock)
                        {
                            // Remove from Redis
                            var getPost = _cacheService.GetPostAsync("posts", updatedPost.Id);
                            if (getPost.Result.Result)
                            {
                                var removeResult = _cacheService.RemovePost("posts", getPost.Result.Data.Id);
                            }
                            _cacheService.SetPost("posts", updateResult.Result.Data);
                        }

                        return await Task.FromResult(new ApiResponse<Post>
                        {
                            Result = updateResult.Result.Result,
                            Data = updateResult.Result.Data
                        });
                    }
                    else
                    {
                        return await Task.FromResult(new ApiResponse<Post>
                        {
                            Result = false,
                            ErrorMessage = updateResult.Result.ErrorMessage,
                        });
                    }
                }
                else
                {
                    return await Task.FromResult(new ApiResponse<Post>
                    {
                        Result = false,
                        ErrorMessage = "The UserId field in not equal with UserId in UpdatedPost Or UpdateUser is null!",
                    });
                }
            }
            catch (Exception ex)
            {
                return await Task.FromResult(new ApiResponse<Post>
                {
                    Result = false,
                    ErrorMessage = ex.Message,
                });
            }
        }

        public async Task<ApiResponse<Post>> DeletePostAsync(int postId)
        {
            try
            {
                //حذف از حافظه کش
                var getPost = _cacheService.GetPostAsync("posts", postId);
                if (getPost is not null)
                {
                    var removeResult = _cacheService.RemovePost("posts", getPost.Id);
                }

                var delResult = _crudRepo.Delete(postId);

                if (delResult.Result.Result)
                {
                    return await Task.FromResult(new ApiResponse<Post>
                    {
                        Result = delResult.Result.Result,
                        Data = delResult.Result.Data
                    });
                }
                else
                {
                    return await Task.FromResult(new ApiResponse<Post>
                    {
                        Result = delResult.Result.Result,
                        ErrorMessage = delResult.Result.ErrorMessage
                    });
                }
            }
            catch (Exception ex)
            {
                return await Task.FromResult(new ApiResponse<Post>
                {
                    Result = false,
                    ErrorMessage = ex.Message,
                });
            }
        }
    }
}
