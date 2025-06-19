using VehicleRental.Application.DTOs.Courier;
using VehicleRental.Application.DTOs.Courier.Update;
using VehicleRental.Application.Interfaces.Integration;
using VehicleRental.Application.Interfaces.UseCases.Courier;
using VehicleRental.Domain.Entities;
using VehicleRental.Domain.Entities.Base;
using VehicleRental.Domain.Enums;
using VehicleRental.Domain.Interfaces.Repositories;

namespace VehicleRental.Application.UseCases.Courier
{
    public class UpdateCourierUseCase(IUserRepository userRepository, IImageStorage fileStorage) : IUpdateCourierUseCase
    {
        public async Task<Result<CourierDTO>> HandleAsync(string identifier, UpdateCourierDTO dto, CancellationToken cancellationToken = default)
        {
            User? user = await userRepository.GetByIdAsync(identifier, cancellationToken);

            if (user is null)
            {
                return Result<CourierDTO>.Failure("Usuário não encontrado", ResultErrorType.NotFound);
            }

            if (!string.IsNullOrWhiteSpace(dto.Name))
            {
                user.Name = dto.Name;
            }

            if (!string.IsNullOrWhiteSpace(dto.TaxNumber) && dto.TaxNumber.Length == 14)
            {
                user.TaxNumber = dto.TaxNumber;
            }

            if (dto.BirthDate.HasValue && dto.BirthDate.Value < DateTime.UtcNow.AddYears(-18))
            {
                user.BirthDate = dto.BirthDate.Value;
            }

            if (!string.IsNullOrWhiteSpace(dto.DriverLicenseNumber))
            {
                if (await IsDriverLicenseNumberInUseAsync(dto.DriverLicenseNumber, identifier, cancellationToken))
                {
                    return Result<CourierDTO>.Failure("Já existe um usuário com este número de CNH", ResultErrorType.BusinessError);
                }

                user.DriverLicenseNumber = dto.DriverLicenseNumber;
            }

            if (dto.DriverLicenseType is not null)
            {
                user.DriverLicenseType = dto.DriverLicenseType;
            }

            if (!string.IsNullOrWhiteSpace(dto.DriverLicenseImageBase64))
            {
                await fileStorage.RemoveAsync(user.DriverLicenseImageIdentifier, cancellationToken);
                user.DriverLicenseImageIdentifier = await fileStorage.UploadImageAsBase64Async(dto.DriverLicenseImageBase64, cancellationToken);
            }

            user.UpdatedAt = DateTime.UtcNow;

            await userRepository.UpdateAsync(user, cancellationToken);

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

        private async Task<bool> IsDriverLicenseNumberInUseAsync(string licenseNumber, string identifier, CancellationToken cancellationToken)
        {
            User? userWithSameDriverLicense = await userRepository.GetByLicenseNumberAsync(licenseNumber, cancellationToken);
            return userWithSameDriverLicense is not null && userWithSameDriverLicense.Id != identifier;
        }
    }
}
