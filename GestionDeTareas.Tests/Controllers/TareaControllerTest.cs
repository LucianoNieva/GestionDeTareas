using GestionDeTareas.Controllers;
using GestionDeTareas.DTO.Tarea;
using GestionDeTareas.Models;
using GestionDeTareas.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;
using Xunit;

namespace GestionDeTareas.Tests.Controllers
{
    public class TareaControllerTests
    {
        private readonly Mock<ITareaService> _mockTareaService;
        private readonly Mock<ICurrentUserService> _mockCurrentUser;
        private readonly Mock<ILogger<TareaController>> _mockLogger;
        private readonly Mock<IUsuarioService> _mockUsuarioService;
        private readonly TareaController _controller;

        public TareaControllerTests()
        {
            _mockTareaService = new Mock<ITareaService>();
            _mockCurrentUser = new Mock<ICurrentUserService>();
            _mockLogger = new Mock<ILogger<TareaController>>();
            _mockUsuarioService = new Mock<IUsuarioService>();

            _controller = new TareaController(
                _mockTareaService.Object,
                _mockCurrentUser.Object,
                _mockLogger.Object,
                _mockUsuarioService.Object
            );
        }

        // ==========================================
        // GET /api/tarea
        // ==========================================

        [Fact]
        public async Task GetMisTareas_DebeRetornarOk_ConListaDeTareas()
        {
            // Arrange
            var tareas = new List<TareaDTO>
            {
                new TareaDTO { Id = 1, Titulo = "Tarea 1" },
                new TareaDTO { Id = 2, Titulo = "Tarea 2" }
            };

            _mockCurrentUser.Setup(c => c.UserId).Returns("user-1");
            _mockCurrentUser.Setup(c => c.IsInRole("Admin")).Returns(false);

            _mockTareaService
                .Setup(s => s.ObtenerTareasSegunRol("user-1", false))
                .ReturnsAsync(tareas);

            // Act
            var resultado = await _controller.GetMisTareas();

            // Assert
            var okResult = resultado.Result as OkObjectResult;
            Assert.NotNull(okResult);
            Assert.Equal(200, okResult.StatusCode);

            var tareasDevueltas = okResult.Value as List<TareaDTO>;
            Assert.NotNull(tareasDevueltas);
            Assert.Equal(2, tareasDevueltas.Count);
        }

        [Fact]
        public async Task GetMisTareas_AdminDebeVerTodasLasTareas()
        {
            // Arrange
            var todasLasTareas = new List<TareaDTO>
            {
                new TareaDTO { Id = 1, Titulo = "Tarea Admin" },
                new TareaDTO { Id = 2, Titulo = "Tarea Usuario 1" },
                new TareaDTO { Id = 3, Titulo = "Tarea Usuario 2" }
            };

            _mockCurrentUser.Setup(c => c.UserId).Returns("admin-1");
            _mockCurrentUser.Setup(c => c.IsInRole("Admin")).Returns(true);

            _mockTareaService
                .Setup(s => s.ObtenerTareasSegunRol("admin-1", true))
                .ReturnsAsync(todasLasTareas);

            // Act
            var resultado = await _controller.GetMisTareas();

            // Assert
            var okResult = resultado.Result as OkObjectResult;
            Assert.NotNull(okResult);

            var tareas = okResult.Value as List<TareaDTO>;
            Assert.Equal(3, tareas!.Count);
        }

        // ==========================================
        // GET /api/tarea/{id}
        // ==========================================

        [Fact]
        public async Task GetTarea_DebeRetornarOk_CuandoTareaExiste()
        {
            // Arrange
            var tarea = new TareaDetalleDTO
            {
                Id = 1,
                Titulo = "Tarea Test",
                Estado = Estado.Pendiente
            };

            _mockCurrentUser.Setup(c => c.UserId).Returns("user-1");

            _mockTareaService
                .Setup(s => s.ObtenerTareaPorId(1, "user-1"))
                .ReturnsAsync(tarea);

            // Act
            var resultado = await _controller.GetTarea(1);

            // Assert
            var okResult = resultado.Result as OkObjectResult;
            Assert.NotNull(okResult);
            Assert.Equal(200, okResult.StatusCode);

            var tareaDevuelta = okResult.Value as TareaDetalleDTO;
            Assert.NotNull(tareaDevuelta);
            Assert.Equal(1, tareaDevuelta.Id);
            Assert.Equal("Tarea Test", tareaDevuelta.Titulo);
        }

