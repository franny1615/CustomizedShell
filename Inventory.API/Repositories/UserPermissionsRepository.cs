using Inventory.API.Models;

namespace Inventory.API.Repositories;

public interface IPermissionsRepository : ICrudRepository<UserPermissions>
{
    Task<RepoResult<UserPermissions>> GetByUser(int userId, int companyId);
}

public class UserPermissionsRepository : BaseRepository, IPermissionsRepository
{
    public async Task<RepoResult<DeleteResult>> Delete(int itemId, int companyId)
    {
        var result = new RepoResult<DeleteResult>();
        try
        {
            string query = $@"
declare
@permId = {itemId},
@companyId = {companyId};

delete from user_permissions
where CompanyId = @companyId
and Id = @permId";

            await QueryAsync<object>(query);
            result.Data = DeleteResult.SuccesfullyDeleted;
        }
        catch (Exception ex) 
        {
            result.ErrorMessage = ex.ToString();
        }
        return result;
    }

    public async Task<RepoResult<UserPermissions>> Get(int id, int companyId)
    {
        var result = new RepoResult<UserPermissions>();
        try
        {
            string query = $@"
declare 
@permId int = {id},
@companyId int = {companyId};
select 
    Id,
    UserId,
    CompanyId,
    InventoryPermissions
from user_permissions
where Id = @permId
and CompanyId = @companyId";
            result.Data = (await QueryAsync<UserPermissions>(query)).First();
        }
        catch (Exception ex)
        {
            result.ErrorMessage = ex.ToString();
        }
        return result;
    }

    public async Task<RepoResult<SearchResult<UserPermissions>>> Get(
        SearchRequest request, 
        int companyId)
    {
        var result = new RepoResult<SearchResult<UserPermissions>>();
        try
        {
            string _params = $@"
declare 
@companyId int = {companyId},
@search NVARCHAR(max) = '{request.Search}',
@page int = {request.Page},
@pageSize int = {request.PageSize};";
            string query = $@"
{_params}
select 
    Id,
    UserId,
    CompanyId,
    InventoryPermissions
from user_permissions 
where CompanyId = @companyId
and [Description] LIKE @search+'%'
order by Id desc 
offset (@page * @pageSize) rows 
fetch next @pageSize rows only";
            var items = (await QueryAsync<UserPermissions>(query)).ToList();
            string totalQuery = $@"
{_params}
select COUNT(*) 
from user_permissions
where CompanyId = @companyId
and [Description] LIKE @search+'%';";
            var total = (await QueryAsync<int>(totalQuery)).First();

            result.Data = new()
            {
                Items = items,
                Total = total
            };
        }
        catch (Exception ex)
        {
            result.ErrorMessage = ex.ToString();
        }
        return result;
    }

    public async Task<RepoResult<int>> Insert(UserPermissions item, int companyId)
    {
        var result = new RepoResult<int>();
        try
        {
            string query = $@"
declare 
@companyId int = {companyId},
@userId    int = {item.UserId},
@invPerms  int = {item.InventoryPermissions};

insert into user_permissions
(
    CompanyId,
    UserId,
    InventoryPermissions
)
values 
(
    @companyId,
    @userId,
    @invPerms
);

select SCOPE_IDENTITY();";
            result.Data = (await QueryAsync<int>(query)).First();
        }
        catch (Exception ex)
        {
            result.ErrorMessage = ex.ToString();
        }
        return result;
    }

    public async Task<RepoResult<bool>> Update(UserPermissions item, int companyId)
    {
        var result = new RepoResult<bool>();
        try
        {
            string query = $@"
declare 
@userId    int = {item.UserId},
@companyId int = {companyId},
@invPerms  int = {item.InventoryPermissions};

update user_permissions set 
    InventoryPermissions = @invPerms
where CompanyId = @companyId
and UserId = @userId;";
            await QueryAsync<object>(query);
            result.Data = true;
        }
        catch (Exception ex)
        {
            result.ErrorMessage = ex.ToString();
        }
        return result;
    }

    public async Task<RepoResult<UserPermissions>> GetByUser(int userId, int companyId)
    {
        var result = new RepoResult<UserPermissions>();
        try
        {
            string query = $@"
declare 
@userId int = {userId},
@companyId int = {companyId};
select 
    Id,
    UserId,
    CompanyId,
    InventoryPermissions
from user_permissions
where UserId = @userId
and CompanyId = @companyId";
            result.Data = (await QueryAsync<UserPermissions>(query)).First();
        }
        catch (Exception ex)
        {
            result.ErrorMessage = ex.ToString();
        }
        return result;
    }
}
