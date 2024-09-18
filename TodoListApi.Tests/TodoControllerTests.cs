using Microsoft.AspNetCore.Mvc;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TodoListApi.Controllers;
using TodoListApi.Models;
using TodoListApi.Services;

namespace TodoListApi.Tests
{
    [TestFixture]
    public class TodoControllerTests
    {
        private Mock<ITodoService> _mockTodoService;
        private TodoController _todoController;

        [SetUp]
        public void SetUp()
        {
            // Створюємо mock для ITodoService
            _mockTodoService = new Mock<ITodoService>();

            // Створюємо екземпляр TodoController з мокнутим ITodoService
            _todoController = new TodoController(_mockTodoService.Object);
        }

        [Test]
        public async Task GetAll_WhenCalled_ReturnsOkWithListOfTodos()
        {
            // Arrange
            var todoItems = new List<TodoItem>
            {
                new TodoItem { Id = 1, Title = "Task 1", Description = "Description 1", IsComplete = false },
                new TodoItem { Id = 2, Title = "Task 2", Description = "Description 2", IsComplete = true }
            };
            _mockTodoService.Setup(service => service.GetAllTodosAsync()).ReturnsAsync(todoItems);

            // Act
            var result = await _todoController.GetAll();

            // Assert
            Assert.IsInstanceOf<OkObjectResult>(result.Result); // Перевірка, що результат — це OkObjectResult
            var okResult = result.Result as OkObjectResult;
            Assert.AreEqual(todoItems, okResult.Value); // Перевірка, що повернутий список той, що очікується
        }

        [Test]
        public async Task Get_WhenItemExists_ReturnsOkWithTodoItem()
        {
            // Arrange
            var todoItem = new TodoItem { Id = 1, Title = "Task 1", Description = "Description 1", IsComplete = false };
            _mockTodoService.Setup(service => service.GetTodoByIdAsync(1)).ReturnsAsync(todoItem);

            // Act
            var result = await _todoController.Get(1);

            // Assert
            Assert.IsInstanceOf<OkObjectResult>(result.Result); // Перевірка, що результат — це OkObjectResult
            var okResult = result.Result as OkObjectResult;
            Assert.AreEqual(todoItem, okResult.Value); // Перевірка, що повернутий елемент той, що очікується
        }

        [Test]
        public async Task Get_WhenItemDoesNotExist_ReturnsNotFound()
        {
            // Arrange
            _mockTodoService.Setup(service => service.GetTodoByIdAsync(1)).ThrowsAsync(new KeyNotFoundException("TodoItem with id 1 was not found."));

            // Act
            var result = await _todoController.Get(1);

            // Assert
            Assert.IsInstanceOf<NotFoundObjectResult>(result.Result); // Перевірка, що результат — це NotFoundObjectResult
            var notFoundResult = result.Result as NotFoundObjectResult;
            Assert.AreEqual("TodoItem with id 1 was not found.", notFoundResult.Value); // Перевірка, що повідомлення про помилку правильне
        }

        [Test]
        public async Task Create_WhenValidTodoItem_ReturnsCreatedAtAction()
        {
            // Arrange
            var todoItem = new TodoItem { Id = 1, Title = "New Task", Description = "New Description", IsComplete = false };
            _mockTodoService.Setup(service => service.CreateTodoAsync(todoItem)).ReturnsAsync(todoItem);

            // Act
            var result = await _todoController.Create(todoItem);

            // Assert
            Assert.IsInstanceOf<CreatedAtActionResult>(result.Result); // Перевірка, що результат — це CreatedAtActionResult
            var createdResult = result.Result as CreatedAtActionResult;
            Assert.AreEqual(todoItem, createdResult.Value); // Перевірка, що створений елемент той, що очікується
        }

        [Test]
        public async Task Update_WhenIdsMatch_ReturnsNoContent()
        {
            // Arrange
            var todoItem = new TodoItem { Id = 1, Title = "Updated Task", Description = "Updated Description", IsComplete = false };
            _mockTodoService.Setup(service => service.UpdateTodoAsync(todoItem)).Returns(Task.CompletedTask);

            // Act
            var result = await _todoController.Update(1, todoItem);

            // Assert
            Assert.IsInstanceOf<NoContentResult>(result); // Перевірка, що результат — це NoContentResult
        }

        [Test]
        public async Task Update_WhenIdsDoNotMatch_ReturnsBadRequest()
        {
            // Arrange
            var todoItem = new TodoItem { Id = 1, Title = "Updated Task", Description = "Updated Description", IsComplete = false };

            // Act
            var result = await _todoController.Update(2, todoItem);

            // Assert
            Assert.IsInstanceOf<BadRequestResult>(result); // Перевірка, що результат — це BadRequestResult
        }

        [Test]
        public async Task Delete_WhenCalled_ReturnsNoContent()
        {
            // Arrange
            _mockTodoService.Setup(service => service.DeleteTodoAsync(1)).Returns(Task.CompletedTask);

            // Act
            var result = await _todoController.Delete(1);

            // Assert
            Assert.IsInstanceOf<NoContentResult>(result); // Перевірка, що результат — це NoContentResult
        }
    }
}
