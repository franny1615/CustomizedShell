using Inventory.API.Utilities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Inventory.API.Controllers;

[Route("api/facedetection")]
public class FaceDetectionController(IHttpContextAccessor httpContextAccessor) : BaseController(httpContextAccessor)
{
    [HttpPost]
    [Authorize]
    [Route("movenet")]
    [ProducesResponseType<float[]>(StatusCodes.Status200OK)]
    [ProducesResponseType<string>(StatusCodes.Status500InternalServerError)]
    public IActionResult PredictMoveNet(IFormFile file)
    {
        var result = MoveNetUtility.Predict(file.OpenReadStream());
        if (!string.IsNullOrEmpty(result.ErrorMessage))
        {
            return Resp.ErrorRespose(result.ErrorMessage);
        }
        return Resp.OkResponse(result.Data);
    }
}
