# TaskFlow — Task Manager

A C# task manager with a graphical interface (WPF). Data is stored locally in SQLite via Entity Framework Core.

---

## Versions

### 🖥️ WPF Version (`WpfTaskFlow/`)

A graphical interface with a dark theme (Catppuccin-inspired color palette).

- Add, edit, and delete tasks via a dialog window
- Filter tasks: All / Active / Completed
- Priority levels: Low / Medium / High with color indicators
- Task statistics with completion percentage
- Confirmation dialog before deleting
- Dark theme (Catppuccin-inspired color palette)

**Screenshots:**
<img width="1290" height="783" alt="image" src="https://github.com/user-attachments/assets/4a877037-7d56-4182-afa1-30c021fcaf53" />

  
<img width="451" height="460" alt="image" src="https://github.com/user-attachments/assets/5944f2b0-4748-4dca-b3f5-fffa97b7bd2d" />

  
<img width="1347" height="790" alt="image" src="https://github.com/user-attachments/assets/577bc88e-3e96-4b63-9d6b-49b1ba066353" />

---

## Tech Stack

| Area | Technologies |
|------|-------------|
| Language | C# (.NET 10) |
| UI | WPF (Windows Presentation Foundation) |
| ORM | Entity Framework Core 10 |
| Database | SQLite |
| Patterns | Repository pattern, separation of concerns |
| Testing | xUnit, EF Core InMemory |

---

## Project Structure

```
TaskFlow/
├── WpfTaskFlow/                  ← Main application
│   ├── Models/
│   │   └── TaskItem.cs           # Entity model
│   ├── Data/
│   │   └── AppDbContext.cs       # EF Core DB context
│   ├── Repository/
│   │   └── TaskRepository.cs     # All database logic
│   ├── ViewModels/
│   │   ├── MainViewModel.cs      # Main MVVM ViewModel
│   │   ├── TaskViewModel.cs      # Display model for ListView
│   │   └── RelayCommand.cs       # ICommand implementation
│   ├── MainWindow.xaml           # Main application window
│   ├── TaskDialog.xaml           # Add / Edit task dialog
│   └── App.xaml
└── TaskFlow.Tests/               ← Unit tests
    └── TaskRepositoryTests.cs    # 19 tests for TaskRepository
```

---

## Key Implementation Details

MVVM pattern — MainViewModel implements INotifyPropertyChanged and exposes ICommand bindings. MainWindow.xaml.cs contains no business logic — only DataContext = new MainViewModel().

RelayCommand — a generic ICommand implementation that accepts Action and Func<bool> lambdas, keeping ViewModel code clean without third-party libraries.

Repository pattern — all database logic lives in TaskRepository. The UI layer never touches the database directly.

Entity Framework Core + SQLite — the database file (taskflow.db) is created automatically on first run via EnsureCreated(). No manual SQL required.

Unit tests — TaskRepository is covered by 19 xUnit tests using an EF Core InMemory database. Each test gets a clean isolated database instance via Guid.NewGuid() as the database name.

WPF dialog window — TaskDialog is reused for both creating new tasks and editing existing ones. When editing, it pre-fills the fields with the current task data.

---

## Run

### WPF version
Open `WpfTaskFlow/WpfTaskFlow.sln` in Visual Studio and press **F5**.

Or via CLI:
```bash
dotnet run --project WpfTaskFlow/WpfTaskFlow
```

---

## Notes

A pet project built to demonstrate C# desktop development with WPF, MVVM, Entity Framework Core, and unit testing.
