using Inventory.API.Models;

namespace Inventory.API.Repositories;

public class InventoryRepository : BaseRepository, ICrudRepository<Models.Inventory>
{
    public async Task<RepoResult<bool>> Delete(int itemId, int companyId)
    {
        var result = new RepoResult<bool>();
        try
        {
            string query = $@"
declare 
@invId int = {itemId},
@companyId int = {companyId};

delete from inventory
where CompanyId = @companyId
and Id = @invId;";
            await QueryAsync<object>(query);
            result.Data = true;
        }
        catch (Exception ex)
        {
            result.ErrorMessage = ex.ToString();
        }
        return result;
    }

    public async Task<RepoResult<Models.Inventory>> Get(int id, int companyId)
    {
        var result = new RepoResult<Models.Inventory>();
        try
        {
            string query = $@"
declare 
@invId int = {id},
@companyId int = {companyId};

select 
    Id,
    CompanyId,
    Description,
    Status,
    Quantity,
    QuantityType,
    Barcode,
    Location,
    LastEditedOn,
    CreatedOn
from inventory
where Id = @invId
and CompanyId = @companyId";
            result.Data = (await QueryAsync<Models.Inventory>(query)).First();
        }
        catch (Exception ex)
        {
            result.ErrorMessage = ex.ToString();
        }
        return result;
    }

    public async Task<RepoResult<SearchResult<Models.Inventory>>> Get(SearchRequest request, int companyId)
    {
        var result = new RepoResult<SearchResult<Models.Inventory>>();
        try
        {
            string query = $@"
declare 
@companyId int = {companyId},
@search NVARCHAR(max) = '{request.Search}',
@page int = {request.Page},
@pageSize int = {request.PageSize};
select 
    Id,
    CompanyId,
    Description,
    Status,
    Quantity,
    QuantityType,
    Barcode,
    Location,
    LastEditedOn,
    CreatedOn
from inventory 
where CompanyId = @companyId
and [Description] LIKE @search+'%'
order by Id desc 
offset (@page * @pageSize) rows 
fetch next @pageSize rows only";
            var items = (await QueryAsync<Models.Inventory>(query)).ToList();
            string totalQuery = $@"select COUNT(*) from location;";
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

    public async Task<RepoResult<int>> Insert(Models.Inventory item, int companyId)
    {
        var result = new RepoResult<int>();
        try
        {
            string query = $@"
declare 
@companyId int = {companyId},
@description nvarchar(max) = '{item.Description}',
@status nvarchar(max) = '{item.Status}',
@quantity int = {item.Quantity},
@quantityType nvarchar(max) = '{item.QuantityType}',
@barcode nvarchar(max) = '{item.Barcode}',
@location nvarchar(max) = '{item.Location}';

insert into inventory
(
    CompanyId,
    [Description],
    Status,
    Quantity,
    QuantityType,
    Barcode,
    Location,
    LastEditedOn,
    CreatedOn
)
values 
(
    @companyId,
    @description,
    @status,
    @quantity,
    @quantityType,
    @barcode,
    @location,
    GETDATE(),
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

    public async Task<RepoResult<bool>> Update(Models.Inventory item, int companyId)
    {
        var result = new RepoResult<bool>();
        try
        {
            string query = $@"
declare 
@invId = {item.Id},
@description nvarchar(max) = '{item.Description}',
@status nvarchar(max) = '{item.Status}',
@quantity int = {item.Quantity},
@quantityType nvarchar(max) = '{item.QuantityType}',
@barcode nvarchar(max) = '{item.Barcode}',
@location nvarchar(max) = '{item.Location}';

update inventory set 
    [Description] = @description,
    Status = @status,
    Quantity = @quantity,
    QuantityType = @quantityType,
    Barcode = @barcode,
    Location = @location,
    LastEditedOn = GETDATE()
where CompanyId = @companyId
and Id = @invId;";
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
