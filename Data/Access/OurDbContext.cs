using LANMovie.Common;
using LANMovie.Data.Entities;
using Microsoft.EntityFrameworkCore;

namespace LANMovie.Data.Access
{
    public class OurDbContext : DbContext
    {
        public DbSet<MovieEntity>? Movies { get; set; }


        /// <summary>
        /// 连接数据库
        /// </summary>
        /// <param name="optionsBuilder"></param>
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            var connStr = GlobalValues.AppConfig?.Data.Database.ConnStr;
            if (!string.IsNullOrEmpty(connStr))
            {
                optionsBuilder.UseSqlite(connStr);
            }
        }
    }


    public class SqlMovieData
    {
        private OurDbContext _context { get; set; }

        public SqlMovieData(OurDbContext context)
        {
            _context = context;
        }


        /// <summary>
        /// 获取数据库中的全部电影
        /// </summary>
        /// <returns>电影列表</returns>
        public IEnumerable<MovieEntity> Get()
        {
            if((_context != null) && (_context.Movies != null))
            {
                return _context.Movies;
            }
            return Array.Empty<MovieEntity>();
        }


        /// <summary>
        /// 获取数据库中的全部电影
        /// </summary>
        /// <returns>电影列表</returns>
        public async Task<IEnumerable<MovieEntity>> GetAsync()
        {
            if ((_context != null) && (_context.Movies != null))
            {
                return await _context.Movies.ToArrayAsync();
            }
            return Array.Empty<MovieEntity>();
        }


        /// <summary>
        /// 根据ID查找电影
        /// </summary>
        /// <param name="id">电影ID</param>
        /// <returns>查找到的电影对象, 若不存在则返回null</returns>
        public MovieEntity? Get(string id)
        {
            if ((_context != null) && (_context.Movies != null))
            {
                return _context.Movies.Where(m => m.Id == id).FirstOrDefault();
            }
            return null;
        }


        /// <summary>
        /// 根据ID查找电影
        /// </summary>
        /// <param name="id">电影ID</param>
        /// <returns>查找到的电影对象, 若不存在则返回null</returns>
        public async Task<MovieEntity?> GetAsync(string id)
        {
            if ((_context != null) && (_context.Movies != null))
            {
                return await _context.Movies.Where(_m => _m.Id == id).FirstOrDefaultAsync();
            }
            return null;
        }


        /// <summary>
        /// 添加电影
        /// </summary>
        /// <param name="movie">待添加的电影</param>
        /// <returns>添加成功返回true, 否则返回false</returns>
        public bool Add(MovieEntity movie)
        {
            if ((_context != null) && (_context.Movies != null))
            {
                _context.Movies.Add(movie);
                if(_context.SaveChanges() > 0)
                {
                    return true;
                }   
            }
            return false;
        }


        /// <summary>
        /// 添加电影
        /// </summary>
        /// <param name="movie">待添加的电影</param>
        /// <returns>添加成功返回true, 否则返回false</returns>
        public async Task<bool> AddAsync(MovieEntity movie)
        {
            if ((_context != null) && (_context.Movies != null))
            {
                await _context.Movies.AddAsync(movie);
                if(await _context.SaveChangesAsync() > 0)
                {
                    return true;
                }  
            }
            return false;
        }


        /// <summary>
        /// 删除指定ID的电影
        /// </summary>
        /// <param name="id">电影ID</param>
        /// <returns>删除成功true, 否则返回false</returns>
        public bool Remove(string id)
        {
            if ((_context != null) && (_context.Movies != null))
            {
                var movie = Get(id);
                if(movie != null)
                {
                    _context.Movies.Remove(movie);
                    if(_context.SaveChanges() > 0)
                    {
                        return true;
                    }
                    return false;
                }
                return true;
            }
            return false;
        }


        /// <summary>
        /// 删除指定ID的电影
        /// </summary>
        /// <param name="id">电影ID</param>
        /// <returns>删除成功true, 否则返回false</returns>
        public async Task<bool> RemoveAsync(string id)
        {
            if ((_context != null) && (_context.Movies != null))
            {
                var movie = Get(id);
                if (movie != null)
                {
                    _context.Movies.Remove(movie);
                    if(await _context.SaveChangesAsync() > 0)
                    {
                        return true;
                    }
                    return false;
                }
                return true;
            }
            return false;
        }


        /// <summary>
        /// 删除指定电影
        /// </summary>
        /// <param name="movie">待移除的电影</param>
        /// <returns></returns>
        public bool Remove(MovieEntity movie)
        {
            if ((_context != null) && (_context.Movies != null))
            {
                _context.Movies.Remove(movie);
                _context.SaveChanges();
                return true;
            }
            return false;
        }


        /// <summary>
        /// 删除指定电影
        /// </summary>
        /// <param name="movie">待移除的电影</param>
        /// <returns></returns>
        public async Task<bool> RemoveAsync(MovieEntity movie)
        {
            if ((_context != null) && (_context.Movies != null))
            {
                _context.Movies.Remove(movie);
                await _context.SaveChangesAsync();
                return true;
            }
            return false;
        }


        /// <summary>
        /// 更新电影数据
        /// </summary>
        /// <param name="movie">待更新的电影对象</param>
        /// <returns>更新成功返回true, 否则返回false</returns>
        public bool Update(MovieEntity movie)
        {
            if ((_context != null) && (_context.Movies != null))
            {
                _context.Movies.Update(movie);
                if(_context.SaveChanges() > 0)
                {
                    return true;
                }
            }
            return false;
        }


        /// <summary>
        /// 更新电影数据
        /// </summary>
        /// <param name="movie">待更新的电影对象</param>
        /// <returns>更新成功返回true, 否则返回false</returns>
        public async Task<bool> UpdateAsync(MovieEntity movie)
        {
            if ((_context != null) && (_context.Movies != null))
            {
                _context.Movies.Update(movie);
                if(await _context.SaveChangesAsync() > 0)
                {
                    return true;
                }
            }
            return false;
        }
    }
}