        [Fact]
        public async Task GetTarea_DebeRetornar404_CuandoTareaNoExiste()
        {
            // Arrange
            _mockCurrentUser.Setup(c => c.UserId).Returns("user-1");

            _mockTareaService
                .Setup(s => s.ObtenerTareaPorId(999, "user-1"))
                .ReturnsAsync((TareaDetalleDTO?)null);

            // Act
            var resultado = await _controller.GetTarea(999);

            // Assert
            var notFoundResult = resultado.Result as NotFoundObjectResult;
            Assert.NotNull(notFoundResult);
            Assert.Equal(404, notFoundResult.StatusCode);
        }

        // ==========================================
        // POST /api/tarea (Solo Admin)
        // ==========================================

        [Fact]
        public async Task CrearTarea_DebeRetornar201_CuandoTareaSeCreaBien()
        {
            // Arrange
            var dto = new CreacionTareaDTO { Titulo = "Nueva tarea" };
            var tareaCreada = new TareaDTO { Id = 1, Titulo = "Nueva tarea" };

            _mockCurrentUser.Setup(c => c.UserId).Returns("admin-1");
            _mockCurrentUser.Setup(c => c.Email).Returns("admin@test.com");

            _mockTareaService
                .Setup(s => s.CrearTarea(dto))
                .ReturnsAsync((true, null, tareaCreada));

            // Act
            var resultado = await _controller.CrearTarea(dto);

            // Assert
            var createdResult = resultado as CreatedAtRouteResult;
            Assert.NotNull(createdResult);
            Assert.Equal(201, createdResult.StatusCode);
            Assert.Equal("ObtenerTarea", createdResult.RouteName);

            var tarea = createdResult.Value as TareaDTO;
            Assert.NotNull(tarea);
            Assert.Equal(1, tarea.Id);
        }

        [Fact]
        public async Task CrearTarea_DebeRetornar400_CuandoFallaValidacion()
        {
            // Arrange
            var dto = new CreacionTareaDTO { Titulo = "Test" };

            _mockCurrentUser.Setup(c => c.UserId).Returns("admin-1");

            _mockTareaService
                .Setup(s => s.CrearTarea(dto))
                .ReturnsAsync((false, "La categoria no existe", null));

            // Act
            var resultado = await _controller.CrearTarea(dto);

            // Assert
            var badRequestResult = resultado as BadRequestObjectResult;
            Assert.NotNull(badRequestResult);
            Assert.Equal(400, badRequestResult.StatusCode);
        }

        [Fact]
        public async Task CrearTarea_DebeRetornar500_CuandoHayExcepcion()
        {
            // Arrange
            var dto = new CreacionTareaDTO { Titulo = "Test" };

            _mockCurrentUser.Setup(c => c.UserId).Returns("admin-1");

            _mockTareaService
                .Setup(s => s.CrearTarea(dto))
                .ThrowsAsync(new Exception("Error inesperado"));

            // Act
            var resultado = await _controller.CrearTarea(dto);

            // Assert
            var statusResult = resultado as StatusCodeResult;
            Assert.NotNull(statusResult);
            Assert.Equal(500, statusResult.StatusCode);
        }

        // ==========================================
        // PUT /api/tarea/{id} (Solo Admin)
        // ==========================================

        [Fact]
        public async Task ActualizarTarea_DebeRetornar204_CuandoSeActualizaBien()
        {
            // Arrange
            var dto = new ActualizarTareaDTO { Titulo = "Tarea actualizada" };

            _mockCurrentUser.Setup(c => c.UserId).Returns("admin-1");

            _mockTareaService
                .Setup(s => s.ActualizarTarea(1, dto, "admin-1"))
                .ReturnsAsync((true, null));

            // Act
            var resultado = await _controller.ActualizarTarea(1, dto);

            // Assert
            var noContentResult = resultado as NoContentResult;
            Assert.NotNull(noContentResult);
            Assert.Equal(204, noContentResult.StatusCode);
        }

        [Fact]
        public async Task ActualizarTarea_DebeRetornar404_CuandoTareaNoExiste()
        {
            // Arrange
            var dto = new ActualizarTareaDTO { Titulo = "Test" };

            _mockCurrentUser.Setup(c => c.UserId).Returns("admin-1");

            _mockTareaService
                .Setup(s => s.ActualizarTarea(999, dto, "admin-1"))
                .ReturnsAsync((false, "Tarea no encontrada"));

            // Act
            var resultado = await _controller.ActualizarTarea(999, dto);

            // Assert
            var notFoundResult = resultado as NotFoundObjectResult;
            Assert.NotNull(notFoundResult);
            Assert.Equal(404, notFoundResult.StatusCode);
        }

