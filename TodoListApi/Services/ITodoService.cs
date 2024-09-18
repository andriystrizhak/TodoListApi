using TodoListApi.Models;

namespace TodoListApi.Services
{
    public interface ITodoService
    {
        Task<IEnumerable<TodoItem>> GetAllTodosAsync();
        Task<TodoItem> GetTodoByIdAsync(int id);
        Task<TodoItem> CreateTodoAsync(TodoItem item);
        Task UpdateTodoAsync(TodoItem item);
        Task DeleteTodoAsync(int id);
    }
}
