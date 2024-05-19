using MyTask.Models;
using System.Collections.Generic;

namespace MyTask.Interfaces
{
        public interface IMyTaskService
    {
        List<TheTask> GetAll(int userId);
        TheTask Get(int userId,int id);
        void Add(int userId,TheTask task);
        void Delete(int userId,int id);
        void Update(int userId,TheTask task);
        int Count {get;}
    }
}