        // ==========================================
        // DELETE /api/tarea/{id} (Solo Admin)
        // ==========================================

        [Fact]
        public async Task BorrarTarea_DebeRetornar204_CuandoSeEliminaBien()
        {
            // Arrange
            _mockCurrentUser.Setup(c => c.UserId).Returns("admin-1");

            _mockTareaService
                .Setup(s => s.EliminarTarea(1, "admin-1"))
                .ReturnsAsync((true, null));

            // Act
            var resultado = await _controller.BorrarTarea(1);

            // Assert
            var noContentResult = resultado as NoContentResult;
            Assert.NotNull(noContentResult);
            Assert.Equal(204, noContentResult.StatusCode);
        }

        [Fact]
        public async Task BorrarTarea_DebeRetornar404_CuandoTareaNoExiste()
        {
            // Arrange
            _mockCurrentUser.Setup(c => c.UserId).Returns("admin-1");

            _mockTareaService
                .Setup(s => s.EliminarTarea(999, "admin-1"))
                .ReturnsAsync((false, "Tarea no encontrada"));

            // Act
            var resultado = await _controller.BorrarTarea(999);

            // Assert
            var notFoundResult = resultado as NotFoundObjectResult;
            Assert.NotNull(notFoundResult);
            Assert.Equal(404, notFoundResult.StatusCode);
        }

        // ==========================================
        // PATCH /api/tarea/{id}/completar
        // ==========================================

        [Fact]
        public async Task CompletarTarea_DebeRetornar204_CuandoSeCompletaBien()
        {
            // Arrange
            _mockCurrentUser.Setup(c => c.UserId).Returns("user-1");

            _mockTareaService
                .Setup(s => s.CompletarTarea(1, "user-1"))
                .ReturnsAsync((true, null));

            // Act
            var resultado = await _controller.CompletarTarea(1);

            // Assert
            var noContentResult = resultado as NoContentResult;
            Assert.NotNull(noContentResult);
            Assert.Equal(204, noContentResult.StatusCode);
        }

        [Fact]
        public async Task CompletarTarea_DebeRetornar404_CuandoTareaNoExiste()
        {
            // Arrange
            _mockCurrentUser.Setup(c => c.UserId).Returns("user-1");

            _mockTareaService
                .Setup(s => s.CompletarTarea(999, "user-1"))
                .ReturnsAsync((false, "Tarea no encontrada"));

            // Act
            var resultado = await _controller.CompletarTarea(999);

            // Assert
            var notFoundResult = resultado as NotFoundObjectResult;
            Assert.NotNull(notFoundResult);
            Assert.Equal(404, notFoundResult.StatusCode);
        }

        // ==========================================
        // GET /api/tarea/filtrar
        // ==========================================

        [Fact]
        public async Task FiltrarTarea_DebeRetornarOk_ConTareasFiltradas()
        {
            // Arrange
            var tareasFiltradas = new List<TareaDTO>
            {
                new TareaDTO { Id = 1, Titulo = "Tarea Pendiente", Estado = "Pendiente" }
            };

            _mockCurrentUser.Setup(c => c.UserId).Returns("user-1");

            _mockTareaService
                .Setup(s => s.FiltrarTareas("user-1", Estado.Pendiente, null, null, null))
                .ReturnsAsync(tareasFiltradas);

            // Act
            var resultado = await _controller.FiltrarTarea(Estado.Pendiente, null, null, null);

            // Assert
            var okResult = resultado.Result as OkObjectResult;
            Assert.NotNull(okResult);

            var tareas = okResult.Value as List<TareaDTO>;
            Assert.NotNull(tareas);
            Assert.Single(tareas);
            Assert.Equal("Pendiente", tareas[0].Estado);
        }

        [Fact]
        public async Task FiltrarTarea_DebeRetornar500_CuandoHayExcepcion()
        {
            // Arrange
            _mockCurrentUser.Setup(c => c.UserId).Returns("user-1");

            _mockTareaService
                .Setup(s => s.FiltrarTareas("user-1", null, null, null, null))
                .ThrowsAsync(new Exception("Error en filtrado"));

            // Act
            var resultado = await _controller.FiltrarTarea(null, null, null, null);

            // Assert
            var statusResult = resultado.Result as StatusCodeResult;
            Assert.NotNull(statusResult);
            Assert.Equal(500, statusResult.StatusCode);
        }
    }
}