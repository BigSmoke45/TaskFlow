using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.EntityFrameworkCore;
using WpfTaskFlow.Data;
using WpfTaskFlow.Models;

namespace WpfTaskFlow.Repository
{
    public class TaskRepository
    {
        private readonly AppDbContext _context;

        // Конструктор отримує контекст БД
        public TaskRepository(AppDbContext context)
        {
            _context = context;
        }

        // Додати нову задачу
        public void AddTask(TaskItem task)
        {
            _context.Tasks.Add(task);
            _context.SaveChanges();
        }

        // Отримати всі задачі
        public List<TaskItem> GetAllTasks()
        {
            return _context.Tasks.ToList();
        }

        // Отримати тільки невиконані задачі
        public List<TaskItem> GetActiveTasks()
        {
            return _context.Tasks
                .Where(t => !t.IsCompleted)
                .ToList();
        }

        // Отримати тільки виконані задачі
        public List<TaskItem> GetCompletedTasks()
        {
            return _context.Tasks
                .Where(t => t.IsCompleted)
                .ToList();
        }

        // Отримати задачі за пріоритетом
        public List<TaskItem> GetTasksByPriority(string priority)
        {
            return _context.Tasks
                .Where(t => t.Priority == priority)
                .ToList();
        }

        // Знайти задачу за Id
        public TaskItem? GetById(int id)
        {
            return _context.Tasks.FirstOrDefault(t => t.Id == id);
        }

        // Позначити задачу як виконану
        public bool CompleteTask(int id)
        {
            var task = GetById(id);
            if (task == null) return false;

            task.IsCompleted = true;
            _context.SaveChanges();
            return true;
        }

        // Видалити задачу за Id
        public bool DeleteTask(int id)
        {
            var task = GetById(id);
            if (task == null) return false;

            _context.Tasks.Remove(task);
            _context.SaveChanges();
            return true;
        }

        // Редагувати існуючу задачу
        public bool EditTask(int id, string newTitle, string newDescription, string newPriority)
        {
            var task = GetById(id);
            if (task == null) return false;

            // Оновлення тільки тих полів які не порожні
            if (!string.IsNullOrWhiteSpace(newTitle))
                task.Title = newTitle;

            if (!string.IsNullOrWhiteSpace(newDescription))
                task.Description = newDescription;

            if (!string.IsNullOrWhiteSpace(newPriority))
                task.Priority = newPriority;

            _context.SaveChanges();
            return true;
        }

        // Підрахунок статистики по всіх задачах
        public (int Total, int Active, int Completed, int HighPriority) GetStats()
        {
            var all = _context.Tasks.ToList();

            int total = all.Count;
            int completed = all.Count(t => t.IsCompleted);
            int active = total - completed;
            int high = all.Count(t => t.Priority == "High" && !t.IsCompleted);

            return (total, active, completed, high);
        }


    }
}