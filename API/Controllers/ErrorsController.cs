using API.Helpers.Errors;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers;

[Route("errors/{code}")]
public class ErrorsController : BaseApiController
{
    // GET: ErrorsController
    public ActionResult Error(int code)
    {
        return new ObjectResult(new ApiResponse(code));
    }

}
