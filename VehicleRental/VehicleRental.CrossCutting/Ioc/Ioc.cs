using Amazon;
using Amazon.Runtime;
using Amazon.S3;
using Amazon.SQS;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;
using MongoDB.Driver;
using VehicleRental.Application.Interfaces.Integration;
using VehicleRental.Application.Interfaces.Messaging;
using VehicleRental.Application.Interfaces.UseCases.Courier;
using VehicleRental.Application.Interfaces.UseCases.Rent;
using VehicleRental.Application.Interfaces.UseCases.Vehicle;
using VehicleRental.Application.Services;
using VehicleRental.Application.UseCases.Courier;
using VehicleRental.Application.UseCases.Rent;
using VehicleRental.Application.UseCases.Vehicle;
using VehicleRental.Domain.Interfaces.Repositories;
using VehicleRental.Domain.Interfaces.Services;
using VehicleRental.Domain.ValueObjects;
using VehicleRental.Infrastructure.Context;
using VehicleRental.Infrastructure.ImageStorage;
using VehicleRental.Infrastructure.Messaging;
using VehicleRental.Infrastructure.Repositories;
using VehicleRental.Infrastructure.Settings;

namespace VehicleRental.CrossCutting.Ioc
{
    public static class Ioc
    {
        public static IServiceCollection AddApplicationLayer(this IServiceCollection services)
        {
            services.AddUseCases();

            services.AddSingleton<IRentPricingCalculator, DefaultRentPricingCalculator>();

            services.Configure<List<RentContract>>(options =>
            {
            });

            services.AddSingleton<IRentContractCatalog, RentContractCatalog>(provider =>
            {
                var rentPlans = provider.GetRequiredService<IOptions<List<RentContract>>>().Value;
                return new RentContractCatalog(rentPlans);
            });

            return services;
        }

        private static IServiceCollection AddUseCases(this IServiceCollection services)
        {
            // Vehicle
            services.AddScoped<IFindVehicleUseCase, FindVehicleUseCase>();
            services.AddScoped<IInsertVehicleUseCase, InsertVehicleUseCase>();
            services.AddScoped<IUpdateVehicleUseCase, UpdateVehicleUseCase>();
            services.AddScoped<IGetVehicleUseCase, GetVehicleUseCase>();
            services.AddScoped<IDeleteVehicleUseCase, DeleteVehicleUseCase>();

            // Courier
            services.AddScoped<IInsertCourierUseCase, InsertCourierUseCase>();
            services.AddScoped<IUpdateCourierUseCase, UpdateCourierUseCase>();

            // Renting
            services.AddScoped<IInsertRentUseCase, InsertRentUseCase>();
            services.AddScoped<IGetRentUseCase, GetRentUseCase>();
            services.AddScoped<ICompleteRentUseCase, ICompleteRentUseCase>();

            return services;
        }


        public static IServiceCollection AddInfrastructureLayer(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddScoped<IVehicleRepository, VehicleRepository>();
            services.AddScoped<IUserRepository, UserRepository>();
            services.AddScoped<IRentRepository, RentRepository>();

            services.UseMongoDbAsDatabase(configuration);
            services.UseSQSAsMessageQueue(configuration);

            var imageStorageService = services.BuildServiceProvider()
                .GetRequiredService<IOptions<InfrastructureSettings>>().Value.ImageStorageService;

            switch (imageStorageService)
            {
                case "mongodb":
                    services.UseMongoAsImageStorage(configuration);
                    break;

                case "s3":
                    services.UseS3AsImageStorage(configuration);
                    break;

                default:
                    throw new ArgumentException($"Unsupported image storage service: {imageStorageService}");
            }

            return services;
        }

        private static IServiceCollection UseMongoDbAsDatabase(this IServiceCollection services, IConfiguration configuration)
        {
            services.RegisterMongoDb(configuration);

            MongoDBSettings mongoDbSettings = services.BuildServiceProvider()
                .GetRequiredService<IOptions<MongoDBSettings>>().Value
                ?? throw new Exception("MongoDbSettings not configured in appsettings.json");

            return services;
        }

        private static IServiceCollection UseSQSAsMessageQueue(this IServiceCollection services, IConfiguration configuration)
        {
            AwsSQSSettings mongoDbSettings = services.BuildServiceProvider()
                .GetRequiredService<IOptions<AwsSQSSettings>>().Value
                ?? throw new Exception("AwsSQSSettings not configured in appsettings.json");

            services.AddSingleton<IAmazonSQS>(sp =>
            {
                AmazonSQSConfig config = new()
                {
                    RegionEndpoint = RegionEndpoint.USEast1,
                };

                IOptions<AwsSQSSettings> options = sp.GetRequiredService<IOptions<AwsSQSSettings>>();

                AWSCredentials credentials = new BasicAWSCredentials(options.Value.Key, options.Value.Secret);

                return new AmazonSQSClient(credentials, config);
            });

            services.AddSingleton<IVehicleEventPublisher, AwsSQSVehicleEventPublisher>();

            return services;
        }

        private static IServiceCollection UseMongoAsImageStorage(this IServiceCollection services, IConfiguration configuration)
        {
            services.RegisterMongoDb(configuration);

            services.AddScoped<IImageStorage, MongoDbImageStorage>();

            return services;
        }

        private static IServiceCollection RegisterMongoDb(this IServiceCollection services, IConfiguration configuration)
        {
            var mongoDbSettings = services.BuildServiceProvider().GetRequiredService<IOptions<MongoDBSettings>>().Value ?? throw new Exception("MongoDbSettings not configured in appsettings.json");

            services.AddSingleton<IMongoClient>(sp =>
            {
                MongoDBSettings mongoDbSettings = sp.GetRequiredService<IOptions<MongoDBSettings>>().Value;
                MongoClientSettings clientSettings = MongoClientSettings.FromConnectionString(mongoDbSettings.ConnectionString);

                MongoClient mongoClient = new(clientSettings);

                return new MongoClient(clientSettings);
            });

            services.AddSingleton(sp =>
            {
                IMongoClient mongoClient = sp.GetRequiredService<IMongoClient>();

                MongoDBSettings mongoDbSettings = sp.GetRequiredService<IOptions<MongoDBSettings>>().Value;

                return mongoClient.GetDatabase(mongoDbSettings.Database);
            });

            return services;
        }


        private static IServiceCollection UseS3AsImageStorage(this IServiceCollection services, IConfiguration configuration)
        {
            var mongoDbSettings = services.BuildServiceProvider().GetRequiredService<IOptions<AwsS3Settings>>().Value ?? throw new Exception("AwsS3Settings not configured in appsettings.json");

            services.AddSingleton<IAmazonS3>(sp =>
            {
                AmazonS3Config amazonS3Config = new()
                {
                    RegionEndpoint = RegionEndpoint.USEast1,
                };

                AwsS3Settings awsS3Settings = sp.GetRequiredService<IOptions<AwsS3Settings>>().Value;

                AWSCredentials credentials = new BasicAWSCredentials(awsS3Settings.Key, awsS3Settings.Secret);

                return new AmazonS3Client(credentials, amazonS3Config);
            });

            services.AddScoped<IImageStorage, AwsS3ImageStorage>();

            return services;
        }

        public static void EnsureDatabaseCreated(this IHost app)
        {
            using IServiceScope scope = app.Services.CreateScope();
            AppDbContext context = scope.ServiceProvider.GetRequiredService<AppDbContext>();
            context.Database.EnsureCreated();
        }
    }
}

