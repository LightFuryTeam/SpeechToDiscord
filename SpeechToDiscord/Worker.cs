using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using SpeechToDiscord.Services;

namespace SpeechToDiscord
{
    public class Worker : BackgroundService
    {
        private readonly ILogger<Worker> _logger;

        public Worker(ILogger<Worker> logger)
        {
            _logger = logger;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            var discordService = new DiscordService("NzI5MjU5NzU1ODYzODAxOTQ2.XwGcnQ.6goBosxHNX7o02qThz-AcJPX32g");

            await discordService.Start();

            while (!stoppingToken.IsCancellationRequested)
            {
                _logger.LogInformation("Worker running at: {time}", DateTimeOffset.Now);
                await Task.Delay(1000, stoppingToken);
            }

            await discordService.Stop();
        }
    }
}
