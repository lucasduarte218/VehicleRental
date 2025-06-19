using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using VehicleRental.Application.DTOs.Vehicle;
using VehicleRental.Application.Interfaces.UseCases.Vehicle;
using VehicleRental.Domain.Entities.Base;
using VehicleRental.Domain.Filters.Vehicles.Find;
using VehicleRentalAPI.Controllers.Base;
using VehicleRentalAPI.Extensions;
using VehicleRentalAPI.ValueObjects.Request;
using VehicleRentalAPI.ValueObjects.Response;

namespace VehicleRentalAPI.Controllers
{
    [Route("motos")]
    [Tags("Motos")]
    public class VehicleController(IMapper mapper, ILogger<VehicleController> logger) : ApiControllerBase
    {
        [HttpPost]
        [EndpointSummary("Cadastrar uma nova moto")]
        [EndpointDescription("Cadastrar uma nova moto no sistema e disponibilizar para aluguel")]
        [Consumes("application/json")]
        [ProducesResponseType(StatusCodes.Status201Created)]
        public async Task<IActionResult> Register([FromServices] IInsertVehicleUseCase useCase,
                                                  [FromBody] InsertVehicleRequest insertVehicleRequest,
                                                  CancellationToken cancellationToken)
        {
            logger.LogInformation("Registering new motorcycle");

            InsertVehicleDTO dto = mapper.Map<InsertVehicleDTO>(insertVehicleRequest);

            Result<VehicleDTO> result = await useCase.HandleAsync(dto, cancellationToken);

            return result.IsSuccess ? Created() : HandleError(result);
        }

        [HttpGet]
        [EndpointSummary("Consultar motos existentes")]
        [EndpointDescription("Consulta e retorna as motos existentes através da placa")]
        [ProducesResponseType(typeof(IEnumerable<VehicleResponse>), StatusCodes.Status200OK, "application/json")]
        public async Task<IActionResult> SearchAsync([FromServices] IFindVehicleUseCase useCase,
                                                     [FromQuery(Name = "placa")] string? plate,
                                                     CancellationToken cancellationToken)
        {
            logger.LogInformation("Searching motorcycles");

            VehicleFindFilter queryDTO = new()
            {
                Plate = plate,
                Offset = 0,
                Limit = 100,
            };

            Result<List<VehicleResponse>> result = await useCase.HandleAsync(queryDTO, cancellationToken)
                                                                   .MapResultTo((VehicleDTO m) => mapper.Map<VehicleResponse>(m));
            return HandleResult(result);
        }

        [HttpPut("{id}/placa")]
        [EndpointSummary("Modificar a placa de uma moto")]
        [EndpointDescription("Atualiza parcialmente os dados de uma moto no sistema com base no identificador")]
        [Consumes("application/json")]
        [ProducesResponseType(typeof(VehicleDTO), StatusCodes.Status200OK, "application/json")]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> Update([FromServices] IUpdateVehicleUseCase useCase,
                                                [FromRoute] string id,
                                                [FromBody] UpdateVehicleRequest updateVehicleRequest,
                                                CancellationToken cancellationToken)
        {
            logger.LogInformation("Updating motorcycle with identifier: {Identifier}", id);

            UpdateVehicleDTO dto = mapper.Map<UpdateVehicleDTO>(updateVehicleRequest);

            Result<VehicleDTO> result = await useCase.HandleAsync(id, dto, cancellationToken);

            return result.IsSuccess ? Ok(new
            {
                mensagem = "Placa atualizada com sucesso",
            }) : HandleError(result);
        }


        [HttpGet("{id}")]
        [EndpointSummary("Consultar motos existentes por id")]
        [EndpointDescription("Consulta e retorna as motos existentes através do identificador")]
        [ProducesResponseType(typeof(VehicleResponse), StatusCodes.Status200OK, "application/json")]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetByIdentifier([FromServices] IGetVehicleUseCase useCase,
                                                         [FromRoute] string id,
                                                         CancellationToken cancellationToken)
        {
            logger.LogInformation("Fetching motorcycle by identifier");

            Result<VehicleResponse> result = await useCase.HandleAsync(id, cancellationToken)
                                                             .MapResultTo(mapper.Map<VehicleResponse>);

            return HandleResult(result);
        }

        [HttpDelete("{id}")]
        [EndpointSummary("Remover moto cadastrada")]
        [EndpointDescription("Remove uma moto pelo identificador")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> DeleteMotorcycle([FromServices] IDeleteVehicleUseCase useCase,
                                                          [FromRoute] string id,
                                                          CancellationToken cancellationToken)
        {
            logger.LogInformation("Deleting motorcycle by identifier");

            Result result = await useCase.HandleAsync(id, cancellationToken);

            return HandleResult(result);
        }

    }
}
