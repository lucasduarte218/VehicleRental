using VehicleRental.Application.DTOs.Courier;
using VehicleRental.Application.DTOs.Courier.Insert;
using VehicleRental.Application.Interfaces.Integration;
using VehicleRental.Application.Interfaces.UseCases.Courier;
using VehicleRental.Domain.Entities;
using VehicleRental.Domain.Entities.Base;
using VehicleRental.Domain.Enums;
using VehicleRental.Domain.Interfaces.Repositories;

namespace VehicleRental.Application.UseCases.Courier
{
    public class InsertCourierUseCase(IUserRepository userRepository, IImageStorage imageStorage) : IInsertCourierUseCase
    {
        public async Task<Result<CourierDTO>> HandleAsync(InsertCourierDTO dto, CancellationToken cancellationToken = default)
        {
            if (string.IsNullOrWhiteSpace(dto.Identifier))
            {
                return Result<CourierDTO>.Failure("Identificador inválido", ResultErrorType.ValidationError);
            }

            if (string.IsNullOrWhiteSpace(dto.Name))
            {
                return Result<CourierDTO>.Failure("Nome inválido", ResultErrorType.ValidationError);
            }

            if (string.IsNullOrWhiteSpace(dto.TaxNumber) || dto.TaxNumber.Length != 14)
            {
                return Result<CourierDTO>.Failure("CNPJ inválido", ResultErrorType.ValidationError);
            }

            if (dto.BirthDate >= DateTime.UtcNow.AddYears(-18))
            {
                return Result<CourierDTO>.Failure("O entregador deve ter pelo menos 18 anos", ResultErrorType.ValidationError);
            }

            User? user = await userRepository.GetByIdAsync(dto.Identifier, cancellationToken);

            if (user is not null)
            {
                return Result<CourierDTO>.Failure("Já existe um usuário com este identificador no sistema", ResultErrorType.BusinessError);
            }

            user = await userRepository.GetByTaxNumberAsync(dto.TaxNumber, cancellationToken);

            if (user is not null)
            {
                return Result<CourierDTO>.Failure("Já existe um usuário com este CNPJ", ResultErrorType.BusinessError);
            }

            user = await userRepository.GetByLicenseNumberAsync(dto.DriverLicenseNumber, cancellationToken);

            if (user is not null)
            {
                return Result<CourierDTO>.Failure("Já existe um usuário com esta CNH", ResultErrorType.BusinessError);
            }

            string imageIdentifier = await imageStorage.UploadImageAsBase64Async(dto.DriverLicenseImageBase64, cancellationToken);

            user = new()
            {
                Id = dto.Identifier,
                Name = dto.Name,
                TaxNumber = dto.TaxNumber,
                BirthDate = dto.BirthDate,
                DriverLicenseNumber = dto.DriverLicenseNumber,
                DriverLicenseType = dto.DriverLicenseType,
                DriverLicenseImageIdentifier = imageIdentifier,
                Type = UserType.Courier,
            };

            await userRepository.AddAsync(user, cancellationToken);

            CourierDTO resultDto = new()
            {
                Identifier = user.Id,
                Name = user.Name,
                TaxNumber = user.TaxNumber,
                BirthDate = DateOnly.FromDateTime(user.BirthDate),
                DriverLicenseNumber = user.DriverLicenseNumber,
                DriverLicenseType = user.DriverLicenseType,
                DriverLicenseImageBase64 = dto.DriverLicenseImageBase64,
            };

            return Result<CourierDTO>.Success(resultDto);
        }
    }
}
