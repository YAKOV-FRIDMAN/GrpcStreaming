using Grpc.Core;
using Grpc.Net.Client;
using GrpcStreaming;
using System;
using System.Threading.Tasks;

namespace GrpcClient
{
    internal class Program
    {
        static async Task Main(string[] args)
        {
            Console.WriteLine("Hello World!");
            Console.ReadLine();
            using var chanel = GrpcChannel.ForAddress("http://localhost:5000");
            var client = new StreamService.StreamServiceClient(chanel);
            var userName = "kobi";
            using var  stream  = client.StartStreaming();
            var respone = Task.Run(async () => {
                await foreach (var item in stream.ResponseStream.ReadAllAsync())
                {
                    Console.WriteLine(item.Message);
                }
            });
            Console.WriteLine("enteer message to stream to server");
            while (true)
            {
                var text = Console.ReadLine();
                if (string.IsNullOrEmpty(text))
                    break;
                await stream.RequestStream.WriteAsync(new StrameMessage()
                {
                    Message = text,
                    Username = userName,
                });
            }
            Console.WriteLine("disconect...");
            await stream.RequestStream.CompleteAsync();
            await respone;
        }
    }
}
