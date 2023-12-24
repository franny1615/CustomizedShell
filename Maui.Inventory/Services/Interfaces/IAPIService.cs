namespace Maui.Inventory.Services.Interfaces;

public interface IAPIService
{
    public Task<T> Get<T>(
        string endpoint,
        Dictionary<string, string> parameters) where T : new();

    public Task<T> Post<T>(
        string endpoint,
        object jsonParams) where T : new();
}
