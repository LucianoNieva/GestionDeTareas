using AutoMapper;
using GestionDeTareas.DTO.Tarea;
using GestionDeTareas.Models;
using GestionDeTareas.Repository;
using GestionDeTareas.Services;
using Moq;
using Xunit;

namespace GestionDeTareas.Tests.Services
{
    public class TareaServiceTests
    {
        private readonly Mock<ITareaRepository> _mockTareaRepo;
        private readonly Mock<ICategoryRepository> _mockCatRepo;
        private readonly Mock<IUsuarioService> _mockUserSvc;
        private readonly Mock<IMapper> _mockMapper;
        private readonly Mock<ICurrentUserService> _mockCurrentUser;
        private readonly TareaService _tareaService;

        public TareaServiceTests()
        {
            _mockTareaRepo = new Mock<ITareaRepository>();
            _mockCatRepo = new Mock<ICategoryRepository>();
            _mockUserSvc = new Mock<IUsuarioService>();
            _mockMapper = new Mock<IMapper>();
            _mockCurrentUser = new Mock<ICurrentUserService>();

            _tareaService = new TareaService(
                _mockTareaRepo.Object,
                _mockCatRepo.Object,
                _mockUserSvc.Object,
                _mockMapper.Object,
                _mockCurrentUser.Object
            );
        }

        // ==========================================
        // CASOS DE ÉXITO (Happy Path)
        // ==========================================

        [Fact]
        public async Task CrearTarea_DebeRetornarExito_CuandoDatosSonValidos()
        {
            // Arrange
            var creacionDto = new CreacionTareaDTO { Titulo = "Tarea Test", IdCategoria = 1, IdUsuario = "user-1" };

            // CORRECCIÓN: Agregamos Titulo porque es requerido
            var tareaEntidad = new Tarea { Id = 1, Titulo = "Tarea Test" };

            // CORRECCIÓN: Agregamos Nombre a la categoría porque es requerido
            _mockCatRepo.Setup(r => r.ObtenerPorId(1)).ReturnsAsync(new Category { Id = 1, Nombre = "Test" });

            _mockUserSvc.Setup(u => u.ExisteUsuarioAsync("user-1")).ReturnsAsync(true);
            _mockCurrentUser.Setup(c => c.UserId).Returns("admin-id");
            _mockMapper.Setup(m => m.Map<Tarea>(creacionDto)).Returns(tareaEntidad);
            _mockMapper.Setup(m => m.Map<TareaDTO>(tareaEntidad)).Returns(new TareaDTO { Id = 1 });

            // Act
            var (exito, error, resultado) = await _tareaService.CrearTarea(creacionDto);

            // Assert
            Assert.True(exito);
            Assert.Null(error);
        }

        [Fact]
        public async Task CompletarTarea_DebeCambiarEstado_CuandoTareaExiste()
        {
            // Arrange
            // CORRECCIÓN: Agregamos Titulo
            var tarea = new Tarea { Id = 1, Titulo = "Tarea a completar", Estado = Estado.Pendiente };
            _mockTareaRepo.Setup(r => r.ObtenerPorIdYUsuario(1, "user-1")).ReturnsAsync(tarea);

            // Act
            var (exito, error) = await _tareaService.CompletarTarea(1, "user-1");

            // Assert
            Assert.True(exito);
            Assert.Equal(Estado.Terminado, tarea.Estado);
        }

        [Fact]
        public async Task ObtenerTareasSegunRol_SiEsAdmin_DebeLlamarRepoConEsAdminTrue()
        {
            // Arrange
            string userId = "admin-123";
            _mockTareaRepo.Setup(r => r.ObtenerTareasSegunRol(userId, true))
                          .ReturnsAsync(new List<Tarea>());
            _mockMapper.Setup(m => m.Map<List<TareaDTO>>(It.IsAny<List<Tarea>>()))
                       .Returns(new List<TareaDTO>());

            // Act
            await _tareaService.ObtenerTareasSegunRol(userId, true);

            // Assert
            _mockTareaRepo.Verify(r => r.ObtenerTareasSegunRol(userId, true), Times.Once);
        }

        // ==========================================
        // CASOS DE ERROR / FALLO
        // ==========================================

        [Fact]
        public async Task CrearTarea_DebeRetornarError_SiCategoriaNoExiste()
        {
            // Arrange
            var dto = new CreacionTareaDTO { IdCategoria = 999, Titulo = "Falla" };
            _mockCatRepo.Setup(r => r.ObtenerPorId(999)).ReturnsAsync((Category)null!);

            // Act
            var (exito, error, resultado) = await _tareaService.CrearTarea(dto);

            // Assert
            Assert.False(exito);
            Assert.Equal("La categoria no existe", error);
            _mockTareaRepo.Verify(r => r.Add(It.IsAny<Tarea>()), Times.Never);
        }

        [Fact]
        public async Task EliminarTarea_DebeRetornarFalso_SiTareaNoExiste()
        {
            // Arrange
            _mockTareaRepo.Setup(r => r.ObtenerPorIdYUsuario(1, "user-1")).ReturnsAsync((Tarea)null!);

            // Act
            var (exito, error) = await _tareaService.EliminarTarea(1, "user-1");

            // Assert
            Assert.False(exito);
            Assert.Equal("Tarea no encontrada", error);
        }
    }
}