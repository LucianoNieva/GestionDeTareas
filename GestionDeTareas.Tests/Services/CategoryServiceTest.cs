using AutoMapper;
using GestionDeTareas.DTO.Category;
using GestionDeTareas.Models;
using GestionDeTareas.Repository;
using GestionDeTareas.Services;
using Moq;

namespace GestionDeTareas.Tests.Services
{
    public class CategoryServiceTests
    {
        private readonly Mock<ICategoryRepository> _mockCatRepo;
        private readonly Mock<IMapper> _mockMapper;
        private readonly CategoryService _categoryService;

        public CategoryServiceTests()
        {
            _mockCatRepo = new Mock<ICategoryRepository>();
            _mockMapper = new Mock<IMapper>();
            _categoryService = new CategoryService(_mockCatRepo.Object, _mockMapper.Object);
        }

        [Fact]
        public async Task CrearCategoriaGlobal_DebeRetornarError_SiNombreYaExiste()
        {
            // Arrange
            var dto = new CategoryCreacionDTO { Nombre = "Estudio" };
            _mockCatRepo.Setup(r => r.Existe("Estudio")).ReturnsAsync(true); // Simulamos que ya existe

            // Act
            var (exito, error, categoria) = await _categoryService.CrearCategoriaGlobal(dto);

            // Assert
            Assert.False(exito);
            Assert.Equal("Ya existe una categoría con ese nombre", error);
            _mockCatRepo.Verify(r => r.Add(It.IsAny<Category>()), Times.Never); // Verificamos que NO se agregó
        }

        [Fact]
        public async Task ActualizarCategoria_DebeRetornarError_SiNombreYaExisteEnOtraCategoria()
        {
            // Arrange
            int idExistente = 1;
            var dto = new CategoryCreacionDTO { Nombre = "Trabajo" };
            var categoriaEnDb = new Category { Id = idExistente, Nombre = "Viejo Nombre" };

            _mockCatRepo.Setup(r => r.ObtenerPorId(idExistente)).ReturnsAsync(categoriaEnDb);
            _mockCatRepo.Setup(r => r.ExisteConNombreExcluyendo("Trabajo", idExistente)).ReturnsAsync(true);

            // Act
            var (exito, error) = await _categoryService.ActualizarCategoria(idExistente, dto);

            // Assert
            Assert.False(exito);
            Assert.Equal("Ya existe otra categoria con ese nombre", error);
        }

        [Fact]
        public async Task ActualizarCategoria_DebeRetornarError_SiCategoriaNoExiste()
        {
            // Arrange
            int idNoExistente = 999;
            var dto = new CategoryCreacionDTO { Nombre = "Nueva Categoria" };
            _mockCatRepo.Setup(r => r.ObtenerPorId(idNoExistente)).ReturnsAsync((Category?)null);
            // Act
            var (exito, error) = await _categoryService.ActualizarCategoria(idNoExistente, dto);
            // Assert
            Assert.False(exito);
            Assert.Equal("Categoria no encontrada", error);
        }

        [Fact]
        public async Task EliminarCategoria_DebeRetornarError_SiCategoriaNoExiste()
        {
            // Arrange
            int idNoExistente = 999;
            _mockCatRepo.Setup(r => r.ObtenerPorId(idNoExistente)).ReturnsAsync((Category?)null);
            // Act
            var (exito, error) = await _categoryService.EliminarCategoria(idNoExistente);
            // Assert
            Assert.False(exito);
            Assert.Equal("Categoria no encontrada", error);
        }

        [Fact]
        public async Task EliminarCategoria_DebeRetornarError_SiTieneTareasAsociadas()
        {
            // Arrange
            int catId = 10;
            var categoriaConTareas = new Category
            {
                Id = catId,
                Nombre = "Importante",
                // Le metemos una tarea para que el Any() de tu service devuelva true
                TareasEnCategoria = new List<Tarea> { new Tarea { Titulo = "Tarea 1" } }
            };

            _mockCatRepo.Setup(r => r.ObtenerConTareas(catId)).ReturnsAsync(categoriaConTareas);

            // Act
            var (exito, error) = await _categoryService.EliminarCategoria(catId);

            // Assert
            Assert.False(exito);
            Assert.Contains("No se puede eliminar porque tiene", error);
            _mockCatRepo.Verify(r => r.Delete(It.IsAny<Category>()), Times.Never);
        }

        [Fact]
        public async Task ObtenerCategoriaPorId_DebeRetornarDTO_CuandoExiste()
        {
            // Arrange
            int id = 1;
            var categoria = new Category { Id = id, Nombre = "Hogar" };
            var dto = new CategoryDTO { Id = id, Nombre = "Hogar" };

            _mockCatRepo.Setup(r => r.ObtenerPorId(id)).ReturnsAsync(categoria);
            _mockMapper.Setup(m => m.Map<CategoryDTO>(categoria)).Returns(dto);

            // Act
            var resultado = await _categoryService.ObtenerCategoriaPorId(id);

            // Assert
            Assert.NotNull(resultado);
            Assert.Equal("Hogar", resultado!.Nombre);
        }
    }

    

}
