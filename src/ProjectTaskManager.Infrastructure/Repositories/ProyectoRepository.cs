using Microsoft.EntityFrameworkCore;
using ProjectTaskManager.Application.Interfaces;
using ProjectTaskManager.Domain.Entities;
using ProjectTaskManager.Infrastructure.Persistence;

namespace ProjectTaskManager.Infrastructure.Repositories;

public class ProyectoRepository(AppDbContext context) : IProyectoRepository
{
    public async Task<IEnumerable<Proyecto>> ObtenerTodosAsync() =>
        await context.Proyectos.AsNoTracking().ToListAsync();

    public async Task<Proyecto?> ObtenerPorIdAsync(Guid id) =>
        await context.Proyectos.FindAsync(id);

    public async Task<Proyecto?> ObtenerConTareasAsync(Guid id) =>
        await context.Proyectos.Include(p => p.Tareas)
            .FirstOrDefaultAsync(p => p.Id == id);

    public async Task AgregarAsync(Proyecto proyecto)
    {
        await context.Proyectos.AddAsync(proyecto);
    }

    public void Actualizar(Proyecto proyecto) =>
        context.Proyectos.Update(proyecto);

    public void Eliminar(Proyecto proyecto) =>
        context.Proyectos.Remove(proyecto);

    public async Task<bool> GuardarCambiosAsync() =>
        await context.SaveChangesAsync() >= 0;
}
