
using Grpc.Core;
using Moq;
using Grpc.Core.Testing;
using Xunit;

using GrpcGreeter.Services;
using GrpcGreeter;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Castle.Core.Logging;

namespace GrpcXU
{
    public class UnitTest1
    {
        private readonly ILogger<GreeterService> _logger;
        private GreeterService createGreaterService() => new(_logger);


        [Fact]
        public async Task SayHelloUnaryTest()
        {
            
            var mockGreeter = new Mock<IGreeter>();
            mockGreeter.Setup(m => m.Greet("Joe")).Returns("Hello Joe");
            var service = new TesterService(mockGreeter.Object);
            var request = new HelloRequest {Name = "Joe"};
            var response = await service.SayHelloUnary(request);
            mockGreeter.Verify(v => v.Greet("Joe"));
            Assert.Equal("Hello Joe", response.Message);

        }

        [Fact]
        public async Task SayHelloServerStreamingTest() { 
            var mockGreeter = new Mock<IGreeter>();
            mockGreeter.Setup(m => m.Greet("Joe")).Returns("Hello Joe");
            var service = new TesterService(mockGreeter.Object);


            var cts = new CancellationTokenSource();
            var callContext = GrpcGreeter.Services.TestServerCallContext.Create(cancellationToken: cts.Token);
            var responseStream = new TestServerStreamWriter<HelloReply>(callContext);
            var request = new HelloRequest { Name = "Joe" };
            using var call = service.SayHelloServerStreaming(request, responseStream, callContext);
            Assert.False(call.IsCompletedSuccessfully,"method should run until cancelled");
            cts.Cancel();
            await call;
            responseStream.Complete();
            var allMessages = new List<HelloReply>();
            await foreach (var message in responseStream.ReadAllAsync()) { 
                allMessages.Add(message);
            }
            Assert.True(allMessages.Count > 1);
           
        }

    }
}
