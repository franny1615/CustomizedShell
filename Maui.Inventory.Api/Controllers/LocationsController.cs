using GenCode128;
using Maui.Inventory.Api.Interfaces;
using Maui.Inventory.Api.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Drawing;

namespace Maui.Inventory.Api.Controllers;

[Route("api/locations")]
public class LocationsController(
    IHttpContextAccessor httpContextAccessor,
    ILocationRepository locationRepository) : BaseController
{
    [HttpGet]
    [Route("list")]
    [Authorize]
    public async Task<APIResponse<PaginatedQueryResponse<Location>>> GetInventory(
        [FromQuery] PaginatedRequest paginatedRequest)
    {
        APIResponse<PaginatedQueryResponse<Location>> response;

        try
        {
            var user = httpContextAccessor.HttpContext?.User!;
            int adminId = Env.GetAdminIDFromIdentity(user);

            response = await locationRepository.GetAll(paginatedRequest, adminId);
        }
        catch (Exception ex)
        {
            response = new();
            response.Success = false;
            response.Message = ex.Message;
            response.Data = new();
        }

        return response;
    }

    [HttpPost]
    [Route("insert")]
    [Authorize]
    public async Task<APIResponse<bool>> Insert([FromBody] Location location)
    {
        return await locationRepository.Insert(location);
    }

    [HttpPost]
    [Route("update")]
    [Authorize]
    public async Task<APIResponse<bool>> Update([FromBody] Location location)
    {
        return await locationRepository.Update(location);
    }

    [HttpPost]
    [Route("delete")]
    [Authorize]
    public async Task<APIResponse<bool>> Delete([FromBody] Location location)
    {
        return await locationRepository.Delete(location);
    }

    [HttpGet]
    [Route("generateBarcode")]
    [Authorize]
    public APIResponse<string> GenerateBarcode([FromQuery] Location location)
    {
        APIResponse<string> response = new();
        try
        {
            Image barcodeImg = Code128Rendering.MakeBarcodeImage(location.Barcode, 2, true);
            using (MemoryStream ms = new MemoryStream()) 
            {
                ImageConverter conventer = new();
                object? imageBytes = conventer.ConvertTo(barcodeImg, typeof(byte[]));

                response.Success = true;
                response.Message = "success";
                response.Data = Convert.ToBase64String((imageBytes as byte[]) ?? []);
            }
        }
        catch (Exception ex)
        {
            response.Success = false;
            response.Message = $"ERROR >>> {ex.Message}";
            response.Data = "";
        }
        return response;
    }
}
