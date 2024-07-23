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
            if (!string.IsNullOrEmpty(request.Search) && request.Search.Length > 5 && int.TryParse(request.Search, out int barcode))
            {
                barcodeSearch = $"and inventory.Barcode = {barcode}";
            }
            else
            {
                barcodeSearch = @"
where 
(
    [Description] LIKE @search+'%' or
    Status LIKE @search+'%' or 
    QuantityType LIKE @search+'%' or
    Quantity LIKE @search+'%' or
    Location LIKE @search+'%'
)";
            }
            
            string exactSearch = $@"";
            if (!string.IsNullOrEmpty(request.Location))
                exactSearch += $"\nand Location = '{request.Location}'";
            if (!string.IsNullOrEmpty(request.Status))
                exactSearch += $"\nand Status = '{request.Status}'";
            if (!string.IsNullOrEmpty(request.QuantityType))
                exactSearch += $"\nand QuantityType = '{request.QuantityType}'";

            string sortBy = "";
            switch ((InventorySortBy)request.SortBy)
            {
                case InventorySortBy.None:
                    sortBy = "order by Id desc";
                    break;
                case InventorySortBy.Quantity:
                    sortBy = "order by Quantity desc";
                    break;
                case InventorySortBy.QuantityType:
                    sortBy = "order by QuantityType desc";
                    break;
                case InventorySortBy.Barcode:
                    sortBy = "order by Barcode desc";
                    break;
                case InventorySortBy.Location:
                    sortBy = "order by Location desc";
                    break;
                case InventorySortBy.Status:
                    sortBy = "order by Status desc";
                    break;
                case InventorySortBy.CreatedDate:
                    sortBy = "order by CreatedDate desc";
                    break;
                case InventorySortBy.LastEditedDate:
                    sortBy = "order by LastEditedDate desc";
                    break;
            }

            string _param = $@"
declare 
@companyId int = {companyId},
@search NVARCHAR(max) = '{request.Search}',
@page int = {request.Page},
@pageSize int = {request.PageSize},
@count int = 0;

declare @invTable table(
	Id int, 
	CompanyId int, 
	Description nvarchar(max), 
	Status nvarchar(max), 
	Quantity int, 
	QuantityType nvarchar(max), 
	Barcode nvarchar(max),
	Location nvarchar(max),
	LastEditedOn DATETIME,
	CreatedOn DATETIME,
	QtyTypeId int,
	LocationId int,
	StatusId int
);

declare @invTableFiltered table(
	Id int, 
	CompanyId int, 
	Description nvarchar(max), 
	Status nvarchar(max), 
	Quantity int, 
	QuantityType nvarchar(max), 
	Barcode nvarchar(max),
	Location nvarchar(max),
	LastEditedOn DATETIME,
	CreatedOn DATETIME,
	QtyTypeId int,
	LocationId int,
	StatusId int
);

insert into @invTable 
select 
    inventory.Id as Id,
    inventory.CompanyId as CompanyId,
    inventory.[Description] as Description,
    status.[Description] as Status,
    Quantity,
    quantity_type.[Description] as QuantityType,
    inventory.Barcode as Barcode,
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
where inventory.CompanyId = @companyId;

insert into @invTableFiltered
select * from @invTable
{barcodeSearch}
{exactSearch};

set @count = (select count(*) from @invTableFiltered);";
            string query = $@"
{_param}
select 
    Id,
    CompanyId,
    [Description],
    Status,
    Quantity,
    QuantityType,
    Barcode,
    Location,
    LastEditedOn,
    CreatedOn,
    QtyTypeId,
    LocationId,
    StatusId
from @invTableFiltered
{sortBy}
offset (@page * @pageSize) rows 
fetch next @pageSize rows only";
#if DEBUG
            System.Diagnostics.Debug.WriteLine(query);
#endif

            var items = (await QueryAsync<Models.Inventory>(query)).ToList();
            string totalQuery = $@"
{_param}
select @count as InventoryCount;";
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
