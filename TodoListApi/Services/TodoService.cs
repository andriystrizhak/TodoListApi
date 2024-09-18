using TodoListApi.Models;
using TodoListApi.Repositories;

namespace TodoListApi.Services
{
    public class TodoService : ITodoService
    {
        private readonly ITodoRepository _repository;

        public TodoService(ITodoRepository repository)
        {
            _repository = repository;
        }

        public async Task<IEnumerable<TodoItem>> GetAllTodosAsync()
        {
            return await _repository.GetAllAsync();
        }

        public async Task<TodoItem> GetTodoByIdAsync(int id)
        {
            return await _repository.GetByIdAsync(id);
        }

        public async Task<TodoItem> CreateTodoAsync(TodoItem item)
        {
            return await _repository.AddAsync(item);
        }

        public async Task UpdateTodoAsync(TodoItem item)
        {
            await _repository.UpdateAsync(item);
        }

        public async Task DeleteTodoAsync(int id)
        {
            await _repository.DeleteAsync(id);
        }
    }
}
