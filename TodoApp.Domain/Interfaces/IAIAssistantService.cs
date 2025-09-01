namespace TodoApp.Domain.Interfaces;

public interface IAIAssistantService
{
    Task<List<string>> SuggestTasksAsync(string userInput);
} 