// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;

namespace Microsoft.Extensions.HealthChecks
{
    public class HealthCheckService : IHealthCheckService
    {
        private readonly ILogger<HealthCheckService> _logger;
        private readonly IHealthCheckFactory _healthCheckFactory;
        private readonly HealthCheckDescriptor[] _checkDescriptors;
        private readonly IHealthCheck[] _checks;

        public HealthCheckService(
            IHealthCheckFactory healthCheckFactory,
            HealthCheckBuilder builder,
            ILogger<HealthCheckService> logger)
        {
            Guard.ArgumentNotNull(nameof(healthCheckFactory), healthCheckFactory);
            Guard.ArgumentNotNull(nameof(builder), builder);
            Guard.ArgumentNotNull(nameof(logger), logger);

            _healthCheckFactory = healthCheckFactory;
            _logger = logger;
            _checkDescriptors = builder.Build().ToArray();
            _checks = _checkDescriptors.Select(_healthCheckFactory.Create).ToArray();
        }

        public async Task<CompositeHealthCheckResult> CheckHealthAsync(CheckStatus partiallyHealthyStatus, CancellationToken cancellationToken)
        {
            var logMessage = new StringBuilder();
            var result = new CompositeHealthCheckResult(partiallyHealthyStatus);

            try
            {
                var healthCheckTasks = new Task<IHealthCheckResult>[_checkDescriptors.Length];
                for (int i = 0; i < _checkDescriptors.Length; i++)
                {
                    healthCheckTasks[i] = _checks[i]
                        .CheckAsync(new HealthCheckContext(_checkDescriptors[i].Requirement, cancellationToken))
                        .AsTask();
                }

                var results = await Task.WhenAll(healthCheckTasks).ConfigureAwait(false);

                for (int i = 0; i < _checkDescriptors.Length; i++)
                {
                    var healthCheckResult = results[i];
                    var key = _checkDescriptors[i].Requirement.Name;
                    try
                    {
                        logMessage.AppendLine($"HealthCheck: {key} : {healthCheckResult.CheckStatus}");
                        result.Add(key, healthCheckResult);
                    }
                    catch (Exception ex)
                    {
                        logMessage.AppendLine($"HealthCheck: {key} : Exception {ex.GetType().FullName} thrown");
                        result.Add(key, CheckStatus.Unhealthy, $"Exception during check: {ex.GetType().FullName}");
                    }
                }

                if (logMessage.Length == 0)
                    logMessage.AppendLine("HealthCheck: No checks have been registered");

                _logger.Log((result.CheckStatus == CheckStatus.Healthy ? LogLevel.Information : LogLevel.Error), 0, logMessage.ToString(), null, MessageFormatter);
                return result;
            }
            catch (TaskCanceledException)
            {
                result = new CompositeHealthCheckResult();
                result.Add("*", CheckStatus.Unhealthy, "The health check operation timed out");
                _logger.Log(LogLevel.Warning, 0, result.Description, null, MessageFormatter);
                return result;
            }
        }

        // This entry point is for non-DI (we leave the single constructor in place for DI)
        public static HealthCheckService FromBuilder(HealthCheckBuilder builder, ILogger<HealthCheckService> logger)
            => new HealthCheckService(new SimpleHealthCheckFactory(), builder, logger);

        private static string MessageFormatter(string state, Exception error) => state;
        
        class SimpleHealthCheckFactory : IHealthCheckFactory
        {
            private readonly Dictionary<Type, IHealthCheck> _singletons = new Dictionary<Type, IHealthCheck>();

            public IHealthCheck Create(HealthCheckDescriptor descriptor)
            {
                if (!_singletons.TryGetValue(descriptor.Type, out var result))
                {
                    result = (IHealthCheck)Activator.CreateInstance(descriptor.Type);
                    _singletons[descriptor.Type] = result;
                }

                return result;
            }
        }
    }
}
