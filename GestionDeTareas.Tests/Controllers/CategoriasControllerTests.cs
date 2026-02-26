using GestionDeTareas.Controllers;
using GestionDeTareas.DTO.Category;
using GestionDeTareas.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;
using Xunit;

namespace GestionDeTareas.Tests.Controllers
{
    public class CategoriasControllerTests
    {
        private readonly Mock<ICategoryService> _mockCategoryService;
        private readonly Mock<ICurrentUserService> _mockCurrentUser;
        private readonly Mock<ILogger<CategoriasController>> _mockLogger;
        private readonly CategoriasController _controller;

        public CategoriasControllerTests()
        {
            _mockCategoryService = new Mock<ICategoryService>();
            _mockCurrentUser = new Mock<ICurrentUserService>();
            _mockLogger = new Mock<ILogger<CategoriasController>>();

            _controller = new CategoriasController(
                _mockCategoryService.Object,
                _mockCurrentUser.Object,
                _mockLogger.Object
            );
        }

        // ==========================================
        // GET /api/categorias
        // ==========================================

        [Fact]
        public async Task GetCategorias_DebeRetornarOk_ConListaDeCategorias()
        {
            // Arrange
            var categorias = new List<CategoryDTO>
            {
                new CategoryDTO { Id = 1, Nombre = "Trabajo" },
                new CategoryDTO { Id = 2, Nombre = "Personal" }
            };

            _mockCategoryService
                .Setup(s => s.ObtenerTodasLasCategorias())
                .ReturnsAsync(categorias);

            // Act
            var resultado = await _controller.GetCategorias();

            // Assert
            var okResult = resultado.Result as OkObjectResult;
            Assert.NotNull(okResult);
            Assert.Equal(200, okResult.StatusCode);

            var categoriasDevueltas = okResult.Value as List<CategoryDTO>;
            Assert.NotNull(categoriasDevueltas);
            Assert.Equal(2, categoriasDevueltas.Count);
        }

        [Fact]
        public async Task GetCategorias_DebeRetornarListaVacia_CuandoNoHayCategorias()
        {
            // Arrange
            _mockCategoryService
                .Setup(s => s.ObtenerTodasLasCategorias())
                .ReturnsAsync(new List<CategoryDTO>());

            // Act
            var resultado = await _controller.GetCategorias();

            // Assert
            var okResult = resultado.Result as OkObjectResult;
            Assert.NotNull(okResult);

            var categorias = okResult.Value as List<CategoryDTO>;
            Assert.NotNull(categorias);
            Assert.Empty(categorias);
        }

        // ==========================================
        // GET /api/categorias/{id}
        // ==========================================

        [Fact]
        public async Task GetCategoria_DebeRetornarOk_CuandoCategoriaExiste()
        {
            // Arrange
            var categoria = new CategoryDTO { Id = 1, Nombre = "Trabajo" };

            _mockCategoryService
                .Setup(s => s.ObtenerCategoriaPorId(1))
                .ReturnsAsync(categoria);

            // Act
            var resultado = await _controller.GetCategoria(1);

            // Assert
            var okResult = resultado.Result as OkObjectResult;
            Assert.NotNull(okResult);

            var categoriaDevuelta = okResult.Value as CategoryDTO;
            Assert.NotNull(categoriaDevuelta);
            Assert.Equal(1, categoriaDevuelta.Id);
            Assert.Equal("Trabajo", categoriaDevuelta.Nombre);
        }

        [Fact]
        public async Task GetCategoria_DebeRetornar404_CuandoCategoriaNoExiste()
        {
            // Arrange
            _mockCategoryService
                .Setup(s => s.ObtenerCategoriaPorId(999))
                .ReturnsAsync((CategoryDTO?)null);

            // Act
            var resultado = await _controller.GetCategoria(999);

            // Assert
            var notFoundResult = resultado.Result as NotFoundObjectResult;
            Assert.NotNull(notFoundResult);
            Assert.Equal(404, notFoundResult.StatusCode);
        }

        // ==========================================
        // GET /api/categorias/{id}/tarea
        // ==========================================

        [Fact]
        public async Task GetTareasDeCategoria_DebeRetornarOk_CuandoCategoriaExiste()
        {
            // Arrange
            var tareas = new
            {
                categoria = "Trabajo",
                cantidadTareas = 5,
                tareas = new[] { "Tarea 1", "Tarea 2" }
            };

            _mockCategoryService
                .Setup(s => s.ObtenerTareasDeCategoria(1))
                .ReturnsAsync(tareas);

            // Act
            var resultado = await _controller.GetTareasDeCategoria(1);

            // Assert
            var okResult = resultado as OkObjectResult;
            Assert.NotNull(okResult);
            Assert.Equal(200, okResult.StatusCode);
        }

        [Fact]
        public async Task GetTareasDeCategoria_DebeRetornar404_CuandoCategoriaNoExiste()
        {
            // Arrange
            _mockCategoryService
                .Setup(s => s.ObtenerTareasDeCategoria(999))
                .ReturnsAsync((object?)null);

            // Act
            var resultado = await _controller.GetTareasDeCategoria(999);

            // Assert
            var notFoundResult = resultado as NotFoundObjectResult;
            Assert.NotNull(notFoundResult);
            Assert.Equal(404, notFoundResult.StatusCode);
        }

