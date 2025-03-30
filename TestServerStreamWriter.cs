using System.Threading.Channels;
using Grpc.Core;

namespace GrpcGreeter.Services
{
    public class TestServerStreamWriter<T> : IServerStreamWriter<T> where T : class
    {
        private readonly ServerCallContext _serverCallContext;
        private readonly Channel<T> _channel;
        public WriteOptions? WriteOptions { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }

        public TestServerStreamWriter(ServerCallContext serverCallContext)
        {
            _channel = System.Threading.Channels.Channel.CreateUnbounded<T>();

            _serverCallContext = serverCallContext;
        }

        public void Complete()
        {
            _channel.Writer.Complete();
        }

        public IAsyncEnumerable<T> ReadAllAsync()
        {
            return _channel.Reader.ReadAllAsync();
        }

        public async Task<T?> ReadNextAsync()
        {
            if (await _channel.Reader.WaitToReadAsync())
            {
                _channel.Reader.TryRead(out var message);
                return message;
            }
            else
            {
                return null;
            }
        }
        public Task WriteAsync(T message)
        {
            if (_serverCallContext.CancellationToken.IsCancellationRequested)
            {
                return Task.FromCanceled(_serverCallContext.CancellationToken);
            }

            if (!_channel.Writer.TryWrite(message))
            {
                throw new InvalidOperationException("Unable to write message.");
            }

            return Task.CompletedTask;
        }
    }
}
