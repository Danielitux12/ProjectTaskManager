using Microsoft.EntityFrameworkCore;
using ProjectTaskManager.Application.Interfaces;
using ProjectTaskManager.Domain.Entities;
using ProjectTaskManager.Domain.Enums;
using ProjectTaskManager.Infrastructure.Persistence;

namespace ProjectTaskManager.Infrastructure.Repositories;

public class TareaRepository(AppDbContext context) : ITareaRepository
{
    public async Task<IEnumerable<Tarea>> ObtenerPorProyectoAsync(Guid proyectoId) =>
        await context.Tareas.AsNoTracking()
            .Where(t => t.ProyectoId == proyectoId)
            .ToListAsync();

    public async Task<IEnumerable<Tarea>> ObtenerConFiltrosAsync(EstadoTarea? estado, Prioridad? prioridad)
    {
        var query = context.Tareas.AsNoTracking().AsQueryable();

        if (estado is not null)
            query = query.Where(t => t.Estado == estado);

        if (prioridad is not null)
            query = query.Where(t => t.Prioridad == prioridad);

        return await query.ToListAsync();
    }

    public async Task<IEnumerable<Tarea>> ObtenerVencidasAsync() =>
        await context.Tareas.AsNoTracking()
            .Where(t => t.FechaLimite < DateTime.UtcNow && t.Estado != EstadoTarea.Completada)
            .ToListAsync();

    public async Task<Tarea?> ObtenerPorIdAsync(Guid id) =>
        await context.Tareas.FindAsync(id);

    public async Task AgregarAsync(Tarea tarea)
    {
        await context.Tareas.AddAsync(tarea);
    }

    public void Actualizar(Tarea tarea) =>
        context.Tareas.Update(tarea);

    public void Eliminar(Tarea tarea) =>
        context.Tareas.Remove(tarea);

    public async Task<bool> GuardarCambiosAsync() =>
        await context.SaveChangesAsync() >= 0;
}
