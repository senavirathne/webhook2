using System;
using System.Globalization;
using System.Net.Http;
using System.Threading;
using System.Threading.Channels;
using System.Threading.Tasks;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace GithubService
{
    public class PackagistService : BackgroundService
    {
        private readonly Channel<string> _channel;
        private readonly ILogger<PackagistService> _logger;
        private readonly IHttpClientFactory _factory;


        public PackagistService(ILogger<PackagistService> logger, IHttpClientFactory factory)
        {
            // _channel = Channel.CreateBounded<string>(32);// <== bounded??
            _channel = Channel.CreateUnbounded<string>();
            _logger = logger;
            _factory = factory;
        }

        private ChannelReader<string> Reader => _channel.Reader;
        private ChannelWriter<string> Writer => _channel.Writer;

        public ValueTask Notify(string name)
        {
            return Writer.WriteAsync(name);
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _logger.LogInformation("Start processing message");
            while (await _channel.Reader.WaitToReadAsync(stoppingToken))
            {
                var attempt = 1;
                Reader.TryRead(out var name);

                while (attempt <= 3)
                {
                    var client = _factory.CreateClient("PackagistService");
                    var res = await client.PostAsync($"/Packages/{name}/webhook",
                        new StringContent(DateTime.Now.ToString(CultureInfo.CurrentCulture)), stoppingToken);
                    if (res.IsSuccessStatusCode)
                    {
                        _logger.LogInformation("message successfully pinged for {Name}", name);
                        break;
                    }

                    if (attempt == 3)
                    {
                        _logger.LogWarning("Unable to send ping for {Name}", name);
                    }


                    ++attempt;
                }
                
            }

            // await _packageService.WebHook(name)

            _logger.LogInformation("Ping message processing end");
            // reader => message consume asynchronously
            //  message => http request =>packagist service (ping mess)
        }
    }
}