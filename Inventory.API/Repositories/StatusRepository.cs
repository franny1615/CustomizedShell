using Inventory.API.Models;

namespace Inventory.API.Repositories;

public class StatusRepository : BaseRepository, ICrudRepository<Status>
{
    public async Task<RepoResult<bool>> Delete(int itemId, int companyId)
    {
        var result = new RepoResult<bool>();
        try 
        {
            string query = $@"
declare 
@statusId int = {itemId},
@companyId int = {companyId};

delete from status 
where Id = @statusId
and CompanyId = @companyId;";
            await QueryAsync<object>(query);
            result.Data = true;
        }
        catch (Exception ex)
        {
            result.ErrorMessage = ex.Message;
        }
        return result;
    }

    public async Task<RepoResult<Status>> Get(int id, int companyId)
    {
        var result = new RepoResult<Status>();
        try
        {
            string query = $@"
declare 
@statusId int = {id};
select 
    Id,
    CompanyId,
    Description
from status
where Id = @statusId;";
            result.Data = (await QueryAsync<Status>(query)).First();
        }
        catch (Exception ex)
        {
            result.ErrorMessage = ex.Message;
        }
        return result;
    }

    public async Task<RepoResult<SearchResult<Status>>> Get(SearchRequest request, int companyId)
    {
        var result = new RepoResult<SearchResult<Status>>();
        try 
        {
            string itemsQuery = $@"
declare 
@companyId int = {companyId},
@search NVARCHAR(max) = '{request.Search}',
@page int = {request.Page},
@pageSize int = {request.PageSize};
select 
    Id,
    CompanyId,
    Description
from status 
where CompanyId = @companyId
and [Description] LIKE @search+'%'
order by Id desc 
offset (@page * @pageSize) rows 
fetch next @pageSize rows only";
            var items = (await QueryAsync<Status>(itemsQuery)).ToList();
            string totalQuery = $@"select COUNT(*) from status;";
            var total = (await QueryAsync<int>(totalQuery)).First();

            result.Data = new() { Items = items, Total = total };
        }
        catch (Exception ex)
        {
            result.ErrorMessage = ex.Message;
        }
        return result;
    }

    public async Task<RepoResult<int>> Insert(Status item, int companyId)
    {
        var result = new RepoResult<int>();
        try 
        {
            string query = $@"
declare 
@companyId int={companyId},
@description nvarchar(max)='{item.Description}';

insert into status 
(
    CompanyId,
    Description 
)
values 
(
    @companyId,
    @description
);

select SCOPE_IDENTITY();";
            result.Data = (await QueryAsync<int>(query)).First();
        }
        catch (Exception ex)
        {
            result.ErrorMessage = ex.Message;
        }
        return result;
    }

    public async Task<RepoResult<bool>> Update(Status item, int companyId)
    {
        var result = new RepoResult<bool>();
        try 
        {
            string query = $@"
declare 
@statusId int = {item.Id},
@companyId int = {companyId},
@description nvarchar(max) = '{item.Description}';

update status set 
status.[Description] = @description
where Id = @statusId
and CompanyId = @companyId;";
            await QueryAsync<object>(query);
            result.Data = true;
        }
        catch (Exception ex)
        {
            result.ErrorMessage = ex.Message;
        }
        return result;
    }
}