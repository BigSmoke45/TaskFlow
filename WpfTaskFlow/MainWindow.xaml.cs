using Microsoft.EntityFrameworkCore;
using System.Windows;
using WpfTaskFlow.Data;
using WpfTaskFlow.Models;
using WpfTaskFlow.Repository;

namespace WpfTaskFlow
{
    // Допоміжний клас для відображення задачі в ListView
    public class TaskViewModel
    {
        public int Id { get; set; }
        public string Title { get; set; } = "";
        public string? Description { get; set; }
        public string Priority { get; set; } = "";
        public string StatusText { get; set; } = "";
        public string CreatedAtText { get; set; } = "";
        public bool IsCompleted { get; set; }
    }

    public partial class MainWindow : Window
    {
        private readonly AppDbContext _context;
        private readonly TaskRepository _repo;

        public MainWindow()
        {
            InitializeComponent();

            // Ініціалізація БД
            _context = new AppDbContext();
            _context.Database.EnsureCreated();
            _repo = new TaskRepository(_context);

            // Всі задачі при старті
            LoadTasks(_repo.GetAllTasks(), "Всі задачі");
        }

        // Перетворення TaskItem в TaskViewModel для відображення
        private List<TaskViewModel> ToViewModel(List<TaskItem> tasks)
        {
            return tasks.Select(t => new TaskViewModel
            {
                Id = t.Id,
                Title = t.Title,
                Description = t.Description,
                Priority = t.Priority,
                StatusText = t.IsCompleted ? "✅ Виконано" : "🔄 Активна",
                CreatedAtText = t.CreatedAt.ToString("dd.MM.yyyy HH:mm"),
                IsCompleted = t.IsCompleted
            }).ToList();
        }

        // Завантаження задачі в ListView
        private void LoadTasks(List<TaskItem> tasks, string title)
        {
            PageTitle.Text = $"{title} ({tasks.Count})";
            TaskListView.ItemsSource = ToViewModel(tasks);
            ActionPanel.Visibility = Visibility.Collapsed;
        }

        // Повернення вибраної задачі або null
        private TaskViewModel? GetSelected()
        {
            return TaskListView.SelectedItem as TaskViewModel;
        }

        // Навігація

        private void ShowAll_Click(object sender, RoutedEventArgs e)
            => LoadTasks(_repo.GetAllTasks(), "Всі задачі");

        private void ShowActive_Click(object sender, RoutedEventArgs e)
            => LoadTasks(_repo.GetActiveTasks(), "Активні задачі");

        private void ShowCompleted_Click(object sender, RoutedEventArgs e)
            => LoadTasks(_repo.GetCompletedTasks(), "Виконані задачі");

        // Показ стат-и у вікні повідомлення
        private void ShowStats_Click(object sender, RoutedEventArgs e)
        {
            var (total, active, completed, high) = _repo.GetStats();
            int percent = total == 0 ? 0 : (int)((double)completed / total * 100);

            MessageBox.Show(
                $"Всього задач:        {total}\n" +
                $"Активних:            {active}\n" +
                $"Виконаних:           {completed}\n" +
                $"Високий пріоритет:   {high}\n\n" +
                $"Прогрес:             {percent}%",
                "Статистика",
                MessageBoxButton.OK,
                MessageBoxImage.Information);
        }

        // Показ панелі дій при виборі задачі
        private void TaskListView_SelectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
        {
            ActionPanel.Visibility = GetSelected() != null
                ? Visibility.Visible
                : Visibility.Collapsed;
        }

        // Дії з задачами

        private void AddTask_Click(object sender, RoutedEventArgs e)
        {
            var dialog = new TaskDialog();
            if (dialog.ShowDialog() == true)
            {
                _repo.AddTask(new TaskItem
                {
                    Title = dialog.TaskTitle,
                    Description = dialog.TaskDescription,
                    Priority = dialog.TaskPriority
                });
                LoadTasks(_repo.GetAllTasks(), "Всі задачі");
            }
        }

        private void Complete_Click(object sender, RoutedEventArgs e)
        {
            var selected = GetSelected();
            if (selected == null) return;

            _repo.CompleteTask(selected.Id);
            LoadTasks(_repo.GetAllTasks(), "Всі задачі");
        }

        private void Edit_Click(object sender, RoutedEventArgs e)
        {
            var selected = GetSelected();
            if (selected == null) return;

            var dialog = new TaskDialog(selected.Title, selected.Description, selected.Priority);
            if (dialog.ShowDialog() == true)
            {
                _repo.EditTask(selected.Id, dialog.TaskTitle, dialog.TaskDescription, dialog.TaskPriority);
                LoadTasks(_repo.GetAllTasks(), "Всі задачі");
            }
        }

        private void Delete_Click(object sender, RoutedEventArgs e)
        {
            var selected = GetSelected();
            if (selected == null) return;

            var result = MessageBox.Show(
                $"Видалити задачу \"{selected.Title}\"?",
                "Підтвердження",
                MessageBoxButton.YesNo,
                MessageBoxImage.Warning);

            if (result == MessageBoxResult.Yes)
            {
                _repo.DeleteTask(selected.Id);
                LoadTasks(_repo.GetAllTasks(), "Всі задачі");
            }
        }
    }
}