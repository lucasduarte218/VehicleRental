using Moq;
using NUnit.Framework;
using VehicleRental.Application.DTOs.Vehicle;
using VehicleRental.Application.UseCases.Vehicle;
using VehicleRental.Domain.Entities.Base;
using VehicleRental.Domain.Enums;
using VehicleRental.Domain.Interfaces.Repositories;
using Assert = NUnit.Framework.Assert;

namespace VehicleRental.Tests.Application.UseCases.Vehicle;

[TestFixture]
public class UpdateVehicleUseCaseTests
{
    private Mock<IVehicleRepository> _repositoryMock;
    private UpdateVehicleUseCase _useCase;

    [SetUp]
    public void Setup()
    {
        _repositoryMock = new Mock<IVehicleRepository>();
        _useCase = new UpdateVehicleUseCase(_repositoryMock.Object);
    }

    [Test]
    public async Task HandleAsync_WithNonExistingVehicle_ShouldReturnNotFoundError()
    {
        string identifier = "123";
        UpdateVehicleDTO dto = new()
        {
            Plate = "XYZ789",
            Year = 2023,
            Model = "Sport"
        };

        _repositoryMock.Setup(r => r.GetByIdAsync(identifier, It.IsAny<CancellationToken>()))
                       .ReturnsAsync((VehicleRental.Domain.Entities.Vehicle?)null);

        Result<VehicleDTO> result = await _useCase.HandleAsync(identifier, dto);

        Assert.Multiple(() =>
        {
            Assert.That(result.IsSuccess, Is.False);
            Assert.That(result.ErrorType, Is.EqualTo(ResultErrorType.NotFound));
            Assert.That(result.ErrorMessage, Is.EqualTo("Veiculo não encontrado"));
        });
    }

    [Test]
    public async Task HandleAsync_WithDuplicatePlate_ShouldReturnBusinessError()
    {
        string identifier = "123";
        UpdateVehicleDTO dto = new()
        {
            Plate = "ABC123"
        };

        VehicleRental.Domain.Entities.Vehicle Vehicle = new()
        {
            Id = identifier,
            Plate = "OLD123"
        };

        VehicleRental.Domain.Entities.Vehicle existingVehicle = new()
        {
            Id = "456",
            Plate = "ABC123"
        };

        _repositoryMock.Setup(r => r.GetByIdAsync(identifier, It.IsAny<CancellationToken>()))
                       .ReturnsAsync(Vehicle);
        _repositoryMock.Setup(r => r.GetByPlateIdentificationAsync(dto.Plate, It.IsAny<CancellationToken>()))
                       .ReturnsAsync(existingVehicle);

        Result<VehicleDTO> result = await _useCase.HandleAsync(identifier, dto);

        Assert.Multiple(() =>
        {
            Assert.That(result.IsSuccess, Is.False);
            Assert.That(result.ErrorType, Is.EqualTo(ResultErrorType.BusinessError));
            Assert.That(result.ErrorMessage, Is.EqualTo("Já existe um Veiculo com esta placa registrada no sistema"));
        });
    }

    [Test]
    public async Task HandleAsync_WithValidPlateUpdate_ShouldUpdatePlateSuccessfully()
    {
        string identifier = "123";
        UpdateVehicleDTO dto = new()
        {
            Plate = "XYZ789"
        };

        VehicleRental.Domain.Entities.Vehicle Vehicle = new()
        {
            Id = identifier,
            Plate = "OLD123"
        };

        _repositoryMock.Setup(r => r.GetByIdAsync(identifier, It.IsAny<CancellationToken>()))
                       .ReturnsAsync(Vehicle);
        _repositoryMock.Setup(r => r.GetByPlateIdentificationAsync(dto.Plate, It.IsAny<CancellationToken>()))
                       .ReturnsAsync((VehicleRental.Domain.Entities.Vehicle?)null);

        Result<VehicleDTO> result = await _useCase.HandleAsync(identifier, dto);

        Assert.Multiple(() =>
        {
            Assert.That(result.IsSuccess, Is.True);
            Assert.That(result.Data?.Plate, Is.EqualTo(dto.Plate));
        });

        _repositoryMock.Verify(r => r.UpdateAsync(Vehicle, It.IsAny<CancellationToken>()), Times.Once);
    }

    [Test]
    public async Task HandleAsync_WithValidYearUpdate_ShouldUpdateYearSuccessfully()
    {
        string identifier = "123";
        UpdateVehicleDTO dto = new()
        {
            Year = 2023
        };

        VehicleRental.Domain.Entities.Vehicle Vehicle = new()
        {
            Id = identifier,
            Year = 2000
        };

        _repositoryMock.Setup(r => r.GetByIdAsync(identifier, It.IsAny<CancellationToken>()))
                       .ReturnsAsync(Vehicle);

        Result<VehicleDTO> result = await _useCase.HandleAsync(identifier, dto);

        Assert.Multiple(() =>
        {
            Assert.That(result.IsSuccess, Is.True);
            Assert.That(result.Data?.Year, Is.EqualTo(dto.Year));
        });

        _repositoryMock.Verify(r => r.UpdateAsync(Vehicle, It.IsAny<CancellationToken>()), Times.Once);
    }

    [Test]
    public async Task HandleAsync_WithValidModelUpdate_ShouldUpdateModelSuccessfully()
    {
        string identifier = "123";
        UpdateVehicleDTO dto = new()
        {
            Model = "Touring"
        };

        VehicleRental.Domain.Entities.Vehicle Vehicle = new()
        {
            Id = identifier,
            Model = "Sport"
        };

        _repositoryMock.Setup(r => r.GetByIdAsync(identifier, It.IsAny<CancellationToken>()))
                       .ReturnsAsync(Vehicle);

        Result<VehicleDTO> result = await _useCase.HandleAsync(identifier, dto);

        Assert.Multiple(() =>
        {
            Assert.That(result.IsSuccess, Is.True);
            Assert.That(result.Data?.Model, Is.EqualTo(dto.Model));
        });

        _repositoryMock.Verify(r => r.UpdateAsync(Vehicle, It.IsAny<CancellationToken>()), Times.Once);
    }
}