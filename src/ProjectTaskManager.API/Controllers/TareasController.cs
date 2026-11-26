using Microsoft.AspNetCore.Mvc;
using ProjectTaskManager.Application.DTOs;
using ProjectTaskManager.Application.Interfaces;
using ProjectTaskManager.Domain.Enums;

namespace ProjectTaskManager.API.Controllers;

/// <summary>
/// Gestión de tareas.
/// </summary>
[ApiController]
[Route("api/v1/tareas")]
public class TareasController(ITareaService tareaService) : ControllerBase
{
    /// <summary>Lista tareas, con filtros opcionales por estado y prioridad.</summary>
    [HttpGet]
    public async Task<ActionResult<IEnumerable<TareaDto>>> ObtenerConFiltros(
        [FromQuery] EstadoTarea? estado, [FromQuery] Prioridad? prioridad)
        => Ok(await tareaService.ObtenerConFiltrosAsync(estado, prioridad));

    /// <summary>Lista las tareas vencidas que no están completadas.</summary>
    [HttpGet("vencidas")]
    public async Task<ActionResult<IEnumerable<TareaDto>>> ObtenerVencidas()
        => Ok(await tareaService.ObtenerVencidasAsync());

    /// <summary>Obtiene una tarea por su id.</summary>
    [HttpGet("{id:guid}")]
    public async Task<ActionResult<TareaDto>> ObtenerPorId(Guid id)
        => Ok(await tareaService.ObtenerPorIdAsync(id));

    /// <summary>Crea una nueva tarea (nace en estado Pendiente).</summary>
    [HttpPost]
    public async Task<ActionResult<TareaDto>> Crear([FromBody] CrearTareaDto dto)
    {
        var creada = await tareaService.CrearAsync(dto);
        return CreatedAtAction(nameof(ObtenerPorId), new { id = creada.Id }, creada);
    }

    /// <summary>Actualiza los datos de una tarea existente.</summary>
    [HttpPut("{id:guid}")]
    public async Task<IActionResult> Actualizar(Guid id, [FromBody] CrearTareaDto dto)
    {
        await tareaService.ActualizarAsync(id, dto);
        return NoContent();
    }

    /// <summary>Cambia el estado de una tarea (operación independiente del resto del CRUD).</summary>
    [HttpPatch("{id:guid}/estado")]
    public async Task<IActionResult> CambiarEstado(Guid id, [FromBody] CambiarEstadoTareaDto dto)
    {
        await tareaService.CambiarEstadoAsync(id, dto.NuevoEstado);
        return NoContent();
    }

    /// <summary>Elimina una tarea.</summary>
    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> Eliminar(Guid id)
    {
        await tareaService.EliminarAsync(id);
        return NoContent();
    }
}