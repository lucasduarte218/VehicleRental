using Moq;
using NUnit.Framework;
using VehicleRental.Application.DTOs.Vehicle;
using VehicleRental.Application.UseCases.Vehicle;
using VehicleRental.Domain.Entities.Base;
using VehicleRental.Domain.Enums;
using VehicleRental.Domain.Filters.Vehicles.Find;
using VehicleRental.Domain.Interfaces.Repositories;
using Assert = NUnit.Framework.Assert;

namespace VehicleRental.Tests.Application.UseCases.Vehicle;

[TestFixture]
public class FindVehicleUseCaseTests
{
    private Mock<IVehicleRepository> _repositoryMock;
    private FindVehicleUseCase _useCase;

    [SetUp]
    public void Setup()
    {
        _repositoryMock = new Mock<IVehicleRepository>();
        _useCase = new FindVehicleUseCase(_repositoryMock.Object);
    }

    [Test]
    public async Task HandleAsync_WithNegativeOffset_ShouldReturnValidationError()
    {
        VehicleFindFilter parameters = new()
        {
            Offset = -1,
            Limit = 10
        };

        Result<List<VehicleDTO>> result = await _useCase.HandleAsync(parameters);

        Assert.Multiple(() =>
        {
            Assert.That(result.IsSuccess, Is.False);
            Assert.That(result.ErrorType, Is.EqualTo(ResultErrorType.ValidationError));
            Assert.That(result.ErrorMessage, Is.EqualTo("Offset and limit must be greater than or equal to zero."));
        });
    }

    [Test]
    public async Task HandleAsync_WithZeroOrNegativeLimit_ShouldReturnValidationError()
    {
        VehicleFindFilter parameters = new()
        {
            Offset = 0,
            Limit = 0
        };

        Result<List<VehicleDTO>> result = await _useCase.HandleAsync(parameters);

        Assert.Multiple(() =>
        {
            Assert.That(result.IsSuccess, Is.False);
            Assert.That(result.ErrorType, Is.EqualTo(ResultErrorType.ValidationError));
            Assert.That(result.ErrorMessage, Is.EqualTo("Offset and limit must be greater than or equal to zero."));
        });
    }

    [Test]
    public async Task HandleAsync_WithLimitGreaterThan100_ShouldReturnValidationError()
    {
        VehicleFindFilter parameters = new VehicleFindFilter
        {
            Offset = 0,
            Limit = 101
        };

        Result<List<VehicleDTO>> result = await _useCase.HandleAsync(parameters);

        Assert.Multiple(() =>
        {
            Assert.That(result.IsSuccess, Is.False);
            Assert.That(result.ErrorType, Is.EqualTo(ResultErrorType.ValidationError));
            Assert.That(result.ErrorMessage, Is.EqualTo("Limit cannot exceed 100."));
        });
    }

    [Test]
    public async Task HandleAsync_WithValidParameters_ShouldCallRepositoryAndReturnData()
    {
        VehicleFindFilter parameters = new()
        {
            Offset = 0,
            Limit = 50
        };

        List<VehicleRental.Domain.Entities.Vehicle> mockVehicles =
        [
            new VehicleRental.Domain.Entities.Vehicle { Id = "1", Year = 2020, Plate = "ABC123", Model = "Sport" },
            new VehicleRental.Domain.Entities.Vehicle { Id = "2", Year = 2021, Plate = "XYZ789", Model = "Touring" }
        ];

        _repositoryMock.Setup(r => r.FindAsync(parameters, It.IsAny<CancellationToken>()))
                       .ReturnsAsync(mockVehicles);

        Result<List<VehicleDTO>> result = await _useCase.HandleAsync(parameters);

        Assert.Multiple(() =>
        {
            Assert.That(result.IsSuccess, Is.True);
            Assert.That(result.Data?.Count, Is.EqualTo(2));
        });

        _repositoryMock.Verify(r => r.FindAsync(parameters, It.IsAny<CancellationToken>()), Times.Once);
    }
}