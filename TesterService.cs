using Grpc.Core;
using Xunit;
using Moq;
using Xunit.Sdk;
using Grpc.Core.Testing;



namespace GrpcGreeter.Services
{
    public class TesterService
    {

        private readonly IGreeter _greeter;

        public TesterService(IGreeter greeter)
        {
            _greeter = greeter;
        }

        public Task<HelloReply> SayHelloUnary(HelloRequest request)
        {
            var message = _greeter.Greet(request.Name);
            return Task.FromResult(new HelloReply { Message = message });
        }

        public async Task SayHelloServerStreaming(HelloRequest request,
    IServerStreamWriter<HelloReply> responseStream, ServerCallContext context)
        {
            var i = 0;
            while (!context.CancellationToken.IsCancellationRequested)
            {
                //var message = _greeter.Greet($"{request.Name} {++i}");
                var message = _greeter.Greet(request.Name);
                await responseStream.WriteAsync(new HelloReply { Message = message });

                await Task.Delay(1000);
            }
        }

    }
}
