# Az AutoNumber

This project is forked from [0x414c49/AzureAutoNumber](0x414c49/AzureAutoNumber)

---

[![NuGet version (AzureAutoNumber)](https://img.shields.io/nuget/v/AzAutoNumber.svg?style=flat-square)](https://www.nuget.org/packages/AzAutoNumber/)

High performance, distributed unique thread-safe id generator for Azure.

- Human-friendly generated ids (number)
- High performant and fast
- 100% guarantee that won't cause any duplicate ids

## How to use

The project is rely on Azure Blob Storage. `AutoNumber` package will generate ids by using a single text file on the Azure Blob Storage.


```
var blobServiceClient = new BlobServiceClient(connectionString);

var blobOptimisticDataStore = new BlobOptimisticDataStore(blobServiceClient, "unique-ids");

var idGen = new UniqueIdGenerator(blobOptimisticDataStore);

// generate ids with different scopes

var id = idGen.NextId("urls");
var id2 = idGen.NextId("orders");
```

### With Microsoft DI
The project has an extension method to add it and its dependencies to Microsoft ASP.NET DI. ~~The only caveat is you need to registry type of  `BlobServiceClient` in DI before registring `AutoNumber`.~~


Use options builder to configure the service, take into account the default settings will read from `appsettings.json`.

```
services.AddAutoNumber(Configuration, x =>
{
	return x.UseContainerName("container-name")
	 .UseStorageAccount("connection-string-or-connection-string-name")
   //.UseBlobServiceClient(blobServiceClient)
	 .SetBatchSize(10)
	 .SetMaxWriteAttempts(100)
	 .Options;
});
```

#### Inject `IUniqueIdGenerator` in constructor

```
public class Foo
{
  public Foo(IUniqueIdGenerator idGenerator)
  {
      _idGenerator = idGenerator;
  }
}
```

### Configuration
These are default configuration for `AutoNumber`. If you prefer registering AutoNumber with `AddAddNumber` method, these options can be set via `appsettings.json`.

```
{
  "AutoNumber": {
    "BatchSize": 50,
    "MaxWriteAttempts": 25,
    "StorageContainerName": "unique-urls"
  }
}
```


## Credits
Most of the credits of this library goes to [Tatham Oddie](https://tatham.blog/2011/07/14/released-snowmaker-a-unique-id-generator-for-azure-or-any-other-cloud-hosting-environment/) for making SnowMaker.
In this post https://itnext.io/generate-auto-increment-id-on-azure-62cc962b6fa6 Ali Bahraminezhad explained how he forked his work and made lots of change to make it available on .NET Standard (2.0 and 2.1). SnowMaker is out-dated and was using very old version of Azure Packages.

Ali archived the repo and the nuget package. Now I'm forking his work and making it available for .NET 6.0 and beyond as long as I have a dependency on it myself.


