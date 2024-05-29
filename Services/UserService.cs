using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;
using Microsoft.AspNetCore.Hosting;
using MyTask.Interfaces;
using MyTask.Models;

namespace MyTask.Services
{
    public class UserService : IUserService
    {
        private List<User> Users { get; }
        private string filePath;
        IMyTaskService TaskService;
        private List<TheTask> Tasks { get; }

        public UserService(IWebHostEnvironment webHost, IMyTaskService TaskService)
        {
            this.TaskService = TaskService;
            this.filePath = Path.Combine(webHost.ContentRootPath, "Data", "Users.json");
            using (var jsonFile = File.OpenText(filePath))
            {
                Users = JsonSerializer.Deserialize<List<User>>(
                    jsonFile.ReadToEnd(),
                    new JsonSerializerOptions { PropertyNameCaseInsensitive = true }
                );
            }
        }

        private void saveToFile()
        {
            File.WriteAllText(filePath, JsonSerializer.Serialize(Users));
        }

        public int Count => Users.Count();

        public void Add(User user)
        {
            user.Id = Users.Count() + 1;
            Users.Add(user);
            saveToFile();
        }

        public void Delete(int userId)
        {
            var user = Get(userId);
            if (user is null)
                return;
            var tasksUser = TaskService.GetAll(userId);
            tasksUser.ForEach(task => TaskService.Delete(userId, task.Id));
            Users.Remove(user);
            saveToFile();
        }

        public User Get(int id) => Users.FirstOrDefault(u => u.Id == id);

        public List<User> GetAll() => Users;

        public bool Update(User newUser)
        {
            var existingUser = Get(newUser.Id);
            if (existingUser == null)
                return false;

            var index = Users.IndexOf(existingUser);
            if (index == -1)
                return false;

            Users[index] = newUser;

            saveToFile();

            return true;
        }

        public User GetUser(string name, string password)
        {
            return Users.Find(u => u.Name == name && u.Password == password);
        }
    }

    public static class UserUtils
    {
        public static void AddUser(this IServiceCollection services)
        {
            services.AddSingleton<IUserService, UserService>();
        }
    }
}
