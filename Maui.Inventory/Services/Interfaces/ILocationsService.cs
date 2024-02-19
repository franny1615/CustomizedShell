namespace Maui.Inventory.Services.Interfaces;

public interface ILocationsService : ICRUDService<Models.Location>
{
    public Task<string> GenerateBarcode(string code);
}
