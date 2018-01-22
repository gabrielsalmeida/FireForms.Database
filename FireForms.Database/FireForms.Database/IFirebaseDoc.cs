using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading.Tasks;
using FireForms.Database.Model;
using LiteDB;

namespace FireForms.Database
{
    public interface IFirebaseDoc<T>
    {
        FirebaseDatabase FirebaseDatabase { get; set; }
        LiteCollection<T> Collection { get; set; }

        void DeleteAll();
        Task DeleteAsync(T obj);
        Task DeleteAsync(Expression<Func<T, bool>> predicate);
        Task<IEnumerable<T>> GetAllAsync();
        Task<T> GetAsync(Expression<Func<T, bool>> predicate);
        Task Listen();
        void StopListening();
        Task<StatusEnum> UpsertAsync(T obj);
    }
}
