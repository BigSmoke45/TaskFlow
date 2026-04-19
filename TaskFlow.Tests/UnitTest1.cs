
// dotnet add package Microsoft.EntityFrameworkCore.InMemory
// dotnet add package xunit
// dotnet add package xunit.runner.visualstudio
// dotnet add package coverlet.collector

using Microsoft.EntityFrameworkCore;
using WpfTaskFlow.Data;
using WpfTaskFlow.Models;
using WpfTaskFlow.Repository;

namespace TaskFlow.Tests;

// Хелпер: створює чистий контекст із InMemory БД для кожного тесту
// Щоразу нове унікальне ім'я БД - тести ізольовані один від одного
public static class TestDbFactory
{
    public static AppDbContext Create()
    {
        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;

        return new AppDbContext(options);
    }
}

public class TaskRepositoryTests
{

    // AddTask

    [Fact]
    public void AddTask_ShouldPersistTaskToDatabase()
    {
        // Arrange - готуємо дані та залежності
        using var context = TestDbFactory.Create();
        var repo = new TaskRepository(context);
        var task = new TaskItem { Title = "Test task", Priority = "High" };

        // Act - викликаємо метод який тестуємо
        repo.AddTask(task);

        // Assert - перевіряємо результат
        var tasks = context.Tasks.ToList();
        Assert.Single(tasks);                        // у БД рівно 1 завдання
        Assert.Equal("Test task", tasks[0].Title);   // з правильною назвою
    }

    [Fact]
    public void AddTask_ShouldAssignIdAutomatically()
    {
        using var context = TestDbFactory.Create();
        var repo = new TaskRepository(context);
        var task = new TaskItem { Title = "Auto ID task" };

        repo.AddTask(task);

        // EF Core має сам призначити Id > 0
        Assert.True(task.Id > 0);
    }

    // GetAllTasks

    [Fact]
    public void GetAllTasks_WhenEmpty_ShouldReturnEmptyList()
    {
        using var context = TestDbFactory.Create();
        var repo = new TaskRepository(context);

        var result = repo.GetAllTasks();

        Assert.Empty(result);
    }

    [Fact]
    public void GetAllTasks_ShouldReturnAllTasks()
    {
        using var context = TestDbFactory.Create();
        var repo = new TaskRepository(context);
        repo.AddTask(new TaskItem { Title = "Task 1" });
        repo.AddTask(new TaskItem { Title = "Task 2" });
        repo.AddTask(new TaskItem { Title = "Task 3" });

        var result = repo.GetAllTasks();

        Assert.Equal(3, result.Count);
    }

    // GetActiveTasks / GetCompletedTasks

    [Fact]
    public void GetActiveTasks_ShouldReturnOnlyNotCompletedTasks()
    {
        using var context = TestDbFactory.Create();
        var repo = new TaskRepository(context);
        repo.AddTask(new TaskItem { Title = "Active",    IsCompleted = false });
        repo.AddTask(new TaskItem { Title = "Done",      IsCompleted = true  });
        repo.AddTask(new TaskItem { Title = "Also active", IsCompleted = false });

        var result = repo.GetActiveTasks();

        Assert.Equal(2, result.Count);
        Assert.All(result, t => Assert.False(t.IsCompleted)); // все у списку - активні
    }

    [Fact]
    public void GetCompletedTasks_ShouldReturnOnlyCompletedTasks()
    {
        using var context = TestDbFactory.Create();
        var repo = new TaskRepository(context);
        repo.AddTask(new TaskItem { Title = "Active",    IsCompleted = false });
        repo.AddTask(new TaskItem { Title = "Done 1",    IsCompleted = true  });
        repo.AddTask(new TaskItem { Title = "Done 2",    IsCompleted = true  });

        var result = repo.GetCompletedTasks();

        Assert.Equal(2, result.Count);
        Assert.All(result, t => Assert.True(t.IsCompleted));
    }

    // GetTasksByPriority

    [Fact]
    public void GetTasksByPriority_ShouldReturnOnlyMatchingPriority()
    {
        using var context = TestDbFactory.Create();
        var repo = new TaskRepository(context);
        repo.AddTask(new TaskItem { Title = "H1", Priority = "High"   });
        repo.AddTask(new TaskItem { Title = "H2", Priority = "High"   });
        repo.AddTask(new TaskItem { Title = "L1", Priority = "Low"    });
        repo.AddTask(new TaskItem { Title = "M1", Priority = "Medium" });

        var result = repo.GetTasksByPriority("High");

        Assert.Equal(2, result.Count);
        Assert.All(result, t => Assert.Equal("High", t.Priority));
    }

    [Fact]
    public void GetTasksByPriority_WhenNoneMatch_ShouldReturnEmptyList()
    {
        using var context = TestDbFactory.Create();
        var repo = new TaskRepository(context);
        repo.AddTask(new TaskItem { Title = "Low task", Priority = "Low" });

        var result = repo.GetTasksByPriority("High");

        Assert.Empty(result);
    }

    // GetById

