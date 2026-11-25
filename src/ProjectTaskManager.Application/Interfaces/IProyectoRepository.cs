using ProjectTaskManager.Domain.Entities;

namespace ProjectTaskManager.Application.Interfaces;

public interface IProyectoRepository
{
    Task<IEnumerable<Proyecto>> ObtenerTodosAsync();
    Task<Proyecto?> ObtenerPorIdAsync(Guid id);
    Task<Proyecto?> ObtenerConTareasAsync(Guid id);
    Task AgregarAsync(Proyecto proyecto);
    void Actualizar(Proyecto proyecto);
    void Eliminar(Proyecto proyecto);
    Task<bool> GuardarCambiosAsync();
}
