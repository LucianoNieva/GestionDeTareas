using AutoMapper;
using GestionDeTareas.DTO.Category;
using GestionDeTareas.DTO.Tarea;
using GestionDeTareas.DTO.Usuario;
using GestionDeTareas.Models;

namespace GestionDeTareas.Utilidades
{
    public class AutoMapperProfile: Profile
    {
        public AutoMapperProfile()
        {
            CreateMap<Tarea, TareaDTO>()
                .ForMember(dest => dest.Estado, config => config.MapFrom(src => src.Estado.ToString()))
                .ForMember(dest => dest.Prioridad, config => config.MapFrom(src => src.Prioridad.ToString()))
                .ForMember(dest => dest.CategoriaNombre,
                opt => opt.MapFrom(src => src.Categoria != null ? src.Categoria.Nombre : null));

            CreateMap<Tarea, TareaDetalleDTO>();


            CreateMap<CreacionTareaDTO, Tarea>()
                .ForMember(dest => dest.Prioridad, config => config.MapFrom(src => Enum.Parse<Prioridad>(src.Prioridad!)));
                

            CreateMap<ActualizarTareaDTO, Tarea>()
                .ForMember(dest => dest.Estado, config => config.MapFrom(src => Enum.Parse<Estado>(src.Estado!)))
                .ForMember(dest => dest.Prioridad, config => config.MapFrom(src => Enum.Parse<Prioridad>(src.Prioridad!)));

            CreateMap<Categoria, CategoryDTO>()
                .ForMember(dest => dest.CantidadTareas,
                    opt => opt.MapFrom(src => src.TareasEnCategoria != null
                                              ? src.TareasEnCategoria.Count
                                              : 0));
            CreateMap<Categoria, CategoryDetailsDTO>();
            CreateMap<CategoryCreacionDTO, Categoria>();

            CreateMap<Usuario, UsuarioDTO>()
                .ForMember(des => des.NombreApellido, config => config.MapFrom(usuario => $"{usuario.Nombre} {usuario.Apellido}")).ReverseMap();

            
        }
    }
}
