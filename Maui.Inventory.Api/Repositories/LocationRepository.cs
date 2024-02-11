using Maui.Inventory.Api.Interfaces;
using Maui.Inventory.Api.Models;
using Maui.Inventory.Api.Utilities;

namespace Maui.Inventory.Api.Repositories;

public class LocationRepository : ILocationRepository
{
    public async Task<APIResponse<PaginatedQueryResponse<Location>>> GetAll(
        PaginatedRequest request, 
        int adminId)
    {
        #region SEARCH
        string searchQuery = "";
        if (!string.IsNullOrEmpty(request.Search))
        {
            string[] words = request.Search.Split(" ", StringSplitOptions.RemoveEmptyEntries);
            foreach (var item in words)
            {
                searchQuery += $"AND Description LIKE '%{item}%'";
            }
        }
        #endregion

        #region QUERY
        string query = $@"
SELECT
    Id,
    AdminId,
    Description,
    Barcode
FROM locations
WHERE AdminId = {adminId}
{searchQuery}";
        #endregion

        #region TOTAL
        string totalQuery = $@"
SELECT COUNT(*)
FROM ({query}) locs";
        #endregion

        APIResponse<PaginatedQueryResponse<Location>> response = new();
        try
        {
            response.Data = new();
            response.Data.Total = (await SQLUtils.QueryAsync<int>(totalQuery)).First();

            query += $@"
ORDER BY Barcode DESC
OFFSET {request.Page * request.ItemsPerPage} ROWS
FETCH NEXT {request.ItemsPerPage} ROWS ONLY";

            response.Data.Items = (await SQLUtils.QueryAsync<Location>(query)).ToList();
        }
        catch (Exception ex)
        {
            response.Success = false;
            response.Message = $"ERROR >>> {ex.Message} <<<";
        }

        return response;
    }

    public async Task<APIResponse<bool>> Insert(Location location)
    {
        #region QUERY
        string query = $@"
INSERT INTO locations
(
    AdminId,
    Description,
    Barcode
)
VALUES
(
    {location.AdminId},
    '{location.Description}',
    '{location.Barcode}'
)";
        #endregion

        APIResponse<bool> response;
        try
        {
            await SQLUtils.QueryAsync<object>(query);
            response = new();
            response.Success = true;
            response.Data = true;
            response.Message = "query executed";
        }
        catch (Exception e)
        {
            response = new();
            response.Success = false;
            response.Data = false;
            response.Message = e.Message;
        }

        return response;
    }

    public async Task<APIResponse<bool>> Update(Location location)
    {
        #region QUERY
        string query = $@"
UPDATE locations
SET
    Description = '{location.Description}',
    Barcode = '{location.Barcode}'
WHERE locations.AdminId = {location.AdminId}
AND locations.Id = {location.Id}";
        #endregion

        APIResponse<bool> response;
        try
        {
            await SQLUtils.QueryAsync<object>(query);
            response = new();
            response.Success = true;
            response.Data = true;
            response.Message = "query executed";
        }
        catch (Exception e)
        {
            response = new();
            response.Success = false;
            response.Data = false;
            response.Message = e.Message;
        }

        return response;
    }

    public async Task<APIResponse<bool>> Delete(Location location)
    {
        #region QUERY
        string query = $@"
DELETE FROM locations
WHERE locations.AdminId = {location.AdminId}
AND locations.Id = {location.Id}";
        #endregion

        APIResponse<bool> response;
        try
        {
            await SQLUtils.QueryAsync<object>(query);
            response = new();
            response.Success = true;
            response.Data = true;
            response.Message = "query executed";
        }
        catch (Exception e)
        {
            response = new();
            response.Success = false;
            response.Data = false;
            response.Message = e.Message;
        }

        return response;
    }
}
