using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Input;
using WpfTaskFlow.Data;
using WpfTaskFlow.Models;
using WpfTaskFlow.Repository;
using WpfTaskFlow.ViewModels;

namespace WpfTaskFlow.ViewModels
{
    public class MainViewModel : INotifyPropertyChanged
    {
        private readonly AppDbContext _context;
        private readonly TaskRepository _repo;

        // Список задач для ListView — ObservableCollection автоматично оновлює UI при змінах
        private ObservableCollection<TaskViewModel> _tasks = new();
        public ObservableCollection<TaskViewModel> Tasks
        {
            get => _tasks;
            set { _tasks = value; OnPropertyChanged(); }
        }

        // Заголовок сторінки
        private string _pageTitle = "Всі задачі";
        public string PageTitle
        {
            get => _pageTitle;
            set { _pageTitle = value; OnPropertyChanged(); }
        }

        // Обрана задача
        private TaskViewModel? _selectedTask;
        public TaskViewModel? SelectedTask
        {
            get => _selectedTask;
            set { _selectedTask = value; OnPropertyChanged(); OnPropertyChanged(nameof(IsTaskSelected)); }
        }

        // Видимість панелі
        public bool IsTaskSelected => _selectedTask != null;

        // Команди

        public ICommand ShowAllCommand { get; }
        public ICommand ShowActiveCommand { get; }
        public ICommand ShowCompletedCommand { get; }
        public ICommand ShowStatsCommand { get; }
        public ICommand AddTaskCommand { get; }
        public ICommand CompleteTaskCommand { get; }
        public ICommand EditTaskCommand { get; }
        public ICommand DeleteTaskCommand { get; }

        // Конструктор

        public MainViewModel()
        {
            _context = new AppDbContext();
            _context.Database.EnsureCreated();
            _repo = new TaskRepository(_context);

            // Прив'язка команд до методів
            ShowAllCommand = new RelayCommand(_ => LoadTasks(_repo.GetAllTasks(), "Всі задачі"));
            ShowActiveCommand = new RelayCommand(_ => LoadTasks(_repo.GetActiveTasks(), "Активні задачі"));
            ShowCompletedCommand = new RelayCommand(_ => LoadTasks(_repo.GetCompletedTasks(), "Виконані задачі"));
            ShowStatsCommand = new RelayCommand(_ => ShowStats());
            AddTaskCommand = new RelayCommand(_ => AddTask());
            CompleteTaskCommand = new RelayCommand(_ => CompleteTask(), _ => IsTaskSelected);
            EditTaskCommand = new RelayCommand(_ => EditTask(), _ => IsTaskSelected);
            DeleteTaskCommand = new RelayCommand(_ => DeleteTask(), _ => IsTaskSelected);

            LoadTasks(_repo.GetAllTasks(), "Всі задачі");
        }

        // Методи

        private void LoadTasks(System.Collections.Generic.List<TaskItem> tasks, string title)
        {
            PageTitle = $"{title} ({tasks.Count})";
            Tasks = new ObservableCollection<TaskViewModel>(
                tasks.Select(t => new TaskViewModel
                {
                    Id = t.Id,
                    Title = t.Title,
                    Description = t.Description,
                    Priority = t.Priority,
                    StatusText = t.IsCompleted ? "✅ Виконано" : "🔄 Активна",
                    CreatedAtText = t.CreatedAt.ToString("dd.MM.yyyy HH:mm"),
                    IsCompleted = t.IsCompleted
                })
            );
            SelectedTask = null;
        }

        private void ShowStats()
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

        private void AddTask()
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

        private void CompleteTask()
        {
            if (SelectedTask == null) return;
            _repo.CompleteTask(SelectedTask.Id);
            LoadTasks(_repo.GetAllTasks(), "Всі задачі");
        }

        private void EditTask()
        {
            if (SelectedTask == null) return;
            var dialog = new TaskDialog(SelectedTask.Title, SelectedTask.Description, SelectedTask.Priority);
            if (dialog.ShowDialog() == true)
            {
                _repo.EditTask(SelectedTask.Id, dialog.TaskTitle, dialog.TaskDescription, dialog.TaskPriority);
                LoadTasks(_repo.GetAllTasks(), "Всі задачі");
            }
        }

        private void DeleteTask()
        {
            if (SelectedTask == null) return;
            var result = MessageBox.Show(
                $"Видалити задачу \"{SelectedTask.Title}\"?",
                "Підтвердження",
                MessageBoxButton.YesNo,
                MessageBoxImage.Warning);

            if (result == MessageBoxResult.Yes)
            {
                _repo.DeleteTask(SelectedTask.Id);
                LoadTasks(_repo.GetAllTasks(), "Всі задачі");
            }
        }

        // INotifyPropertyChanged

        public event PropertyChangedEventHandler? PropertyChanged;

        protected void OnPropertyChanged([CallerMemberName] string? name = null)
            => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
    }
}