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

        public List<TheTask> GetAll() => Tasks;

        public TheTask Get(int id) => Tasks.FirstOrDefault(t => t.Id == id);

        public void Add(TheTask task)
        {
            task.Id = Tasks.Count() + 1;
            Tasks.Add(task);
            saveToFile();
        }

        public void Delete(int id)
        {
            var task = Get(id);
            if (task is null)
                return;

            Tasks.Remove(task);
            saveToFile();
        }

        public void Update(TheTask task)
        {
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
