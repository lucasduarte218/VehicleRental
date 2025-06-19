using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using VehicleRental.Application.DTOs.Courier;
using VehicleRental.Application.DTOs.Courier.Insert;
using VehicleRental.Application.DTOs.Courier.Update;
using VehicleRental.Application.Interfaces.UseCases.Courier;
using VehicleRental.Domain.Entities.Base;
using VehicleRentalAPI.Controllers.Base;
using VehicleRentalAPI.ValueObjects.Request;

namespace VehicleRentalAPI.Controllers
{
    [Route("entregadores")]
    [Tags("Entregadores")]
    public class CourierController(ILogger<CourierController> logger, IMapper mapper) : ApiControllerBase
    {
        [HttpPost]
        [EndpointSummary("Cadastrar um novo entregador")]
        [EndpointDescription("Registra um novo entregador no sistema para realizar entregas")]
        [Consumes("application/json")]
        [ProducesResponseType(typeof(CourierDTO), StatusCodes.Status201Created, "application/json")]
        public async Task<IActionResult> Register([FromServices] IInsertCourierUseCase useCase,
                                                  [FromBody] InsertCourierRequest InsertCourierRequest,
                                                  CancellationToken cancellationToken)
        {
            logger.LogInformation("Registering new courier");

            InsertCourierDTO dto = mapper.Map<InsertCourierDTO>(InsertCourierRequest);

            Result<CourierDTO> result = await useCase.HandleAsync(dto, cancellationToken);

            return result.IsSuccess ? Created() : HandleError(result);
        }

        [HttpPost("{id}/cnh")]
        [EndpointSummary("Enviar foto da CNH")]
        [EndpointDescription("Atualiza a foto da CNH de um entregador no sistema com base no identificador")]
        [Consumes("application/json")]
        [ProducesResponseType(typeof(CourierDTO), StatusCodes.Status200OK, "application/json")]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> Update([FromServices] IUpdateCourierUseCase useCase,
                                                [FromRoute] string id,
                                                [FromBody] UpdateCourierRequest updateCourierRequest,
                                                CancellationToken cancellationToken)
        {
            logger.LogInformation("Updating courier with identifier: {Identifier}", id);

            UpdateCourierDTO dto = mapper.Map<UpdateCourierDTO>(updateCourierRequest);

            Result<CourierDTO> result = await useCase.HandleAsync(id, dto, cancellationToken);

            return result.IsSuccess ? Ok(new
            {
                mensagem = "Entregador atualizado com sucesso",
            }) : HandleError(result);
        }

    }
}
