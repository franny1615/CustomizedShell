using Inventory.API.Models;

namespace Inventory.API.Repositories;

public class QuantityTypesRepository : BaseRepository, ICrudRepository<QuantityType>
{
    public async Task<RepoResult<DeleteResult>> Delete(int itemId, int companyId)
    {
        var result = new RepoResult<DeleteResult>();
        try 
        {
            string checkIfLinkedQuery = $@"
select inventory.Id
from [quantity_type]
inner join inventory on inventory.QtyTypeId = [quantity_type].Id";
            var response = await QueryAsync<object>(checkIfLinkedQuery);
            if (response.Count() > 0)
            {
                result.Data = DeleteResult.LinkedToOtherItems;
            }
            else
            {
                string query = $@"
declare 
@qtyTypeId int = {itemId},
@companyId int = {companyId};

delete from quantity_type
where CompanyId = @companyId
and Id = @qtyTypeId;";
                await QueryAsync<object>(query);
                result.Data = DeleteResult.SuccesfullyDeleted;
            }    
        }
        catch (Exception ex)
        {
            result.ErrorMessage = ex.ToString();
        }
        return result;
    }

    public async Task<RepoResult<QuantityType>> Get(int id, int companyId)
    {
        var result = new RepoResult<QuantityType>();
        try 
        {
            string query = $@"
declare 
@qtyTypeId int = {id},
@companyId int = {companyId};

select 
    Id,
    CompanyId,
    Description 
from quantity_type
where Id = @qtyTypeId
and CompanyId = @companyId";
            result.Data = (await QueryAsync<QuantityType>(query)).First();
        }
        catch (Exception ex)
        {
            result.ErrorMessage = ex.ToString();
        }
        return result;
    }

    public async Task<RepoResult<SearchResult<QuantityType>>> Get(
        SearchRequest request, 
        int companyId)
    {
        var result = new RepoResult<SearchResult<QuantityType>>();
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
    Description
from quantity_type 
where CompanyId = @companyId
and [Description] LIKE @search+'%'
order by Id desc 
offset (@page * @pageSize) rows 
fetch next @pageSize rows only";
            var items = (await QueryAsync<QuantityType>(query)).ToList();
            string totalQuery = $@"select COUNT(*) from quantity_type;";
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

    public async Task<RepoResult<int>> Insert(QuantityType item, int companyId)
    {
        var result = new RepoResult<int>();
        try 
        {
            string query = $@"
declare 
@companyId int = {companyId},
@description nvarchar(max) = '{item.Description}';

insert into quantity_type
(
    CompanyId,
    [Description]
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
            result.ErrorMessage = ex.ToString();
        }
        return result;
    }

    public async Task<RepoResult<bool>> Update(QuantityType item, int companyId)
    {
        var result = new RepoResult<bool>();
        try 
        {
            string query = $@"
declare 
@qtyTypeId int = {item.Id},
@companyId int = {companyId},
@description nvarchar(max) = '{item.Description}';

update quantity_type set 
    [Description] = @description
where CompanyId = @companyId
and Id = @qtyTypeId;";
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