    [Fact]
    public void GetById_WhenExists_ShouldReturnCorrectTask()
    {
        using var context = TestDbFactory.Create();
        var repo = new TaskRepository(context);
        var task = new TaskItem { Title = "Find me" };
        repo.AddTask(task);

        var result = repo.GetById(task.Id);

        Assert.NotNull(result);
        Assert.Equal("Find me", result.Title);
    }

    [Fact]
    public void GetById_WhenNotExists_ShouldReturnNull()
    {
        using var context = TestDbFactory.Create();
        var repo = new TaskRepository(context);

        var result = repo.GetById(999);

        Assert.Null(result);
    }

    // CompleteTask

    [Fact]
    public void CompleteTask_WhenExists_ShouldMarkAsCompletedAndReturnTrue()
    {
        using var context = TestDbFactory.Create();
        var repo = new TaskRepository(context);
        var task = new TaskItem { Title = "Finish me", IsCompleted = false };
        repo.AddTask(task);

        var result = repo.CompleteTask(task.Id);

        Assert.True(result);
        Assert.True(context.Tasks.Find(task.Id)!.IsCompleted);
    }

    [Fact]
    public void CompleteTask_WhenNotExists_ShouldReturnFalse()
    {
        using var context = TestDbFactory.Create();
        var repo = new TaskRepository(context);

        var result = repo.CompleteTask(999);

        Assert.False(result);
    }

    // DeleteTask

    [Fact]
    public void DeleteTask_WhenExists_ShouldRemoveTaskAndReturnTrue()
    {
        using var context = TestDbFactory.Create();
        var repo = new TaskRepository(context);
        var task = new TaskItem { Title = "Delete me" };
        repo.AddTask(task);

        var result = repo.DeleteTask(task.Id);

        Assert.True(result);
        Assert.Empty(context.Tasks.ToList()); // БД порожня після видалення
    }

    [Fact]
    public void DeleteTask_WhenNotExists_ShouldReturnFalse()
    {
        using var context = TestDbFactory.Create();
        var repo = new TaskRepository(context);

        var result = repo.DeleteTask(999);

        Assert.False(result);
    }

    // EditTask

    [Fact]
    public void EditTask_WhenExists_ShouldUpdateFieldsAndReturnTrue()
    {
        using var context = TestDbFactory.Create();
        var repo = new TaskRepository(context);
        var task = new TaskItem { Title = "Old title", Description = "Old desc", Priority = "Low" };
        repo.AddTask(task);

        var result = repo.EditTask(task.Id, "New title", "New desc", "High");

        Assert.True(result);
        var updated = context.Tasks.Find(task.Id)!;
        Assert.Equal("New title", updated.Title);
        Assert.Equal("New desc",  updated.Description);
        Assert.Equal("High",      updated.Priority);
    }

    [Fact]
    public void EditTask_WithEmptyTitle_ShouldNotChangeTitleField()
    {
        // Порожнє поле не повинно перетирати старе значення - важлива бізнес-логіка
        using var context = TestDbFactory.Create();
        var repo = new TaskRepository(context);
        var task = new TaskItem { Title = "Keep me", Priority = "Low" };
        repo.AddTask(task);

        repo.EditTask(task.Id, "", "", "High"); // пусті title и description

        var updated = context.Tasks.Find(task.Id)!;
        Assert.Equal("Keep me", updated.Title); // title не змінився
        Assert.Equal("High",    updated.Priority);
    }

    [Fact]
    public void EditTask_WhenNotExists_ShouldReturnFalse()
    {
        using var context = TestDbFactory.Create();
        var repo = new TaskRepository(context);

        var result = repo.EditTask(999, "X", "X", "High");

        Assert.False(result);
    }

    // GetStats

    [Fact]
    public void GetStats_ShouldReturnCorrectCounts()
    {
        using var context = TestDbFactory.Create();
        var repo = new TaskRepository(context);

        repo.AddTask(new TaskItem { Title = "A1", Priority = "High",   IsCompleted = false });
        repo.AddTask(new TaskItem { Title = "A2", Priority = "High",   IsCompleted = false });
        repo.AddTask(new TaskItem { Title = "A3", Priority = "Medium", IsCompleted = false });
        repo.AddTask(new TaskItem { Title = "C1", Priority = "High",   IsCompleted = true  }); // виконана не в HighPriority
        repo.AddTask(new TaskItem { Title = "C2", Priority = "Low",    IsCompleted = true  });

        var (total, active, completed, highPriority) = repo.GetStats();

        Assert.Equal(5, total);
        Assert.Equal(3, active);
        Assert.Equal(2, completed);
        Assert.Equal(2, highPriority); // тільки невиконані High
    }

    [Fact]
    public void GetStats_WhenEmpty_ShouldReturnAllZeros()
    {
        using var context = TestDbFactory.Create();
        var repo = new TaskRepository(context);

        var (total, active, completed, highPriority) = repo.GetStats();

        Assert.Equal(0, total);
        Assert.Equal(0, active);
        Assert.Equal(0, completed);
        Assert.Equal(0, highPriority);
    }
}