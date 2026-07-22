using System;
using System.Collections.Generic;
using System.Text;

namespace PetWalk.Repositories
{
    public interface IRepository<T> where T : class
    {
        List<T> GetAll();
        T? GetById(int id);
        void Add(T entity);
        void Update(T entity);
        void Delete(int id);
    }
}
