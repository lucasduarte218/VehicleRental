using Moq;
using NUnit.Framework;
using VehicleRental.Application.DTOs.Vehicle;
using VehicleRental.Application.Events;
using VehicleRental.Application.Interfaces.Messaging;
using VehicleRental.Application.UseCases.Vehicle;
using VehicleRental.Domain.Entities.Base;
using VehicleRental.Domain.Enums;
using VehicleRental.Domain.Interfaces.Repositories;
using Assert = NUnit.Framework.Assert;

namespace VehicleRental.Tests.Application.UseCases.Vehicle;

[TestFixture]
public class InsertVehicleUseCaseTests
{
    private Mock<IVehicleRepository> _repositoryMock;
    private Mock<IVehicleEventPublisher> _publisherMock;
    private InsertVehicleUseCase _useCase;

    [SetUp]
    public void Setup()
    {
        _repositoryMock = new Mock<IVehicleRepository>();
        _publisherMock = new Mock<IVehicleEventPublisher>();
        _useCase = new InsertVehicleUseCase(_repositoryMock.Object, _publisherMock.Object);
    }

    [Test]
    public async Task HandleAsync_WithInvalidPlate_ShouldReturnValidationError()
    {
        InsertVehicleDTO dto = new()
        {
            Plate = "  ", // Placa inválida
            Year = 2022,
            Model = "Sport",
            Identifier = "moto-001"
        };

        Result<VehicleDTO> result = await _useCase.HandleAsync(dto);

        Assert.Multiple(() =>
        {
            Assert.That(result.IsSuccess, Is.False);
            Assert.That(result.ErrorType, Is.EqualTo(ResultErrorType.ValidationError));
            Assert.That(result.ErrorMessage, Is.EqualTo("Placa inválida"));
        });
    }

    [Test]
    public async Task HandleAsync_WithInvalidYear_ShouldReturnValidationError()
    {
        InsertVehicleDTO dto = new()
        {
            Plate = "ABC123",
            Year = 1899, // Ano inválido
            Model = "Sport",
            Identifier = "moto-001"
        };

        Result<VehicleDTO> result = await _useCase.HandleAsync(dto);

        Assert.Multiple(() =>
        {
            Assert.That(result.IsSuccess, Is.False);
            Assert.That(result.ErrorType, Is.EqualTo(ResultErrorType.ValidationError));
            Assert.That(result.ErrorMessage, Is.EqualTo("O ano deve ser maior que 1900"));
        });
    }

    [Test]
    public async Task HandleAsync_WithExistingPlate_ShouldReturnBusinessError()
    {
        InsertVehicleDTO dto = new()
        {
            Plate = "ABC123",
            Year = 2022,
            Model = "Sport",
            Identifier = "moto-001"
        };

        VehicleRental.Domain.Entities.Vehicle existingVehicle = new() { Plate = "ABC123" };

        _repositoryMock.Setup(r => r.GetByPlateIdentificationAsync(dto.Plate, It.IsAny<CancellationToken>()))
                       .ReturnsAsync(existingVehicle);

        Result<VehicleDTO> result = await _useCase.HandleAsync(dto);

        Assert.Multiple(() =>
        {
            Assert.That(result.IsSuccess, Is.False);
            Assert.That(result.ErrorType, Is.EqualTo(ResultErrorType.BusinessError));
            Assert.That(result.ErrorMessage, Is.EqualTo("Já existe uma moto com esta placa registrado no sistema"));
        });
    }

    [Test]
    public async Task HandleAsync_WithExistingIdentifier_ShouldReturnBusinessError()
    {
        InsertVehicleDTO dto = new()
        {
            Plate = "XYZ789",
            Year = 2023,
            Model = "Touring",
            Identifier = "moto-001"
        };

        VehicleRental.Domain.Entities.Vehicle existingVehicle = new() { Id = "moto-001" };

        _repositoryMock.Setup(r => r.GetByIdAsync(dto.Identifier, It.IsAny<CancellationToken>()))
                       .ReturnsAsync(existingVehicle);

        Result<VehicleDTO> result = await _useCase.HandleAsync(dto);

        Assert.Multiple(() =>
        {
            Assert.That(result.IsSuccess, Is.False);
            Assert.That(result.ErrorType, Is.EqualTo(ResultErrorType.BusinessError));
            Assert.That(result.ErrorMessage, Is.EqualTo("Já existe uma moto com este identificador no sistema"));
        });
    }

    [Test]
    public async Task HandleAsync_WithValidData_ShouldInsertVehicleSuccessfully()
    {
        InsertVehicleDTO dto = new()
        {
            Plate = "XYZ789",
            Year = 2023,
            Model = "Touring",
            Identifier = "moto-001"
        };

        _repositoryMock.Setup(r => r.GetByPlateIdentificationAsync(dto.Plate, It.IsAny<CancellationToken>()))
                       .ReturnsAsync((VehicleRental.Domain.Entities.Vehicle?)null);
        _repositoryMock.Setup(r => r.GetByIdAsync(dto.Identifier, It.IsAny<CancellationToken>()))
                       .ReturnsAsync((VehicleRental.Domain.Entities.Vehicle?)null);

        Result<VehicleDTO> result = await _useCase.HandleAsync(dto);

        Assert.Multiple(() =>
        {
            Assert.That(result.IsSuccess, Is.True);
            Assert.That(result.Data?.Identifier, Is.EqualTo(dto.Identifier));
            Assert.That(result.Data?.Plate, Is.EqualTo(dto.Plate));
            Assert.That(result.Data?.Year, Is.EqualTo(dto.Year));
            Assert.That(result.Data?.Model, Is.EqualTo(dto.Model));
        });

        _repositoryMock.Verify(r => r.AddAsync(It.IsAny<VehicleRental.Domain.Entities.Vehicle>(), It.IsAny<CancellationToken>()), Times.Once);
        _publisherMock.Verify(r => r.PublishVehicleRegisteredAsync(It.IsAny<VehicleRegisteredEvent>(), It.IsAny<CancellationToken>()), Times.Once);
    }
}