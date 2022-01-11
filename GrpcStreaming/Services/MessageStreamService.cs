using Grpc.Core;
using Microsoft.Extensions.Logging;
using System;
using System.Threading.Tasks;

namespace GrpcStreaming.Services
{
    public class MessageStreamService : StreamService.StreamServiceBase
    {
        public ILogger<MessageStreamService> Logger;
        public MessageStreamService(ILogger<MessageStreamService> logger)
        {
            Logger = logger;
        }
        public override async Task StartStreaming(IAsyncStreamReader<StrameMessage> requestStream, IServerStreamWriter<StrameMessage> responseStream, ServerCallContext context)
        {
            if (requestStream != null)
            {
                if (!await requestStream.MoveNext())
                    return;
            }
            try
            {
                if (!string.IsNullOrEmpty(requestStream.Current.Message))
                {
                    if (string.Equals(requestStream.Current.Message, "qw!", StringComparison.OrdinalIgnoreCase))
                        return;

                }
                Console.WriteLine($"message from cleint: {requestStream.Current.Username}, message: {requestStream.Current.Message}");
                await responseStream.WriteAsync(new StrameMessage()
                {
                    Username = requestStream.Current.Username,
                    Message = $"Replay strem from server @: {DateTime.UtcNow:f} to {requestStream.Current.Username}"
                });
            }
            catch (Exception e)
            {
                Logger.LogInformation(e.Message);
            }
        }
    }
}
