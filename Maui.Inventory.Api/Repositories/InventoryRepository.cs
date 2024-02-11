using Maui.Inventory.Api.Interfaces;
using Maui.Inventory.Api.Models;
using Maui.Inventory.Api.Utilities;

namespace Maui.Inventory.Api.Repositories;

public class InventoryRepository : IInventoryRepository
{
    #region GET INVENTORY
    public async Task<APIResponse<PaginatedQueryResponse<Models.Inventory>>> GetAll(
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
                searchQuery += $"AND (Description LIKE '%{item}%' OR Status LIKE '%{item}%' OR Quantity LIKE '%{item}%' OR Barcode LIKE '%{item}%' OR Location LIKE '%{item}%')";
            }
        }
        #endregion

        #region QUERY
        string query = $@"
SELECT
	Id,
	AdminId,
	Description,
	Status,
	Quantity,
	Barcode,
	Location,
	LastEditedOn,
	CreatedOn
FROM inventory
WHERE AdminId = {adminId}
{searchQuery}";
        #endregion

        #region TOTAL
        string totalQuery = $@"
SELECT COUNT(*)
FROM ({query}) inventory";
        #endregion

        APIResponse<PaginatedQueryResponse<Models.Inventory>> response = new();
        try
        {
            response.Data = new();
            response.Data.Total = (await SQLUtils.QueryAsync<int>(totalQuery)).First();

            query += $@"
ORDER BY LastEditedOn DESC
OFFSET {request.Page * request.ItemsPerPage} ROWS
FETCH NEXT {request.ItemsPerPage} ROWS ONLY";

            response.Data.Items = (await SQLUtils.QueryAsync<Models.Inventory>(query)).ToList();
        }
        catch (Exception ex)
        {
            response.Success = false;
            response.Message = $"ERROR >>> {ex.Message} <<<";
        }

        return response;
    }
    #endregion

    #region INSERT
    public async Task<APIResponse<bool>> Insert(Models.Inventory inventory)
    {
        #region QUERY
        string query = $@"
INSERT INTO inventory 
(
	AdminId,
	Description,
	Status,
	Quantity,
	Barcode,
	Location
)
VALUES
(
    {inventory.AdminId},
    '{inventory.Description}',
    '{inventory.Status}',
    {inventory.Quantity},
    '{inventory.Barcode}',
    '{inventory.Location}'
);";
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
    #endregion

    #region UPDATE
    public async Task<APIResponse<bool>> Update(Models.Inventory inventory)
    {
        #region QUERY
        string query = $@"
UPDATE inventory
SET
    Description = '{inventory.Description}',
    Status = '{inventory.Status}',
    Quantity = {inventory.Quantity},
    Barcode = '{inventory.Barcode}',
    Location = '{inventory.Location}',
    LastEditedOn = GETDATE()
WHERE inventory.AdminId = {inventory.AdminId}
AND   inventory.Id = {inventory.Id}";
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
    #endregion

    #region DELETE
    public async Task<APIResponse<bool>> Delete(Models.Inventory inventory)
    {
        #region QUERY
        string query = $@"
DELETE FROM inventory
WHERE inventory.Id = {inventory.Id}
AND   inventory.AdminId = {inventory.AdminId}";
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
    #endregion
}
