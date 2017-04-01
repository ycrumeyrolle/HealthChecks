// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;

namespace Microsoft.Extensions.HealthChecks
{
    public class HealthCheckBuilder
    {
        private readonly List<HealthCheckDescriptor> _descriptors;
        private readonly IServiceCollection _serviceCollection;

        public HealthCheckBuilder(IServiceCollection serviceCollection)
        {
            _serviceCollection = serviceCollection;
            _descriptors = new List<HealthCheckDescriptor>();
            DefaultCacheDuration = TimeSpan.FromMinutes(5);
        }

        public IServiceCollection Services => _serviceCollection;

        public IEnumerable<HealthCheckDescriptor> Requirements => _descriptors;

        public TimeSpan DefaultCacheDuration { get; private set; }

        //public HealthCheckBuilder AddCheck(string name, Func<HealthCheckContext, ValueTask<IHealthCheckResult>> check)
        //{
        //    Guard.ArgumentNotNull(nameof(check), check);

        //    AddCheck<InlineHealthCheck>(new InlineHealthCheckRequirement(name, check));
        //    return this;
        //}

        public HealthCheckBuilder AddCheck(string name, Func<HealthCheckContext, Task<IHealthCheckResult>> check)
        {
            Guard.ArgumentNotNull(nameof(check), check);

            AddCheck<InlineHealthCheck>(new InlineHealthCheckRequirement(name, check));
            return this;
        }

        public HealthCheckBuilder AddCheck(string name, Func<HealthCheckContext, IHealthCheckResult> check)
        {
            Guard.ArgumentNotNull(nameof(check), check);

            AddCheck(InlineHealthCheck.FromCheck(check), new InlineHealthCheckRequirement(name, check));
            return this;
        }

        public HealthCheckBuilder AddCheck<TCheck>(TCheck check, IHealthCheckRequirement requirement) where TCheck : class, IHealthCheck
        {
            Guard.ArgumentNotNull(nameof(check), check);
            Guard.ArgumentNotNull(nameof(requirement), requirement);

            _serviceCollection.AddSingleton(check);
            _descriptors.Add(new HealthCheckDescriptor(check.GetType(), requirement));
            return this;
        }

        public HealthCheckBuilder AddCheck<TCheck>(IHealthCheckRequirement requirement) where TCheck : class, IHealthCheck
        {
            Guard.ArgumentNotNull(nameof(requirement), requirement);

            _serviceCollection.AddSingleton<TCheck>();
            _descriptors.Add(new HealthCheckDescriptor(typeof(TCheck), requirement));
            return this;
        }

        public HealthCheckBuilder WithDefaultCacheDuration(TimeSpan duration)
        {
            Guard.ArgumentValid(duration >= TimeSpan.Zero, nameof(duration), "Duration must be zero (disabled) or a positive duration");

            DefaultCacheDuration = duration;
            return this;
        }

        public IEnumerable<HealthCheckDescriptor> Build()
        {

            foreach (var descriptor in _descriptors)
            {
                descriptor.Requirement.CacheDuration = DefaultCacheDuration;
            }

            return _descriptors;
        }
    }
}
