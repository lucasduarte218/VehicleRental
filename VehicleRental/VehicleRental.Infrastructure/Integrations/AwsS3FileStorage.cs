using Amazon.S3;
using Amazon.S3.Model;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using VehicleRental.Application.Interfaces.Integration;
using VehicleRental.Infrastructure.Settings;

namespace VehicleRental.Infrastructure.Integrations;

public class AwsS3FileStorage(IAmazonS3 s3Client, IOptions<AwsS3Settings> options, ILogger<AwsS3FileStorage> logger) : IImageStorage
{
    private readonly string _bucketName = options.Value.BucketName ?? throw new ArgumentNullException("A configuração do bucket S3 é obrigatória");

    public async Task<string?> GetImageAsBase64Async(string fileIdentifier, CancellationToken cancellationToken = default)
    {
        logger.LogDebug("Iniciando obtenção da imagem '{FileIdentifier}' do bucket S3 '{BucketName}'.", fileIdentifier, _bucketName);
        try
        {
            GetObjectRequest request = new()
            {
                BucketName = _bucketName,
                Key = fileIdentifier
            };

            using GetObjectResponse response = await s3Client.GetObjectAsync(request, cancellationToken);
            using MemoryStream memoryStream = new();
            await response.ResponseStream.CopyToAsync(memoryStream, cancellationToken);

            byte[] bytes = memoryStream.ToArray();
            logger.LogDebug("Imagem '{FileIdentifier}' obtida com sucesso do bucket S3.", fileIdentifier);
            return Convert.ToBase64String(bytes);
        }
        catch (AmazonS3Exception ex) when (ex.StatusCode == System.Net.HttpStatusCode.NotFound)
        {
            logger.LogDebug("Imagem '{FileIdentifier}' não encontrada no bucket S3.", fileIdentifier);
            return null;
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Erro ao obter imagem '{FileIdentifier}' do bucket S3.", fileIdentifier);
            throw;
        }
    }

    public async Task RemoveAsync(string fileIdentifier, CancellationToken cancellationToken = default)
    {
        logger.LogDebug("Iniciando remoção da imagem '{FileIdentifier}' do bucket S3 '{BucketName}'.", fileIdentifier, _bucketName);
        try
        {
            DeleteObjectRequest request = new()
            {
                BucketName = _bucketName,
                Key = fileIdentifier
            };

            await s3Client.DeleteObjectAsync(request, cancellationToken);
            logger.LogDebug("Imagem '{FileIdentifier}' removida com sucesso do bucket S3.", fileIdentifier);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Erro ao remover imagem '{FileIdentifier}' do bucket S3.", fileIdentifier);
            throw;
        }
    }

    public async Task<string> UploadImageAsBase64Async(string base64Image, CancellationToken cancellationToken = default)
    {
        logger.LogDebug("Iniciando upload de imagem para o bucket S3 '{BucketName}'.", _bucketName);
        try
        {
            byte[] imageBytes = Convert.FromBase64String(base64Image);

            string fileIdentifier = $"{Guid.NewGuid()}.jpg";

            PutObjectRequest request = new()
            {
                BucketName = _bucketName,
                Key = fileIdentifier,
                InputStream = new MemoryStream(imageBytes),
                ContentType = "image/jpeg"
            };

            await s3Client.PutObjectAsync(request, cancellationToken);

            logger.LogDebug("Upload da imagem '{FileIdentifier}' realizado com sucesso no bucket S3.", fileIdentifier);
            return fileIdentifier;
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Erro ao fazer upload da imagem para o bucket S3.");
            throw;
        }
    }
}
