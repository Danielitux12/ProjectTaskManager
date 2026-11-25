using ProjectTaskManager.Domain.Enums;

namespace ProjectTaskManager.Domain.Entities;

public class Tarea
{
    public Guid Id { get; set; } = Guid.CreateVersion7();
    public string Titulo { get; set; } = string.Empty;
    public string? Descripcion { get; set; }
    public EstadoTarea Estado { get; set; } = EstadoTarea.Pendiente;
    public Prioridad Prioridad { get; set; } = Prioridad.Media;
    public DateTime FechaLimite { get; set; }

    // FK
    public Guid ProyectoId { get; set; }
    public Proyecto? Proyecto { get; set; }
}
