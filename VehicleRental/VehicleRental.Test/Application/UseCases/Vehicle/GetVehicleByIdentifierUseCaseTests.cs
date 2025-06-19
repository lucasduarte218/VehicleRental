using Moq;
using VehicleRental.Application.DTOs.Vehicle;
using VehicleRental.Application.UseCases.Vehicle;
using VehicleRental.Domain.Entities.Base;
using VehicleRental.Domain.Enums;
using VehicleRental.Domain.Interfaces.Repositories;
using Assert = NUnit.Framework.Assert;

namespace VehicleRental.Tests.Application.UseCases.Vehicle;

[TestFixture]
public class GetVehicleByIdentifierUseCaseTests
{
    private Mock<IVehicleRepository> _repositoryMock;
    private GetVehicleUseCase _useCase;

    [SetUp]
    public void Setup()
    {
        _repositoryMock = new Mock<IVehicleRepository>();
        _useCase = new GetVehicleUseCase(_repositoryMock.Object);
    }

    [Test]
    public async Task HandleAsync_WithNonExistingVehicle_ShouldReturnNotFoundError()
    {
        string identifier = "123";

        _repositoryMock.Setup(r => r.GetByIdAsync(identifier, It.IsAny<CancellationToken>()))
                       .ReturnsAsync((VehicleRental.Domain.Entities.Vehicle?)null);

        Result<VehicleDTO> result = await _useCase.HandleAsync(identifier);

        Assert.Multiple(() =>
        {
            Assert.That(result.IsSuccess, Is.False);
            Assert.That(result.ErrorType, Is.EqualTo(ResultErrorType.NotFound));
            Assert.That(result.ErrorMessage, Is.EqualTo("Veiculo não encontrado"));
        });
    }

    [Test]
    public async Task HandleAsync_WithExistingVehicle_ShouldReturnVehicleData()
    {
        string identifier = "123";

        VehicleRental.Domain.Entities.Vehicle Vehicle = new()
        {
            Id = identifier,
            Plate = "ABC123",
            Year = 2020,
            Model = "Sport"
        };

        _repositoryMock.Setup(r => r.GetByIdAsync(identifier, It.IsAny<CancellationToken>()))
                       .ReturnsAsync(Vehicle);

        Result<VehicleDTO> result = await _useCase.HandleAsync(identifier);

        Assert.Multiple(() =>
        {
            Assert.That(result.IsSuccess, Is.True);
            Assert.That(result.Data?.Identifier, Is.EqualTo(Vehicle.Id));
            Assert.That(result.Data?.Plate, Is.EqualTo(Vehicle.Plate));
            Assert.That(result.Data?.Year, Is.EqualTo(Vehicle.Year));
            Assert.That(result.Data?.Model, Is.EqualTo(Vehicle.Model));
        });

        _repositoryMock.Verify(r => r.GetByIdAsync(identifier, It.IsAny<CancellationToken>()), Times.Once);
    }
}