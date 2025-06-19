using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using VehicleRental.Application.DTOs.Rent;
using VehicleRental.Application.Interfaces.UseCases.Rent;
using VehicleRental.Domain.Entities.Base;
using VehicleRentalAPI.Controllers.Base;
using VehicleRentalAPI.Extensions;
using VehicleRentalAPI.ValueObjects.Request;
using VehicleRentalAPI.ValueObjects.Response;

namespace VehicleRentalAPI.Controllers
{
    [Route("locacao")]
    [Tags("Locação")]
    public class RentController(IMapper mapper, ILogger<RentController> logger) : ApiControllerBase
    {
        [HttpPost]
        [EndpointSummary("Alugar uma moto")]
        [EndpointDescription("Registra um novo aluguel de moto no sistema")]
        [Consumes("application/json")]
        [ProducesResponseType(typeof(RentDetailsResponse), StatusCodes.Status201Created, "application/json")]
        public async Task<IActionResult> RentMotorcycle([FromServices] IInsertRentUseCase useCase,
                                                        [FromBody] InsertRentRequest insertRentRequest,
                                                        CancellationToken cancellationToken)
        {
            logger.LogInformation("Registering new motorcycle rent");

            RentVehicleDTO dto = mapper.Map<RentVehicleDTO>(insertRentRequest);

            Result<RentDetailsResponse> result = await useCase.HandleAsync(dto, cancellationToken)
                                                              .MapResultTo(mapper.Map<RentDetailsResponse>);

            return result.IsSuccess ? Created($"{Request.Path}/{result.Data!.Identifier}", result.Data) : HandleError(result);
        }

        [HttpGet("{id}")]
        [EndpointSummary("Consultar locação por id")]
        [EndpointDescription("Retorna os detalhes de um aluguel específico baseado no identificador")]
        [ProducesResponseType(typeof(RentDetailsResponse), StatusCodes.Status200OK, "application/json")]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetByIdentifier([FromServices] IGetRentUseCase useCase,
                                                         [FromRoute] string id,
                                                         CancellationToken cancellationToken)
        {
            logger.LogInformation("Fetching rent details for ID: {Id}", id);

            Result<RentDetailsResponse> result = await useCase.HandleAsync(id, cancellationToken)
                                                              .MapResultTo(mapper.Map<RentDetailsResponse>);

            return HandleResult(result);
        }

        [HttpPut("{id}/devolucao")]
        [EndpointSummary("Informar data de devolução e calcular valor")]
        [EndpointDescription("Define a data em que a moto foi devolvida no sistema e calcula o custo do aluguel")]
        [Consumes("application/json")]
        [ProducesResponseType(typeof(CompletedRentalResponse), StatusCodes.Status200OK, "application/json")]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> ReturnMotorcycle([FromServices] ICompleteRentUseCase useCase,
                                                          [FromRoute] string id,
                                                          [FromBody] CompleteRentRequest returnMotorcycleRequest,
                                                          CancellationToken cancellationToken)
        {
            logger.LogInformation("Processing motorcycle return for rent ID: {Id}", id);

            CompleteRentRequestDTO dto = new()
            {
                RentIdentifier = id,
                ReturnDate = returnMotorcycleRequest.ReturnDate
            };

            Result<CompletedRentalResponse> result = await useCase.HandleAsync(dto, cancellationToken)
                                                                  .MapResultTo(mapper.Map<CompletedRentalResponse>);

            return HandleResult(result);
        }

    }
}
