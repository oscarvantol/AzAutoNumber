﻿using System.IO;
using AutoNumber.Interfaces;
using AutoNumber.Options;
using Azure.Storage.Blobs;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using NUnit.Framework;

namespace AutoNumber.IntegrationTests
{
    [TestFixture]
    public class DependencyInjectionTest
    {
        public IConfigurationRoot Configuration = new ConfigurationBuilder()
            .SetBasePath(Directory.GetCurrentDirectory())
            .AddJsonFile("appsettings.json", true, true).Build();

        private ServiceProvider GenerateServiceProvider()
        {
            var serviceCollection = new ServiceCollection();
            serviceCollection.AddSingleton(new BlobServiceClient("UseDevelopmentStorage=true"));
            serviceCollection.AddSingleton<IConfiguration>(Configuration);
            serviceCollection.AddAutoNumber(Configuration, builder => {

                return builder.UseDefaultContainerName()
                    .UseDefaultStorageAccount()
                    .SetBatchSize(50)
                    .SetMaxWriteAttempts(25)
                    .Options;
            });
            return serviceCollection.BuildServiceProvider();
        }

        [Test]
        public void OptionsBuilderShouldGenerateOptions()
        {
            var serviceProvider = GenerateServiceProvider();
            var optionsBuilder = new AutoNumberOptionsBuilder(serviceProvider.GetService<IConfiguration>());

            optionsBuilder.SetBatchSize(5);
            Assert.AreEqual(5, optionsBuilder.Options.BatchSize);

            optionsBuilder.SetMaxWriteAttempts(10);
            Assert.AreEqual(10, optionsBuilder.Options.MaxWriteAttempts);

            optionsBuilder.UseDefaultContainerName();
            Assert.AreEqual("unique-urls", optionsBuilder.Options.StorageContainerName);

            optionsBuilder.UseContainerName("test");
            Assert.AreEqual("test", optionsBuilder.Options.StorageContainerName);

            optionsBuilder.UseDefaultStorageAccount();
            Assert.AreEqual(null, optionsBuilder.Options.StorageAccountConnectionString);

            optionsBuilder.UseStorageAccount("test");
            Assert.AreEqual("test123", optionsBuilder.Options.StorageAccountConnectionString);

            optionsBuilder.UseStorageAccount("test-22");
            Assert.AreEqual("test-22", optionsBuilder.Options.StorageAccountConnectionString);
        }

        [Test]
        public void ShouldCraeteUniqueIdGenerator()
        {
            var serviceProvider = GenerateServiceProvider();

            var uniqueId = serviceProvider.GetService<IUniqueIdGenerator>();

            Assert.NotNull(uniqueId);
        }


        [Test]
        public void ShouldResolveUniqueIdGenerator()
        {
            var serviceCollection = new ServiceCollection();
            serviceCollection.AddSingleton(new BlobServiceClient("UseDevelopmentStorage=true"));

            serviceCollection.AddAutoNumber(Configuration, x =>
            {
                return x.UseContainerName("ali")
                    .UseDefaultStorageAccount()
                    .SetBatchSize(10)
                    .SetMaxWriteAttempts()
                    .Options;
            });

            var service = serviceCollection.BuildServiceProvider()
                .GetService<IUniqueIdGenerator>();

            Assert.NotNull(service);
        }
    }
}