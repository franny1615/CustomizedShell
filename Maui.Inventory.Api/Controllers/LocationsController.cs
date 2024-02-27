using GenCode128;
using Maui.Inventory.Api.Interfaces;
using Maui.Inventory.Api.Models;
using Maui.Inventory.Api.Utilities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Drawing;
using System.Drawing.Imaging;

namespace Maui.Inventory.Api.Controllers;

[Route("api/locations")]
public class LocationsController(
    IHttpContextAccessor httpContextAccessor,
    ILocationRepository locationRepository) : BaseController
{
    [HttpGet]
    [Route("list")]
    [Authorize]
    public async Task<APIResponse<PaginatedQueryResponse<Location>>> GetAll(
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
            Image barcodeImg = BarcodeGenerator.Code128(location.Description, location.Barcode, 2, true);

            MemoryStream ms = new MemoryStream();
            ((Bitmap)barcodeImg).Save(ms, ImageFormat.Png);

            byte[] byteImage = ms.ToArray();
            string strImage = Convert.ToBase64String(byteImage);

            response.Success = true;
            response.Message = "success";
            response.Data = strImage;
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