        // ==========================================
        // POST /api/categorias (Solo Admin)
        // ==========================================

        [Fact]
        public async Task CrearCategoria_DebeRetornar201_CuandoCategoriaSeCreaBien()
        {
            // Arrange
            var dto = new CategoryCreacionDTO { Nombre = "Nueva Categoria" };
            var categoriaCreada = new CategoryDTO { Id = 1, Nombre = "Nueva Categoria" };

            _mockCategoryService
                .Setup(s => s.CrearCategoriaGlobal(dto))
                .ReturnsAsync((true, null, categoriaCreada));

            // Act
            var resultado = await _controller.CrearCategoria(dto);

            // Assert
            var createdResult = resultado.Result as CreatedAtRouteResult;
            Assert.NotNull(createdResult);
            Assert.Equal(201, createdResult.StatusCode);
            Assert.Equal("ObtenerCategoria", createdResult.RouteName);
        }

        [Fact]
        public async Task CrearCategoria_DebeRetornar400_CuandoNombreYaExiste()
        {
            // Arrange
            var dto = new CategoryCreacionDTO { Nombre = "Trabajo" };

            _mockCategoryService
                .Setup(s => s.CrearCategoriaGlobal(dto))
                .ReturnsAsync((false, "Ya existe una categoría con ese nombre", null));

            // Act
            var resultado = await _controller.CrearCategoria(dto);

            // Assert
            var badRequestResult = resultado.Result as BadRequestObjectResult;
            Assert.NotNull(badRequestResult);
            Assert.Equal(400, badRequestResult.StatusCode);
        }

        [Fact]
        public async Task CrearCategoria_DebeRetornar500_CuandoHayExcepcion()
        {
            // Arrange
            var dto = new CategoryCreacionDTO { Nombre = "Test" };

            _mockCategoryService
                .Setup(s => s.CrearCategoriaGlobal(dto))
                .ThrowsAsync(new Exception("Error inesperado"));

            // Act
            var resultado = await _controller.CrearCategoria(dto);

            // Assert
            var statusResult = resultado.Result as StatusCodeResult;
            Assert.NotNull(statusResult);
            Assert.Equal(500, statusResult.StatusCode);
        }

        // ==========================================
        // PUT /api/categorias/{id} (Solo Admin)
        // ==========================================

        [Fact]
        public async Task ActualizarCategoria_DebeRetornar204_CuandoSeActualizaBien()
        {
            // Arrange
            var dto = new CategoryCreacionDTO { Nombre = "Categoria Actualizada" };

            _mockCategoryService
                .Setup(s => s.ActualizarCategoria(1, dto))
                .ReturnsAsync((true, null));

            // Act
            var resultado = await _controller.ActualizarCategoria(1, dto);

            // Assert
            var noContentResult = resultado as NoContentResult;
            Assert.NotNull(noContentResult);
            Assert.Equal(204, noContentResult.StatusCode);
        }

        [Fact]
        public async Task ActualizarCategoria_DebeRetornar404_CuandoCategoriaNoExiste()
        {
            // Arrange
            var dto = new CategoryCreacionDTO { Nombre = "Test" };

            _mockCategoryService
                .Setup(s => s.ActualizarCategoria(999, dto))
                .ReturnsAsync((false, "Categoria no encontrada"));

            // Act
            var resultado = await _controller.ActualizarCategoria(999, dto);

            // Assert
            var notFoundResult = resultado as NotFoundObjectResult;
            Assert.NotNull(notFoundResult);
            Assert.Equal(404, notFoundResult.StatusCode);
        }

        // ==========================================
        // DELETE /api/categorias/{id} (Solo Admin)
        // ==========================================

        [Fact]
        public async Task EliminarCategoria_DebeRetornar204_CuandoSeEliminaBien()
        {
            // Arrange
            _mockCurrentUser.Setup(c => c.UserId).Returns("admin-1");

            _mockCategoryService
                .Setup(s => s.EliminarCategoria(1))
                .ReturnsAsync((true, null));

            // Act
            var resultado = await _controller.EliminarCategoria(1);

            // Assert
            var noContentResult = resultado as NoContentResult;
            Assert.NotNull(noContentResult);
            Assert.Equal(204, noContentResult.StatusCode);
        }

        [Fact]
        public async Task EliminarCategoria_DebeRetornar400_CuandoTieneTareasAsociadas()
        {
            // Arrange
            _mockCurrentUser.Setup(c => c.UserId).Returns("admin-1");

            _mockCategoryService
                .Setup(s => s.EliminarCategoria(1))
                .ReturnsAsync((false, "No se puede eliminar porque tiene 3 tarea(s) asociada(s)"));

            // Act
            var resultado = await _controller.EliminarCategoria(1);

            // Assert
            var badRequestResult = resultado as BadRequestObjectResult;
            Assert.NotNull(badRequestResult);
            Assert.Equal(400, badRequestResult.StatusCode);
        }
    }
}