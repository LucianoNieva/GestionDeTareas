namespace GestionDeTareas.Repository
{
    public interface IRepository<T> where T : class
    {
        Task<List<T>> GetAll();
        Task<T?> GetById(int id);
        Task<T> Add(T item);
        Task Update(T item);
        Task Delete(T item);
        Task<bool> Exist(int id);
        Task Save();
    }
}
