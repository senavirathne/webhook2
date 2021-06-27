using System.Threading;
using System.Threading.Channels;
using System.Threading.Tasks;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace Packagist_Service
{
    public class PingQueueService : BackgroundService
    {
        private readonly Channel<string> _channel;
        private readonly PackageService _packageService;
        private readonly ILogger<PingQueueService> _logger;

        public PingQueueService(PackageService packageService, ILogger<PingQueueService> logger)
        {
            _packageService = packageService;
            _logger = logger;
            // _channel = Channel.CreateBounded<string>(32);

            _channel = Channel.CreateUnbounded<string>();


        }

        public ChannelReader<string> Reader => _channel.Reader;
        public ChannelWriter<string> Writer => _channel.Writer;

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _logger.LogInformation("Start processing ping message");
            while (await  _channel.Reader.WaitToReadAsync(stoppingToken))
            {
                _channel.Reader.TryRead(out var name);
                await _packageService.WebHook(name);
            } 
            _logger.LogInformation("Ping message processing end");
        } 
    }
}