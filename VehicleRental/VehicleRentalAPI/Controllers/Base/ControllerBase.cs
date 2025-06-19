using Microsoft.AspNetCore.Mvc;
using VehicleRental.Domain.Entities.Base;
using VehicleRental.Domain.Enums;

namespace VehicleRentalAPI.Controllers.Base
{
    [ApiController]
    public class ApiControllerBase : ControllerBase
    {
        protected virtual IActionResult HandleResult(Result result)
        {
            return result.IsSuccess
                ? Ok()
                : HandleError(result);
        }

        protected virtual IActionResult HandleResult<T>(Result<T> result)
        {
            return result.IsSuccess
                ? Ok(result.Data)
                : HandleError(result);
        }

        protected virtual IActionResult HandleError(Result result)
        {
            var resultObject = new
            {
                mensagem = result.ErrorMessage
            };

            return result.ErrorType switch
            {
                ResultErrorType.NotFound => NotFound(resultObject),
                ResultErrorType.ValidationError => BadRequest(resultObject),
                ResultErrorType.Unauthorized => Unauthorized(resultObject),
                ResultErrorType.BusinessError => BadRequest(resultObject),
                _ => StatusCode(StatusCodes.Status500InternalServerError, resultObject)
            };
        }
    }
}
