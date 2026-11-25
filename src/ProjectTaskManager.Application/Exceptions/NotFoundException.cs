namespace ProjectTaskManager.Application.Exceptions;

public class NotFoundException(string mensaje) : Exception(mensaje);