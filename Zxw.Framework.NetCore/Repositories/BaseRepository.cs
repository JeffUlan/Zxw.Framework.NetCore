﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore;
using Zxw.Framework.NetCore.EfDbContext;
using Zxw.Framework.NetCore.Extensions;
using Zxw.Framework.NetCore.IRepositories;
using Zxw.Framework.NetCore.Models;

namespace Zxw.Framework.NetCore.Repositories
{
    public abstract class BaseRepository<T, TKey>:IRepository<T, TKey> where T : class, IBaseModel<TKey>
    {
        private readonly DefaultDbContext _dbContext;
        private readonly DbSet<T> _set;
        public BaseRepository(DefaultDbContext dbContext)
        {
            if (dbContext == null) throw new ArgumentNullException(nameof(dbContext));
            _dbContext = dbContext;
            _dbContext.Database.EnsureCreated();
            _set = dbContext.Set<T>();
        }
        public virtual int Add(T entity)
        {
            _set.Add(entity);
            return Save();
        }

        public virtual int AddRange(ICollection<T> entities)
        {
            _set.AddRange(entities);
            return Save();
        }

        public virtual void BulkInsert(IList<T> entities, string destinationTableName = null)
        {
            _dbContext.BulkInsert(entities, destinationTableName);
        }

        public virtual int Count(Expression<Func<T, bool>> @where = null)
        {
            return where == null ? _set.Count() : _set.Count(@where);
        }

        public virtual int Delete(TKey key)
        {
            var entity = _set.Find(key);
            if (entity == null) return 0;
            _set.Remove(entity);
            return Save();
        }

        public virtual int Delete(Expression<Func<T, bool>> @where)
        {
            return _dbContext.Delete(where);
        }

        public virtual int Edit(T entity)
        {
            return _dbContext.Edit(entity);
        }

        public virtual int EditRange(ICollection<T> entities)
        {
            return _dbContext.EditRange(entities);
        }

        public virtual bool Exist(Expression<Func<T, bool>> @where = null)
        {
            return Get(where).Any();
        }

        public virtual bool Exist(Expression<Func<T, bool>> @where = null, params Expression<Func<T, object>>[] includes)
        {
            return Get(where, includes).Any();
        }

        public virtual int ExecuteSqlWithNonQuery(string sql, params object[] parameters)
        {
            return _dbContext.ExecuteSqlWithNonQuery(sql, parameters);
        }

        public virtual T GetSingle(TKey key)
        {
            return _set.Find(key);
        }

        public T GetSingle(TKey key, params Expression<Func<T, object>>[] includes)
        {
            if (includes == null) return GetSingle(key);
            var query = _set.AsQueryable();
            foreach (var include in includes)
            {
                query = query.Include(include);
            }
            Expression<Func<T, bool>> filter = m => m.Id.Equal(key);
            return query.SingleOrDefault(filter.Compile());
        }

        public T GetSingle(Expression<Func<T, bool>> @where = null)
        {
            if (where == null) return _set.SingleOrDefault();
            return _set.SingleOrDefault(@where);
        }

        public T GetSingle(Expression<Func<T, bool>> @where = null, params Expression<Func<T, object>>[] includes)
        {
            if (includes == null) return GetSingle(where);
            var query = _set.AsQueryable();
            foreach (var include in includes)
            {
                query = query.Include(include);
            }
            if (where == null) return query.SingleOrDefault();
            return query.SingleOrDefault(where);
        }

        public virtual IQueryable<T> Get(Expression<Func<T, bool>> @where = null)
        {
            return @where != null ? _set.AsNoTracking().Where(@where) : _set.AsNoTracking();
        }

        public virtual IQueryable<T> Get(Expression<Func<T, bool>> @where = null, params Expression<Func<T, object>>[] includes)
        {
            if (includes == null)
                return Get(where);
            var query = _set.AsQueryable();
            foreach (var include in includes)
            {
                query = query.Include(include);
            }
            return @where != null ? query.AsNoTracking().Where(@where) : query.AsNoTracking();
        }

        public IEnumerable<T> GetByPagination(Expression<Func<T, bool>> @where, int pageSize, int pageIndex, bool asc = true, params Func<T, object>[] @orderby)
        {
            var filter = Get(where).AsEnumerable();
            if (orderby != null)
            {
                foreach (var func in orderby)
                {
                    filter = asc ? filter.OrderBy(func) : filter.OrderByDescending(func);
                }
            }
            return filter.Skip(pageSize * (pageIndex - 1)).Take(pageSize);
        }

        public virtual int Save()
        {
            return _dbContext.SaveChanges();
        }

        public virtual IList<TView> SqlQuery<TView>(string sql, params object[] parameters) where TView : class, new()
        {
            return _dbContext.SqlQuery<T, TView>(sql, parameters);
        }

        public virtual int Update(Expression<Func<T, bool>> @where, Expression<Func<T, T>> updateExp)
        {
            return _dbContext.Update(where, updateExp);
        }

        public virtual int Update(T model, params string[] updateColumns)
        {
            return _dbContext.Update(model, updateColumns);
        }

        public virtual void Dispose()
        {
            if (_dbContext != null)
            {
                _dbContext.Dispose();
            }
        }
    }
}
