namespace Todo.Api.Domain.Todos.Exceptions;

public class UnsupportedColourException(string code) : Exception($"Colour \"{code}\" is unsupported.")
{
}