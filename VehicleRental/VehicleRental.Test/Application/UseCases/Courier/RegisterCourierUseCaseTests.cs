using Moq;
using NUnit.Framework;
using VehicleRental.Application.DTOs.Courier;
using VehicleRental.Application.DTOs.Courier.Insert;
using VehicleRental.Application.Interfaces.Integration;
using VehicleRental.Application.UseCases.Courier;
using VehicleRental.Domain.Entities;
using VehicleRental.Domain.Entities.Base;
using VehicleRental.Domain.Enums;
using VehicleRental.Domain.Interfaces.Repositories;
using Assert = NUnit.Framework.Assert;

namespace VehicleRental.Tests.Application.UseCases.Courier;

[TestFixture]
public class InsertCourierUseCaseTests
{
    private Mock<IUserRepository> _userRepositoryMock;
    private Mock<IImageStorage> _imageStorageMock;
    private InsertCourierUseCase _useCase;

    [SetUp]
    public void Setup()
    {
        _userRepositoryMock = new Mock<IUserRepository>();
        _imageStorageMock = new Mock<IImageStorage>();
        _useCase = new InsertCourierUseCase(_userRepositoryMock.Object, _imageStorageMock.Object);
    }

    [Test]
    public async Task HandleAsync_WithInvalidIdentifier_ShouldReturnValidationError()
    {
        InsertCourierDTO dto = new()
        {
            Identifier = "  ", // Identificador inválido
            Name = "João",
            TaxNumber = "12345678901234",
            BirthDate = DateTime.UtcNow.AddYears(-20),
            DriverLicenseNumber = "ABC123",
            DriverLicenseType = DriverLicenseType.A,
            DriverLicenseImageBase64 = "ImagemBase64"
        };

        Result<CourierDTO> result = await _useCase.HandleAsync(dto);

        Assert.Multiple(() =>
        {
            Assert.That(result.IsSuccess, Is.False);
            Assert.That(result.ErrorType, Is.EqualTo(ResultErrorType.ValidationError));
            Assert.That(result.ErrorMessage, Is.EqualTo("Identificador inválido"));
        });
    }

    [Test]
    public async Task HandleAsync_WithInvalidName_ShouldReturnValidationError()
    {
        InsertCourierDTO dto = new()
        {
            Identifier = "123",
            Name = "", // Nome inválido
            TaxNumber = "12345678901234",
            BirthDate = DateTime.UtcNow.AddYears(-20),
            DriverLicenseNumber = "ABC123",
            DriverLicenseType = DriverLicenseType.A,
            DriverLicenseImageBase64 = "ImagemBase64"
        };

        Result<CourierDTO> result = await _useCase.HandleAsync(dto);

        Assert.Multiple(() =>
        {
            Assert.That(result.IsSuccess, Is.False);
            Assert.That(result.ErrorType, Is.EqualTo(ResultErrorType.ValidationError));
            Assert.That(result.ErrorMessage, Is.EqualTo("Nome inválido"));
        });
    }

    [Test]
    public async Task HandleAsync_WithInvalidTaxNumber_ShouldReturnValidationError()
    {
        InsertCourierDTO dto = new()
        {
            Identifier = "123",
            Name = "João",
            TaxNumber = "12345", // CNPJ inválido
            BirthDate = DateTime.UtcNow.AddYears(-20),
            DriverLicenseNumber = "ABC123",
            DriverLicenseType = DriverLicenseType.A,
            DriverLicenseImageBase64 = "ImagemBase64"
        };

        Result<CourierDTO> result = await _useCase.HandleAsync(dto);

        Assert.Multiple(() =>
        {
            Assert.That(result.IsSuccess, Is.False);
            Assert.That(result.ErrorType, Is.EqualTo(ResultErrorType.ValidationError));
            Assert.That(result.ErrorMessage, Is.EqualTo("CNPJ inválido"));
        });
    }

