using Microsoft.AspNetCore.Mvc;

namespace Inventory.API.Utilities;

public static class Resp
{
    public static ObjectResult OkResponse<T>(T data)
    {
        return new ObjectResult(data) { StatusCode = StatusCodes.Status200OK };
    }

    public static ObjectResult ErrorRespose(string errorMessage)
    {
        return new ObjectResult(errorMessage) { StatusCode = StatusCodes.Status500InternalServerError };
    }
}
