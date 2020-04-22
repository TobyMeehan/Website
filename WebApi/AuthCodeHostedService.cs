using DataAccessLibrary.Data;
using Microsoft.Extensions.Hosting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace WebApi
{
    public class AuthCodeHostedService : BackgroundService
    {
        private readonly IConnectionProcessor _connectionProcessor;

        public AuthCodeHostedService(IConnectionProcessor connectionProcessor)
        {
            _connectionProcessor = connectionProcessor;
        }

        protected override async Task ExecuteAsync(CancellationToken cancellationToken)
        {
            while (!cancellationToken.IsCancellationRequested)
            {
                await _connectionProcessor.DeleteInvalidAuthCodes();
                await Task.Delay(1000 * 60 * 60 * 24, cancellationToken); // Repeat every 24 hours
            }
        }
    }
}
