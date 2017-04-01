// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Threading.Tasks;

namespace Microsoft.Extensions.HealthChecks
{
    public abstract class HealthCheck<TRequirement> : CachedHealthCheck where TRequirement : IHealthCheckRequirement
    {
        protected    override ValueTask<IHealthCheckResult> ExecuteCheckAsync(HealthCheckContext context)
        {
            return ExecuteCheckAsync(context, (TRequirement)context.Requirement);
        }

        protected abstract ValueTask<IHealthCheckResult> ExecuteCheckAsync(HealthCheckContext context, TRequirement requirement);
    }
}
