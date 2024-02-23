using Maui.Inventory.Api.Interfaces;
using Maui.Inventory.Api.Models;
using Maui.Inventory.Api.Utilities;

namespace Maui.Inventory.Api.Repositories;

public class QuantityTypeRepository : IQuantityTypeRepository
{
    #region GET LIST
    public async Task<APIResponse<PaginatedQueryResponse<QuantityType>>> GetAll(PaginatedRequest request, int adminId)
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
    Description
FROM quantity_types
WHERE AdminId = {adminId}
{searchQuery}";
        #endregion

        #region TOTAL
        string totalQuery = $@"
SELECT COUNT(*)
FROM ({query}) qtyTypes";
        #endregion

        APIResponse<PaginatedQueryResponse<QuantityType>> response = new();
        try
        {
            response.Data = new();
            response.Data.Total = (await SQLUtils.QueryAsync<int>(totalQuery)).First();

            query += $@"
ORDER BY Description DESC
OFFSET {request.Page * request.ItemsPerPage} ROWS
FETCH NEXT {request.ItemsPerPage} ROWS ONLY";

            response.Data.Items = (await SQLUtils.QueryAsync<QuantityType>(query)).ToList();
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
    public async Task<APIResponse<bool>> Insert(QuantityType quantityType)
    {
        #region QUERY
        string query = $@"
INSERT INTO quantity_types
(
    AdminId,
    Description
)
VALUES
(
    {quantityType.AdminId},
    '{quantityType.Description}'
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
    #endregion

    #region UPDATE
    public async Task<APIResponse<bool>> Update(QuantityType quantityType)
    {
        #region QUERY
        string query = $@"
UPDATE quantity_types
SET
    Description = '{quantityType.Description}'
WHERE quantity_types.AdminId = {quantityType.AdminId}
AND quantity_types.Id = {quantityType.Id}";
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
    public async Task<APIResponse<bool>> Delete(QuantityType location)
    {
        #region QUERY
        string query = $@"
DELETE FROM quantity_types
WHERE quantity_types.AdminId = {location.AdminId}
AND quantity_types.Id = {location.Id}";
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
