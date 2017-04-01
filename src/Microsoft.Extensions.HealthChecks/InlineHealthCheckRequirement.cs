// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Threading;
using System.Threading.Tasks;

namespace Microsoft.Extensions.HealthChecks
{
    internal class InlineHealthCheckRequirement : IHealthCheckRequirement
    {
        public InlineHealthCheckRequirement(string name, Func<HealthCheckContext, IHealthCheckResult> check)
            :this(name, ctx => new ValueTask<IHealthCheckResult>(check(ctx)))
        {
        }

        public InlineHealthCheckRequirement(string name, Func<HealthCheckContext, Task<IHealthCheckResult>> check)
            :this(name, ctx => new ValueTask<IHealthCheckResult>(check(ctx)))
        {
        }

        public InlineHealthCheckRequirement(string name, Func<HealthCheckContext, ValueTask<IHealthCheckResult>> check)
        {
            Name = name;
            Check = check;
        }
        
        public string Name { get; set; }

        public TimeSpan CacheDuration { get; set; }

        public Func<HealthCheckContext, ValueTask<IHealthCheckResult>> Check { get;  }
    }
}