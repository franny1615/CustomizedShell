using Inventory.API.Models;

namespace Inventory.API.Repositories;

public class InventoryRepository : BaseRepository, ICrudRepository<Models.Inventory>
{
    public async Task<RepoResult<DeleteResult>> Delete(int itemId, int companyId)
    {
        var result = new RepoResult<DeleteResult>();
        try
        {
            string query = $@"
declare 
@invId int = {itemId},
@companyId int = {companyId};

delete from inventory_image
where CompanyId = @companyId
and InventoryId = @invId;

delete from inventory
where CompanyId = @companyId
and Id = @invId;";
            await QueryAsync<object>(query);
            result.Data = DeleteResult.SuccesfullyDeleted;
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
    inventory.Id,
    inventory.CompanyId,
    inventory.[Description],
    status.[Description] as Status,
    Quantity,
    quantity_type.[Description] as QuantityType,
    inventory.Barcode,
    [location].[Description] as Location,
    LastEditedOn,
    CreatedOn,
    QtyTypeId,
    LocationId,
    StatusId
from inventory 
inner join [status] on status.Id = inventory.StatusId
inner join [location] on [location].Id = inventory.LocationId
inner join [quantity_type] on quantity_type.Id = inventory.QtyTypeId
where inventory.CompanyId = @companyId
and inventory.Id = @invId";
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
            string barcodeSearch = "";
            if (int.TryParse(request.Search, out int barcode))
            {
                barcodeSearch = $"and inventory.Barcode = {barcode}";
            }
            else
            {
                barcodeSearch = "and inventory.[Description] LIKE @search+'%'";
            }
            string _param = $@"
declare 
@companyId int = {companyId},
@search NVARCHAR(max) = '{request.Search}',
@page int = {request.Page},
@pageSize int = {request.PageSize};";
            string query = $@"
{_param}
select 
    inventory.Id,
    inventory.CompanyId,
    inventory.[Description],
    status.[Description] as Status,
    Quantity,
    quantity_type.[Description] as QuantityType,
    inventory.Barcode,
    [location].[Description] as Location,
    LastEditedOn,
    CreatedOn,
    QtyTypeId,
    LocationId,
    StatusId
from inventory 
inner join [status] on status.Id = inventory.StatusId
inner join [location] on [location].Id = inventory.LocationId
inner join [quantity_type] on quantity_type.Id = inventory.QtyTypeId
where inventory.CompanyId = @companyId
{barcodeSearch}
order by inventory.Id desc 
offset (@page * @pageSize) rows 
fetch next @pageSize rows only";
            var items = (await QueryAsync<Models.Inventory>(query)).ToList();
            string totalQuery = $@"
{_param}
select COUNT(*) 
from inventory
where inventory.CompanyId = @companyId
{barcodeSearch};";
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
@quantity int = {item.Quantity},
@barcode int = {item.Barcode},
@quantityType int = {item.QtyTypeID},
@status int = {item.StatusID},
@location int = '{item.LocationID}';

insert into inventory
(
    CompanyId,
    StatusId,
    LocationId,
    QtyTypeId,
    [Description],
    Quantity,
    Barcode,
    LastEditedOn,
    CreatedOn
)
values 
(
    @companyId,
    @status,
    @location,
    @quantityType,
    @description,
    @quantity,
    @barcode,
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
@invId int = {item.Id},
@companyId int = {companyId},
@description nvarchar(max) = '{item.Description}',
@quantity int = {item.Quantity},
@barcode int = {item.Barcode},
@quantityType int = {item.QtyTypeID},
@status int = {item.StatusID},
@location int = '{item.LocationID}';

update inventory set 
    [Description] = @description,
    StatusId = @status,
    Quantity = @quantity,
    QtyTypeId = @quantityType,
    Barcode = @barcode,
    LocationId = @location,
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
