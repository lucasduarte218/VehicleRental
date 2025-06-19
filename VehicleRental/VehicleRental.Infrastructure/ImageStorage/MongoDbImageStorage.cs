using Microsoft.Extensions.Logging;
using MongoDB.Bson;
using MongoDB.Driver;
using VehicleRental.Application.Interfaces.Integration;

namespace VehicleRental.Infrastructure.ImageStorage
{
    public class MongoDbImageStorage(IMongoDatabase database, ILogger<MongoDbImageStorage> logger) : IImageStorage
    {
        private readonly IMongoCollection<BsonDocument> _imagesCollection = database.GetCollection<BsonDocument>("Images");

        public async Task<string?> GetImageAsBase64Async(string fileIdentifier, CancellationToken cancellationToken = default)
        {
            try
            {
                logger.LogDebug("Buscando imagem com o identificador {FileIdentifier} no MongoDB.", fileIdentifier);
                FilterDefinition<BsonDocument> filter = Builders<BsonDocument>.Filter.Eq("_id", fileIdentifier);
                BsonDocument doc = await _imagesCollection.Find(filter).FirstOrDefaultAsync(cancellationToken);
                logger.LogDebug("Busca concluída para o identificador {FileIdentifier}. Encontrado: {Found}", fileIdentifier, doc != null);
                return doc?["Base64"]?.AsString;
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Erro ao buscar imagem com o identificador {FileIdentifier}.", fileIdentifier);
                throw;
            }
        }

        public async Task<string> UploadImageAsBase64Async(string base64Image, CancellationToken cancellationToken = default)
        {
            try
            {
                string id = ObjectId.GenerateNewId().ToString();
                logger.LogDebug("Iniciando upload da imagem. Novo identificador: {Id}", id);
                BsonDocument doc = new()
            {
                { "_id", id },
                { "Base64", base64Image }
            };
                await _imagesCollection.InsertOneAsync(doc, null, cancellationToken);
                logger.LogDebug("Upload concluído para o identificador {Id}.", id);
                return id;
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Erro ao fazer upload da imagem.");
                throw;
            }
        }

        public async Task RemoveAsync(string fileIdentifier, CancellationToken cancellationToken = default)
        {
            try
            {
                logger.LogDebug("Removendo imagem com o identificador {FileIdentifier} do MongoDB.", fileIdentifier);
                FilterDefinition<BsonDocument> filter = Builders<BsonDocument>.Filter.Eq("_id", fileIdentifier);
                await _imagesCollection.DeleteOneAsync(filter, cancellationToken);
                logger.LogDebug("Remoção concluída para o identificador {FileIdentifier}.", fileIdentifier);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Erro ao remover imagem com o identificador {FileIdentifier}.", fileIdentifier);
                throw;
            }
        }
    }
}
