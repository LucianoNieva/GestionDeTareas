using GestionDeTareas.Models;

namespace GestionDeTareas.DTO.Tarea
{
    public class TareaDTO
    {
        public int Id { get; set; }
        public string? Titulo { get; set; }
        public string? Estado { get; set; }
        public string? Prioridad { get; set; }
        public DateTime FechaCreacion { get; set; }
        public int? IdCategoria { get; set; }
        public string? CategoriaNombre { get; set; }
    }
}
