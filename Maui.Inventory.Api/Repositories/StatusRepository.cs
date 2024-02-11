using Maui.Inventory.Api.Interfaces;
using Maui.Inventory.Api.Models;
using Maui.Inventory.Api.Utilities;

namespace Maui.Inventory.Api.Repositories;

public class StatusRepository : IStatusRepository
{
    public async Task<APIResponse<PaginatedQueryResponse<Status>>> GetAll(
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
    Description
FROM status
WHERE AdminId = {adminId}
{searchQuery}";
        #endregion

        #region TOTAL
        string totalQuery = $@"
SELECT COUNT(*)
FROM ({query}) status";
        #endregion

        APIResponse<PaginatedQueryResponse<Status>> response = new();
        try
        {
            response.Data = new();
            response.Data.Total = (await SQLUtils.QueryAsync<int>(totalQuery)).First();

            query += $@"
ORDER BY Id DESC
OFFSET {request.Page * request.ItemsPerPage} ROWS
FETCH NEXT {request.ItemsPerPage} ROWS ONLY";

            response.Data.Items = (await SQLUtils.QueryAsync<Status>(query)).ToList();
        }
        catch (Exception ex)
        {
            response.Success = false;
            response.Message = $"ERROR >>> {ex.Message} <<<";
        }

        return response;
    }

    public async Task<APIResponse<bool>> Insert(Status status)
    {
        #region QUERY
        string query = $@"
INSERT INTO status
(
    AdminId,
    Description
)
VALUES
(
    {status.AdminId},
    '{status.Description}'
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

    public async Task<APIResponse<bool>> Update(Status status)
    {
        #region QUERY
        string query = $@"
UPDATE status
SET
    Description = '{status.Description}'
WHERE status.Id = {status.Id}
AND status.AdminId = {status.AdminId}";
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

    public async Task<APIResponse<bool>> Delete(Status status)
    {
        #region QUERY
        string query = $@"
DELETE FROM status
WHERE status.Id = {status.Id}
AND status.AdminId = {status.AdminId}";
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
