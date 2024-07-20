using Inventory.API.Models;

namespace Inventory.API.Repositories;

public class InventoryImageRepository : BaseRepository, ICrudRepository<InventoryImage>
{
    public async Task<RepoResult<DeleteResult>> Delete(int itemId, int companyId)
    {
        var result = new RepoResult<DeleteResult>();
        try
        {
            string query = $@"
declare 
@id int = {itemId},
@companyId int = {companyId};

delete from inventory_image
where CompanyId = @companyId
and Id = @id;";
            await QueryAsync<object>(query);
            result.Data = DeleteResult.SuccesfullyDeleted;
        }
        catch (Exception ex)
        {
            result.ErrorMessage = ex.ToString();
        }
        return result;
    }

    public async Task<RepoResult<InventoryImage>> Get(int id, int companyId)
    {
        var result = new RepoResult<InventoryImage>();
        try
        {
            string query = $@"
declare 
@id int = {id},
@companyId int = {companyId};

select 
    Id,
    CompanyId,
    InventoryId,
    ImageBase64,
    CreatedOn
from inventory_image
where Id = @id
and CompanyId = @companyId";
            result.Data = (await QueryAsync<InventoryImage>(query)).First();
        }
        catch (Exception ex)
        {
            result.ErrorMessage = ex.ToString();
        }
        return result;
    }

    public async Task<RepoResult<SearchResult<InventoryImage>>> Get(
        SearchRequest request,
        int companyId)
    {
        var result = new RepoResult<SearchResult<InventoryImage>>();
        try
        {
            string _params = $@"
declare 
@invID int = {request.InventoryItemID},
@companyId int = {companyId},
@search NVARCHAR(max) = '{request.Search}',
@page int = {request.Page},
@pageSize int = {request.PageSize};";
            string query = $@"
{_params}
select 
    Id,
    CompanyId,
    InventoryId,
    ImageBase64,
    CreatedOn 
from inventory_image
where CompanyId = @companyId
and InventoryId = @invID
order by Id desc 
offset (@page * @pageSize) rows 
fetch next @pageSize rows only";
            var items = (await QueryAsync<InventoryImage>(query)).ToList();
            string totalQuery = $@"
{_params}
select COUNT(*) 
from inventory_image
where CompanyId = @companyId
and InventoryId = @invID";
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

    public async Task<RepoResult<int>> Insert(InventoryImage item, int companyId)
    {
        var result = new RepoResult<int>();
        try
        {
            string query = $@"
declare 
@invId int = {item.InventoryId},
@companyId int = {companyId},
@img nvarchar(max) = '{item.ImageBase64}';

insert into inventory_image
(
    CompanyId,    
    InventoryId,
    ImageBase64,
    CreatedOn
)
values 
(
    @companyId,
    @invId,
    @img,
    GETDATE()
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

    public async Task<RepoResult<bool>> Update(InventoryImage item, int companyId)
    {
        var result = new RepoResult<bool>();
        try
        {
            string query = $@"
declare 
@id int = {item.Id},
@invId int = {item.InventoryId},
@companyId int = {companyId},
@img nvarchar(max) = '{item.ImageBase64}';

update inventory_image set 
    ImageBase64 = @img
where CompanyId = @companyId
and InventoryId = @invId
and Id = @id;";
            await QueryAsync<object>(query);
            result.Data = true;
        }
        catch (Exception ex)
        {
            result.ErrorMessage = ex.ToString();
        }
        return result;
    }
}
