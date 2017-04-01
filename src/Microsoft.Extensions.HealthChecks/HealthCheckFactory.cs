// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;

namespace Microsoft.Extensions.HealthChecks
{
    public class HealthCheckFactory : IHealthCheckFactory
    {
        private readonly IServiceProvider _serviceProvider;

        public HealthCheckFactory(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }

        public IHealthCheck Create(HealthCheckDescriptor descriptor)
        {
            return (IHealthCheck)_serviceProvider.GetService(descriptor.Type);
        }
    }
}
