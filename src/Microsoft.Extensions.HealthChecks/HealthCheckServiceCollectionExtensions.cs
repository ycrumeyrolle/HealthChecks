// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using Microsoft.Extensions.HealthChecks;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace Microsoft.Extensions.DependencyInjection
{
    public static class HealthCheckServiceCollectionExtensions
    {
        public static IServiceCollection AddHealthChecks(this IServiceCollection services, Action<HealthCheckBuilder> checkupAction)
        {
            var checkupBuilder = new HealthCheckBuilder(services);

            checkupAction.Invoke(checkupBuilder);

            services.AddSingleton(checkupBuilder);
            services.AddSingleton<IHealthCheckService, HealthCheckService>();
            services.AddSingleton<HealthCheckFactory>();
            return services;
        }
    }
}
