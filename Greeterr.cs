namespace GrpcGreeter.Services
{
    public class Greeterr : IGreeter
    {
        private readonly ILogger<Greeterr> _logger;

        public Greeterr(ILoggerFactory loggerFactory)
        {
            _logger = loggerFactory.CreateLogger<Greeterr>();
        }

        public string Greet(string name)
        {
            _logger.LogInformation($"Creating greeting to {name}");
            return $"Hello {name}";
        }
    }
}
