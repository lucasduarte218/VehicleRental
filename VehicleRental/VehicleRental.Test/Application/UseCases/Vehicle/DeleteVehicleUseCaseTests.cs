using Moq;
using NUnit.Framework;
using VehicleRental.Application.UseCases.Vehicle;
using VehicleRental.Domain.Entities;
using VehicleRental.Domain.Entities.Base;
using VehicleRental.Domain.Enums;
using VehicleRental.Domain.Interfaces.Repositories;
using Assert = NUnit.Framework.Assert;

namespace VehicleRental.Tests.Application.UseCases.Vehicle;

[TestFixture]
public class DeleteVehicleUseCaseTests
{
    private Mock<IVehicleRepository> _VehicleRepositoryMock;
    private Mock<IRentRepository> _rentRepositoryMock;
    private DeleteVehicleUseCase _useCase;

    [SetUp]
    public void Setup()
    {
        _VehicleRepositoryMock = new Mock<IVehicleRepository>();
        _rentRepositoryMock = new Mock<IRentRepository>();
        _useCase = new DeleteVehicleUseCase(_VehicleRepositoryMock.Object, _rentRepositoryMock.Object);
    }

    [Test]
    public async Task HandleAsync_WithNonExistingVehicle_ShouldReturnNotFoundError()
    {
        string identifier = "123";

        _VehicleRepositoryMock.Setup(r => r.GetByIdAsync(identifier, It.IsAny<CancellationToken>()))
                       .ReturnsAsync((VehicleRental.Domain.Entities.Vehicle?)null);

        Result result = await _useCase.HandleAsync(identifier);

        Assert.Multiple(() =>
        {
            Assert.That(result.IsSuccess, Is.False);
            Assert.That(result.ErrorType, Is.EqualTo(ResultErrorType.NotFound));
            Assert.That(result.ErrorMessage, Is.EqualTo("Moto não encontrada"));
        });
    }

    [Test]
    public async Task HandleAsync_WithVehicleCurrentlyRented_ShouldReturnBusinessError()
    {
        string identifier = "123";
        VehicleRental.Domain.Entities.Vehicle Vehicle = new()
        {
            Id = identifier
        };
        _VehicleRepositoryMock.Setup(r => r.GetByIdAsync(identifier, It.IsAny<CancellationToken>()))
                       .ReturnsAsync(Vehicle);
        _rentRepositoryMock.Setup(r => r.IsVehicleRentedAsync(Vehicle.Id!, It.IsAny<CancellationToken>()))
                           .ReturnsAsync(true);

        Result result = await _useCase.HandleAsync(identifier);
        Assert.Multiple(() =>
        {
            Assert.That(result.IsSuccess, Is.False);
            Assert.That(result.ErrorType, Is.EqualTo(ResultErrorType.BusinessError));
            Assert.That(result.ErrorMessage, Is.EqualTo("Moto está atualmente alugada"));
        });
    }

    [Test]
    public async Task HandleAsync_WithExistingVehicle_ShouldDeleteSuccessfully()
    {
        string identifier = "123";

        VehicleRental.Domain.Entities.Vehicle Vehicle = new()
        {
            Id = identifier
        };

        _VehicleRepositoryMock.Setup(r => r.GetByIdAsync(identifier, It.IsAny<CancellationToken>()))
                       .ReturnsAsync(Vehicle);

        _rentRepositoryMock.Setup(r => r.IsVehicleRentedAsync(Vehicle.Id!, It.IsAny<CancellationToken>()))
                           .ReturnsAsync(false);

        Result result = await _useCase.HandleAsync(identifier);

        Assert.That(result.IsSuccess, Is.True);

        _VehicleRepositoryMock.Verify(r => r.DeleteAsync(Vehicle.Id!, It.IsAny<CancellationToken>()), Times.Once);
    }

}