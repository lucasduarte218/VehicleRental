using Moq;
using VehicleRental.Application.DTOs.Rent;
using VehicleRental.Application.UseCases.Rent;
using VehicleRental.Domain.Entities;
using VehicleRental.Domain.Entities.Base;
using VehicleRental.Domain.Enums;
using VehicleRental.Domain.Interfaces.Repositories;
using VehicleRental.Domain.Interfaces.Services;
using VehicleRental.Domain.ValueObjects;
using Assert = NUnit.Framework.Assert;

namespace MotoHub.Tests.UseCases.Renting;

[TestFixture]
public class RentVehicleUseCaseTests
{
    private Mock<IRentRepository> _rentRepositoryMock;
    private Mock<IVehicleRepository> _VehicleRepositoryMock;
    private Mock<IUserRepository> _userRepositoryMock;
    private Mock<IRentContractCatalog> _rentContractCatalogMock;
    private InsertRentUseCase _useCase;

    [SetUp]
    public void Setup()
    {
        _rentRepositoryMock = new Mock<IRentRepository>();
        _VehicleRepositoryMock = new Mock<IVehicleRepository>();
        _userRepositoryMock = new Mock<IUserRepository>();
        _rentContractCatalogMock = new Mock<IRentContractCatalog>();
        _useCase = new InsertRentUseCase(
            _rentRepositoryMock.Object,
            _VehicleRepositoryMock.Object,
            _userRepositoryMock.Object,
            _rentContractCatalogMock.Object);
    }

    [Test]
    public async Task HandleAsync_WithExistingRentIdentifier_ShouldReturnValidationError()
    {
        RentVehicleDTO dto = new()
        {
            Identifier = "rent-001",
            CourierIdentifier = "courier-001",
            VehicleIdentifier = "moto-001",
            Contract = 1
        };

        _rentRepositoryMock.Setup(r => r.GetByIdAsync(dto.Identifier, It.IsAny<CancellationToken>()))
                           .ReturnsAsync(new Rent());

        Result<RentDTO> result = await _useCase.HandleAsync(dto);

        Assert.Multiple(() =>
        {
            Assert.That(result.IsSuccess, Is.False);
            Assert.That(result.ErrorType, Is.EqualTo(ResultErrorType.ValidationError));
            Assert.That(result.ErrorMessage, Is.EqualTo("Identificador de locação já existe"));
        });
    }

    [Test]
    public async Task HandleAsync_WithInvalidCourierIdentifier_ShouldReturnValidationError()
    {
        RentVehicleDTO dto = new()
        {
            Identifier = "rent-001",
            CourierIdentifier = " ",
            VehicleIdentifier = "moto-001",
            Contract = 1
        };

        Result<RentDTO> result = await _useCase.HandleAsync(dto);

        Assert.Multiple(() =>
        {
            Assert.That(result.IsSuccess, Is.False);
            Assert.That(result.ErrorType, Is.EqualTo(ResultErrorType.ValidationError));
            Assert.That(result.ErrorMessage, Is.EqualTo("Identificador do locatário inválido"));
        });
    }

    [Test]
    public async Task HandleAsync_WithInvalidVehicleIdentifier_ShouldReturnValidationError()
    {
        RentVehicleDTO dto = new()
        {
            Identifier = "rent-001",
            CourierIdentifier = "courier-001",
            VehicleIdentifier = " ",
            Contract = 1
        };

        Result<RentDTO> result = await _useCase.HandleAsync(dto);

        Assert.Multiple(() =>
        {
            Assert.That(result.IsSuccess, Is.False);
            Assert.That(result.ErrorType, Is.EqualTo(ResultErrorType.ValidationError));
            Assert.That(result.ErrorMessage, Is.EqualTo("Identificador da moto inválido"));
        });
    }

    [Test]
    public async Task HandleAsync_WithInvalidRentContract_ShouldReturnValidationError()
    {
        RentVehicleDTO dto = new()
        {
            Identifier = "rent-001",
            CourierIdentifier = "courier-001",
            VehicleIdentifier = "moto-001",
            Contract = 99
        };

        _rentContractCatalogMock.Setup(r => r.FindContractByNumber(dto.Contract))
                            .Returns((RentContract?)null);

        Result<RentDTO> result = await _useCase.HandleAsync(dto);

        Assert.Multiple(() =>
        {
            Assert.That(result.IsSuccess, Is.False);
            Assert.That(result.ErrorType, Is.EqualTo(ResultErrorType.ValidationError));
            Assert.That(result.ErrorMessage, Is.EqualTo("Plano de aluguel inválido"));
        });
    }
}