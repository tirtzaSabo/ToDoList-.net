using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;
using MyTask.Interfaces;
using MyTask.Models;
using Microsoft.AspNetCore.Hosting;

namespace MyTask.Services
{
    public class UserService :  IUserService
    {
        private List<User> Users { get; }
        private string filePath;

        public UserService(IWebHostEnvironment webHost)
        {
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

        public void Delete(int id)
        {
            var user = Get(id);
            if (user is null)
                return;

            Users.Remove(user);
            saveToFile();
        }

        public User Get(int id) => Users.FirstOrDefault(u => u.Id == id);


        public List<User> GetAll() => Users;

        public void Update(User user)
        {
            var index = Users.FindIndex(t => t.Id == user.Id);
            if (index == -1)
                return;

            Users[index] = user;
            saveToFile();
        }
        public User GetUser(string name, string password)
        {
            return Users.Find(u => u.Name == name && u.password==password);
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
