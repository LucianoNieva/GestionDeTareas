using GestionDeTareas.DTO.Category;
using GestionDeTareas.Models;

namespace GestionDeTareas.DTO.Tarea
{
    public class TareaDetalleDTO
    {
        public int Id { get; set; }
        public string Titulo { get; set; } = null!;
        public Estado Estado { get; set; }
        public Prioridad Prioridad { get; set; }
        public DateTime FechaCreacion { get; set; }

        public CategoryDetailsDTO? Categoria { get; set; }
    }
}
