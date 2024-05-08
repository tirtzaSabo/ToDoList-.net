using MyTask.Models;
using System.Collections.Generic;

namespace MyTask.Interfaces
{
        public interface IMyTaskService
    {
        List<TheTask> GetAll();
        TheTask Get(int id);
        void Add(TheTask task);
        void Delete(int id);
        void Update(TheTask task);
        int Count {get;}
    }
}