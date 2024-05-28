using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text.Json;
using Microsoft.AspNetCore.Hosting;
using MyTask.Interfaces;
using MyTask.Models;

namespace MyTask.Services
{
    public class MyTaskService : IMyTaskService
    {
        private List<TheTask> Tasks { get; }
        private string filePath;

        public MyTaskService(IWebHostEnvironment webHost)
        {
            this.filePath = Path.Combine(webHost.ContentRootPath, "Data", "MyTask.json");
            using (var jsonFile = File.OpenText(filePath))
            {
                Tasks = JsonSerializer.Deserialize<List<TheTask>>(
                    jsonFile.ReadToEnd(),
                    new JsonSerializerOptions { PropertyNameCaseInsensitive = true }
                );
            }
        }

        private void saveToFile()
        {
            File.WriteAllText(filePath, JsonSerializer.Serialize(Tasks));
        }

        public List<TheTask> GetAll(int userId) => Tasks.FindAll(t => t.UserId == userId);
   

        public TheTask Get(int userId, int id) => Tasks.FirstOrDefault(t => t.UserId == userId && t.Id == id);

        public void Add(int userId, TheTask task)
        {
            task.Id = Tasks.Count() + 1;
            task.UserId = userId;
            Tasks.Add(task);
            saveToFile();
        }

        public void Delete(int userId, int id)
        {
            var task = Get(userId, id);
            if (task is null)
                return;

            Tasks.Remove(task);
            saveToFile();
        }

        public void Update(int userId, TheTask task)
        {
            task.UserId = userId;
            var index = Tasks.FindIndex(t => t.Id == task.Id);
            if (index == -1)
                return;

            Tasks[index] = task;
            saveToFile();
        }

        public int Count => Tasks.Count();
    }

    public static class TaskUtils
    {
        public static void AddTask(this IServiceCollection services)
        {
            services.AddSingleton<IMyTaskService, MyTaskService>();
        }
    }
}
