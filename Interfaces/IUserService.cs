using MyTask.Models;
using System.Collections.Generic;


namespace MyTask.Interfaces
{
        public interface IUserService
    {
        List<User> GetAll();
        User Get(int id);
        void Add(User user);
        void Delete(int id);
        bool Update(User user);
        int Count {get;}
        User GetUser(string name, string password);
    }
}