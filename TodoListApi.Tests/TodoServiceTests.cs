using Moq;
using TodoListApi.Exceptions;
using TodoListApi.Models;
using TodoListApi.Repositories;
using TodoListApi.Services;

namespace TodoListApi.Tests
{
    [TestFixture]
    public class TodoServiceTests
    {
        private Mock<ITodoRepository> _mockRepository;
        private TodoService _todoService;

        [SetUp]
        public void SetUp()
        {
            // Create mock repository
            _mockRepository = new Mock<ITodoRepository>();

            // Create the service to test, passing the mock repository
            _todoService = new TodoService(_mockRepository.Object);
        }

        [Test]
        public async Task GetAllTodosAsync_WhenCalled_ShouldReturnAllTodoItems()
        {
            // Arrange
            var todoItems = new List<TodoItem>
        {
            new TodoItem { Id = 1, Title = "Task 1", Description = "Description 1", IsComplete = false },
            new TodoItem { Id = 2, Title = "Task 2", Description = "Description 2", IsComplete = true }
        };

            // Set up the repository to return the list of todo items
            _mockRepository.Setup(repo => repo.GetAllAsync()).ReturnsAsync(todoItems);

            // Act
            var result = await _todoService.GetAllTodosAsync();

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(2, result.Count());
            CollectionAssert.AreEqual(todoItems, result);
        }

        [Test]
        public async Task GetTodoByIdAsync_WhenItemExists_ShouldReturnTodoItem()
        {
            // Arrange
            var todoItem = new TodoItem { Id = 1, Title = "Test Task", Description = "Test Desc", IsComplete = false };

            // Set up the repository to return the item
            _mockRepository.Setup(repo => repo.GetByIdAsync(1)).ReturnsAsync(todoItem);

            // Act
            var result = await _todoService.GetTodoByIdAsync(1);

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(todoItem, result);
        }

        [Test]
        public void GetTodoByIdAsync_WhenItemDoesNotExist_ShouldThrowKeyNotFoundException()
        {
            // Arrange: Simulate the repository throwing the KeyNotFoundException
            _mockRepository
                .Setup(repo => repo.GetByIdAsync(It.IsAny<int>()))
                .ThrowsAsync(new TodoItemNotFoundException($"TodoItem with id 1 was not found."));

            // Act & Assert: Ensure the service method throws the KeyNotFoundException
            var ex = Assert.ThrowsAsync<TodoItemNotFoundException>(async () => await _todoService.GetTodoByIdAsync(1));

            // Verify the exception message
            Assert.That(ex.Message, Is.EqualTo("TodoItem with id 1 was not found."));
        }

        [Test]
        public async Task CreateTodoAsync_WhenCalled_ShouldReturnCreatedTodoItem()
        {
            // Arrange
            var todoItem = new TodoItem { Id = 1, Title = "New Task", Description = "New Desc", IsComplete = false };

            // Set up the repository to add the item
            _mockRepository.Setup(repo => repo.AddAsync(todoItem)).ReturnsAsync(todoItem);

            // Act
            var result = await _todoService.CreateTodoAsync(todoItem);

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(todoItem, result);
        }

        [Test]
        public async Task UpdateTodoAsync_WhenCalled_ShouldCallRepositoryUpdate()
        {
            // Arrange
            var todoItem = new TodoItem { Id = 1, Title = "Updated Task", Description = "Updated Desc", IsComplete = true };

            // Set up the repository to expect an update call
            _mockRepository.Setup(repo => repo.UpdateAsync(todoItem)).Returns(Task.CompletedTask);

            // Act
            await _todoService.UpdateTodoAsync(todoItem);

            // Assert
            _mockRepository.Verify(repo => repo.UpdateAsync(todoItem), Times.Once);
        }

        [Test]
        public async Task DeleteTodoAsync_WhenCalled_ShouldCallRepositoryDelete()
        {
            // Arrange
            var todoItemId = 1;

            // Set up the repository to expect a delete call
            _mockRepository.Setup(repo => repo.DeleteAsync(todoItemId)).Returns(Task.CompletedTask);

            // Act
            await _todoService.DeleteTodoAsync(todoItemId);

            // Assert
            _mockRepository.Verify(repo => repo.DeleteAsync(todoItemId), Times.Once);
        }

        [Test]
        public void DeleteTodoAsync_WhenItemDoesNotExist_ShouldThrowKeyNotFoundException()
        {
            // Arrange
            var todoItemId = 1;
            _mockRepository.Setup(repo => repo.DeleteAsync(It.IsAny<int>())).ThrowsAsync(new KeyNotFoundException("TodoItem with id 1 was not found."));

            // Act & Assert
            var ex = Assert.ThrowsAsync<KeyNotFoundException>(async () => await _todoService.DeleteTodoAsync(todoItemId));
            Assert.That(ex.Message, Is.EqualTo("TodoItem with id 1 was not found."));
        }
    }
}