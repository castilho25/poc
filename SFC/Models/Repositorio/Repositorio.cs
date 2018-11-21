using IdentitySample.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;

namespace SFC.Models.Repositorio
{
    public class Repository<T> where T : class
    {
        private ApplicationDbContext context = new ApplicationDbContext();

        protected DbSet<T> DbSet
        {
            get; set;
        }
        public Repository()
        {
            DbSet = context.Set<T>();
        }
        public List<T> GetList()
        {
            return DbSet.ToList();
        }
        public T Get(Guid id)
        {
            return DbSet.Find(id);
        }
        public void Add(T entity)
        {
            DbSet.Add(entity);
        }
        public void Remove(T entity)
        {
            DbSet.Remove(entity);
        }
        public void SaveChanges()
        {
            context.SaveChanges();
        }
    }
}