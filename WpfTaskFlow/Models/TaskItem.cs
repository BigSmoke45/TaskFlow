using System;
using System.Collections.Generic;
using System.Text;

namespace WpfTaskFlow.Models
{

    public class TaskItem
    {
        // Унікальний ідентифікатор EF Core сам заповнює це поле
        public int Id { get; set; }

        // Назва задачі обов'язкове поле
        public string Title { get; set; } = string.Empty;

        // Опис задачі необов'язкове
        public string? Description { get; set; }

        // Пріоритет: Low, Medium, High
        public string Priority { get; set; } = "Medium";

        // Дата створення встановлюється автоматично
        public DateTime CreatedAt { get; set; } = DateTime.Now;

        // Статус виконання
        public bool IsCompleted { get; set; } = false;
    }
}