    [Test]
    public async Task HandleAsync_WithUnderageCourier_ShouldReturnValidationError()
    {
        InsertCourierDTO dto = new()
        {
            Identifier = "123",
            Name = "João",
            TaxNumber = "12345678901234",
            BirthDate = DateTime.UtcNow.AddYears(-17), // Menor de 18 anos
            DriverLicenseNumber = "ABC123",
            DriverLicenseType = DriverLicenseType.A,
            DriverLicenseImageBase64 = "ImagemBase64"
        };

        Result<CourierDTO> result = await _useCase.HandleAsync(dto);

        Assert.Multiple(() =>
        {
            Assert.That(result.IsSuccess, Is.False);
            Assert.That(result.ErrorType, Is.EqualTo(ResultErrorType.ValidationError));
            Assert.That(result.ErrorMessage, Is.EqualTo("O entregador deve ter pelo menos 18 anos"));
        });
    }

    [Test]
    public async Task HandleAsync_WithExistingIdentifier_ShouldReturnBusinessError()
    {
        InsertCourierDTO dto = new()
        {
            Identifier = "123",
            Name = "João",
            TaxNumber = "12345678901234",
            BirthDate = DateTime.UtcNow.AddYears(-20),
            DriverLicenseNumber = "ABC123",
            DriverLicenseType = DriverLicenseType.A,
            DriverLicenseImageBase64 = "ImagemBase64"
        };

        User existingUser = new() { Id = "123" };

        _userRepositoryMock.Setup(r => r.GetByIdAsync(dto.Identifier, It.IsAny<CancellationToken>()))
                           .ReturnsAsync(existingUser);

        Result<CourierDTO> result = await _useCase.HandleAsync(dto);

        Assert.Multiple(() =>
        {
            Assert.That(result.IsSuccess, Is.False);
            Assert.That(result.ErrorType, Is.EqualTo(ResultErrorType.BusinessError));
            Assert.That(result.ErrorMessage, Is.EqualTo("Já existe um usuário com este identificador no sistema"));
        });
    }

    [Test]
    public async Task HandleAsync_WithValidParameters_ShouldInsertCourierSuccessfully()
    {
        InsertCourierDTO dto = new()
        {
            Identifier = "123",
            Name = "João",
            TaxNumber = "12345678901234",
            BirthDate = DateTime.UtcNow.AddYears(-20),
            DriverLicenseNumber = "ABC123",
            DriverLicenseType = DriverLicenseType.A,
            DriverLicenseImageBase64 = "ImagemBase64"
        };

        _userRepositoryMock.Setup(r => r.GetByIdAsync(dto.Identifier, It.IsAny<CancellationToken>()))
                           .ReturnsAsync((User?)null);
        _userRepositoryMock.Setup(r => r.GetByTaxNumberAsync(dto.TaxNumber, It.IsAny<CancellationToken>()))
                           .ReturnsAsync((User?)null);
        _userRepositoryMock.Setup(r => r.GetByLicenseNumberAsync(dto.DriverLicenseNumber, It.IsAny<CancellationToken>()))
                           .ReturnsAsync((User?)null);
        _imageStorageMock.Setup(r => r.UploadImageAsBase64Async(dto.DriverLicenseImageBase64, It.IsAny<CancellationToken>()))
                         .ReturnsAsync("image-123");

        Result<CourierDTO> result = await _useCase.HandleAsync(dto);

        Assert.Multiple(() =>
        {
            Assert.That(result.IsSuccess, Is.True);
            Assert.That(result.Data?.Identifier, Is.EqualTo(dto.Identifier));
            Assert.That(result.Data?.Name, Is.EqualTo(dto.Name));
            Assert.That(result.Data?.TaxNumber, Is.EqualTo(dto.TaxNumber));
            Assert.That(result.Data?.DriverLicenseImageBase64, Is.EqualTo(dto.DriverLicenseImageBase64));
        });

        _userRepositoryMock.Verify(r => r.AddAsync(It.IsAny<User>(), It.IsAny<CancellationToken>()), Times.Once);
        _imageStorageMock.Verify(r => r.UploadImageAsBase64Async(dto.DriverLicenseImageBase64, It.IsAny<CancellationToken>()), Times.Once);
    }
}