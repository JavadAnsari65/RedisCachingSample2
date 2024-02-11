using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using RedisCachingSample2.Application;
using RedisCachingSample2.Extensions;
using RedisCachingSample2.Infrastructure.Entities;

namespace RedisCachingSample2.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class RedisCachingController : ControllerBase
    {
        private readonly IPostService _postService;
        private readonly ICacheService _cacheService;
        public RedisCachingController(IPostService postService, ICacheService cacheService)
        {
            _postService = postService;  
            _cacheService = cacheService;
        }

        [HttpGet]
        [Route("GetAllPost")]
        public async Task<ApiResponse<List<Post>>> GetAll()
        {
            try
            {
                //اول کش را بررسی می کند
                var cacheResult = _cacheService.GetAllPostAsync();
                if (cacheResult.Result.Result)
                {
                    return await Task.FromResult(new ApiResponse<List<Post>>
                    {
                        Result = cacheResult.Result.Result,
                        Data = cacheResult.Result.Data,
                        ErrorMessage = cacheResult.Result.ErrorMessage + "Of Cache"
                    });
                }
                else
                {
                    //اگر در کش وجود نداشت
                    var getResult = _postService.GetAllPostAsync();

                    return await Task.FromResult(new ApiResponse<List<Post>>
                    {
                        Result = getResult.Result.Result,
                        Data = getResult.Result.Data,
                        ErrorMessage = getResult.Result.ErrorMessage + "Of DB"
                    });
                }
            }
            catch (Exception ex)
            {
                return await Task.FromResult(new ApiResponse<List<Post>>
                {
                    Result = false,
                    ErrorMessage = ex.Message,
                });
            }
        }

        [HttpGet]
        [Route("GetPostById")]
        public async Task<ApiResponse<Post>> Get(int postId)
        {
            try
            {
                //اول حافظه کش بررسی می شود
                var cacheResult = _cacheService.GetPostAsync("posts", postId);

                if (cacheResult.Result.Result)
                {
                    return await Task.FromResult(new ApiResponse<Post>
                    {
                        Result = cacheResult.Result.Result,
                        Data = cacheResult.Result.Data,
                        ErrorMessage = cacheResult.Result.ErrorMessage + "Of Cache"
                    });
                }

                var getResult = _postService.GetPostAsync(postId);

                return await Task.FromResult(new ApiResponse<Post>
                {
                    Result = getResult.Result.Result,
                    Data = getResult.Result.Data,
                    ErrorMessage = getResult.Result.ErrorMessage + "Of DB"
                });
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

        [HttpPost]
        [Route("AddPost")]
        public async Task<ApiResponse<Post>> Add(Post newPost)
        {
            try
            {
                var addResult = _postService.AddPostAsync(newPost);

                if (addResult.Result.Result)
                {
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
            catch (Exception ex)
            {
                return await Task.FromResult(new ApiResponse<Post>
                {
                    Result = false,
                    ErrorMessage = ex.Message,
                });
            }
        }

        [HttpPut]
        [Route("UpdatePost")]
        public async Task<ApiResponse<Post>> Update(int postId, Post updatedPost)
        {
            try
            {
                var updateResult = _postService.UpdatePostAsync(postId, updatedPost);

                if (updateResult.Result.Result)
                {
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
            catch (Exception ex)
            {
                return await Task.FromResult(new ApiResponse<Post>
                {
                    Result = false,
                    ErrorMessage = ex.Message,
                });
            }
        }

        [HttpDelete]
        [Route("DeletePost")]
        public async Task<ApiResponse<Post>> Delete(int postId)
        {
            try
            {
                var delResult = _postService.DeletePostAsync(postId);

                return await Task.FromResult(new ApiResponse<Post>
                {
                    Result = delResult.Result.Result,
                    Data = delResult.Result.Data,
                    ErrorMessage = delResult.Result.ErrorMessage
                });
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
