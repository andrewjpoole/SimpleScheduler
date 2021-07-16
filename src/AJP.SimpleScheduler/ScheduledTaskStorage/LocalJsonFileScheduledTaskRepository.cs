using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;
using AJP.SimpleScheduler.ScheduledTasks;
using NodaTime;

namespace AJP.SimpleScheduler.ScheduledTaskStorage
{
    public class LocalJsonFileScheduledTaskRepository : IScheduledTaskRepository
    {
        private Dictionary<string, ScheduledTask> _allTasks = new Dictionary<string, ScheduledTask>();
        public IClock ClockInstance { get; private set; }

        private string _jsonFilePath;
        public string JsonFilePath {
            get
            {
                if (string.IsNullOrEmpty(_jsonFilePath))
                    _jsonFilePath = ".\\localScheduledTaskStorage.json";

                return _jsonFilePath;
            }
            set => _jsonFilePath = value;
        }

        public LocalJsonFileScheduledTaskRepository(IClock clock)
        {
            ClockInstance = clock;

            if (File.Exists(JsonFilePath))
                LoadStateFromFile();
        }

        public void AddScheduledTask(ScheduledTask task)
        {
            _allTasks.Add(task.Id, task);
            PersistStateToFile();
        }

        public void AddScheduledTask(ScheduledTaskBuilder taskBuilder)
        {
            var task = taskBuilder.CreateTask();
            _allTasks.Add(task.Id, task);
            PersistStateToFile();
        }

        public void RemoveScheduledTask(string id)
        {
            if (_allTasks.ContainsKey(id))
                _allTasks.Remove(id);
            PersistStateToFile();
        }

        public List<ScheduledTask> AllTasks()
        {
            return _allTasks.Values.ToList();
        }

        public List<ScheduledTask> DetermineIfAnyTasksAreDue()
        {
            return _allTasks.Values.Where(task => task.Due < ClockInstance.GetCurrentInstant().ToDateTimeUtc()).ToList();
        }

        public ScheduledTask GetById(string id)
        {
            return _allTasks.ContainsKey(id) ? _allTasks[id] : null;
        }

        public void UpdateScheduledTask(ScheduledTask dueTask)
        {
            _allTasks[dueTask.Id] = dueTask;
            PersistStateToFile();
        }

        private void PersistStateToFile()
        {
            File.WriteAllText(JsonFilePath, JsonSerializer.Serialize(_allTasks));
        }

        private void LoadStateFromFile()
        {
            try
            {
                var jsonFromFile = File.ReadAllText(JsonFilePath);
                var stateFromFile = JsonSerializer.Deserialize<Dictionary<string, ScheduledTask>>(jsonFromFile);

                if (stateFromFile != null)
                    _allTasks = stateFromFile;
            }
            catch (Exception e)
            {
                Console.WriteLine($"Unable to load state from file {JsonFilePath} {e.Message}");
                throw;
            }
        }
    }
}