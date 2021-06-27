using Google.Protobuf.WellKnownTypes;
using Grpc.Core;
using Grpc.Net.Client;
using ProductGrpc.Protos;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace ProductGrpcClient
{
    class Program
    {
        static async Task Main(string[] args)
        {
            Console.WriteLine("waiting for Server is running");
            Thread.Sleep(2000);
            using var channel = GrpcChannel.ForAddress("https://localhost:5001");
            var client = new ProductProtoService.ProductProtoServiceClient(channel);

            await GetProductAsync(client);
            await GetAllProducts(client);
            await AddProductAsync(client);

            await UpdateProductAsync(client);
            await DeleteProductAsync(client); 

            await InsertBulkProduct(client);
            await GetAllProducts(client);

            Console.ReadLine();
            
        }      

        private static async Task GetProductAsync(ProductProtoService.ProductProtoServiceClient client)
        {
            //GetproductAsync
            Console.WriteLine("GetProductAsync Started");
            var response = await client.GetProductAsync(
                new GetProductRequest
                {
                    ProductId = 1
                });
            Console.WriteLine("GetProductAsync Response :" + response.ToString());
        }
        private static async Task GetAllProducts(ProductProtoService.ProductProtoServiceClient client)
        {
            //GetAllProduct
            //Console.WriteLine("GetAllProduct Started");
            //using(var clientData = client.GetAllProducts(new GetAllProductsRequest()))
            //{
            //    while (await clientData.ResponseStream.MoveNext(new System.Threading.CancellationToken()))
            //    {
            //        var currentProduct = clientData.ResponseStream.Current;
            //        Console.WriteLine(currentProduct);
            //    }
            //}

            //GetAll Products with C# 9 feature #Refactor
            Console.WriteLine("GetAllProducts with c#9 feature");
            using var clientData = client.GetAllProducts(new GetAllProductsRequest());
            await foreach (var responseData in clientData.ResponseStream.ReadAllAsync())
            {
                Console.WriteLine(responseData);
            }
        }       
        private static async Task AddProductAsync(ProductProtoService.ProductProtoServiceClient client)
        {
            Console.WriteLine("AddProductAsync Started....");
            var addProductResponse = await client.AddProductAsync(

                new AddProductRequest
                {
                    Product = new ProductModel
                    {
                        Name = "Red",
                        Description = "New Red phone Mi10T",
                        Price = 699,
                        Status = ProductStatus.Instock,
                        CreatedTime = Timestamp.FromDateTime(DateTime.UtcNow)
                    }
                });

            Console.WriteLine("AddProduct Response: " + addProductResponse.ToString());
        }
        private static async Task UpdateProductAsync(ProductProtoService.ProductProtoServiceClient client)
        {
            //UpadteProductAsync
            Console.WriteLine("UpadteProductAsync started.....");
            var upadteProductResponse = await client.UpdateProductAsync(

                new UpdateProductRequest
                {
                    Product = new ProductModel
                    {
                        ProductId = 1,
                        Name = "Red",
                        Description = "New Red phone Mi10T",
                        Price = 749,
                        Status = ProductStatus.Instock,
                        CreatedTime = Timestamp.FromDateTime(DateTime.UtcNow)
                    }
                });
            Console.WriteLine("UpadteProductAsync Response: " + upadteProductResponse.ToString());
        }
        private static async Task DeleteProductAsync(ProductProtoService.ProductProtoServiceClient client)
        {
            //DeleteProductAsync
            Console.WriteLine("DeleteProductAsync started");
            var deleteProductResponse = await client.DeleteProductAsync(
                new DeleteProductRequest
                {
                    ProductId = 3
                });

            Console.WriteLine("DeleteProductAsync Response: " + deleteProductResponse.Success.ToString());
            Thread.Sleep(1000);
        }

        private static async Task InsertBulkProduct(ProductProtoService.ProductProtoServiceClient client)
        {
            //InsertBulkProduct
            Console.WriteLine("InsertBulkProduct started....");
            using var clientBulk = client.InsertBulkProduct();

            for (int i = 0; i < 3; i++)
            {
                var productModel = new ProductModel
                {
                    Name = $"Product {i}",
                    Description = $"Bulk insert product {i}",
                    Price = 399,
                    Status = ProductStatus.Instock,
                    CreatedTime = Timestamp.FromDateTime(DateTime.UtcNow)
                };
                await clientBulk.RequestStream.WriteAsync(productModel);
            }

            await clientBulk.RequestStream.CompleteAsync();

            var responseBulk = await clientBulk;

            Console.WriteLine("InsertBulkProduct Response: " + responseBulk.ToString());
        }
    }
}
