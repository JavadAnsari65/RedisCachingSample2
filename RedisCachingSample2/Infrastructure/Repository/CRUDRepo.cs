using Microsoft.EntityFrameworkCore;
using RedisCachingSample2.Extensions;
using RedisCachingSample2.Infrastructure.Configuration;
using RedisCachingSample2.Infrastructure.Entities;

namespace RedisCachingSample2.Infrastructure.Repository
{
    public class CRUDRepo<TEntity> where TEntity : class
    {
        private readonly AppDbContext _dbContext;
        public CRUDRepo(AppDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<ApiResponse<List<TEntity>>> GetAll()
        {
            try
            {
                var getResult = _dbContext.Set<TEntity>().ToList();

                if (getResult.Count > 0)
                {
                    return await Task.FromResult(new ApiResponse<List<TEntity>>
                    {
                        Result = true,
                        Data = getResult
                    });
                }
                else
                {
                    return await Task.FromResult(new ApiResponse<List<TEntity>>
                    {
                        Result = false,
                        ErrorMessage = "There are no records!"
                    });
                }
            }
            catch (Exception ex)
            {
                return await Task.FromResult(new ApiResponse<List<TEntity>>
                {
                    Result = false,
                    ErrorMessage = ex.Message
                });
            }
        }

        public async Task<ApiResponse<TEntity>> Get(int itemId)
        {
            try
            {
                var getResult = _dbContext.Set<TEntity>().Find(itemId);

                if(getResult is not null)
                {
                    return await Task.FromResult(new ApiResponse<TEntity>
                    {
                        Result = true,
                        Data = getResult
                    });
                }
                else
                {
                    return await Task.FromResult(new ApiResponse<TEntity>
                    {
                        Result = true,
                        ErrorMessage = "The Post with the PostId is not found!"
                    });
                }
            }
            catch (Exception ex)
            {
                return await Task.FromResult(new ApiResponse<TEntity>
                {
                    Result = false,
                    ErrorMessage = ex.Message
                });
            }
        }

        public async Task<ApiResponse<TEntity>> AddAsync(TEntity newItem)
        {
            try
            {
                await _dbContext.Set<TEntity>().AddAsync(newItem);
                await _dbContext.SaveChangesAsync();

                return await Task.FromResult(new ApiResponse<TEntity>
                {
                    Result = true,
                    Data = newItem
                });
            }
            catch (Exception ex)
            {
                return await Task.FromResult(new ApiResponse<TEntity>
                {
                    Result = false,
                    ErrorMessage = ex.Message
                });
            }
        }

        public async Task<ApiResponse<TEntity>> UpdateAsync(TEntity updatedItem)
        {
            try
            {
                _dbContext.Entry(updatedItem).State = EntityState.Modified;
                await _dbContext.SaveChangesAsync();

                return await Task.FromResult(new ApiResponse<TEntity>
                {
                    Result = true,
                    Data = updatedItem
                });
            }
            catch (Exception ex)
            {
                return await Task.FromResult(new ApiResponse<TEntity>
                {
                    Result = false,
                    ErrorMessage = ex.Message
                });
            }
        }

        public async Task<ApiResponse<TEntity>> Delete(int itemId)
        {
            try
            {
                var delItem = await _dbContext.Set<TEntity>().FindAsync(itemId);
                
                if (delItem != null)
                {
                    _dbContext.Set<TEntity>().Remove(delItem);
                    await _dbContext.SaveChangesAsync();

                    return await Task.FromResult(new ApiResponse<TEntity>
                    {
                        Result = true,
                        Data = delItem
                    });
                }
                else
                {
                    return await Task.FromResult(new ApiResponse<TEntity>
                    {
                        Result = false,
                        ErrorMessage = "The Post with the postId is not found!"
                    });
                }
            }
            catch (Exception ex)
            {
                return await Task.FromResult(new ApiResponse<TEntity>
                {
                    Result = false,
                    ErrorMessage = ex.Message
                });
            }
        }
    }
}
