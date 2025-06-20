namespace TaskListApi.Exceptions;

public class ForbiddenOrNotFound(string? message = null) : Exception(message);
