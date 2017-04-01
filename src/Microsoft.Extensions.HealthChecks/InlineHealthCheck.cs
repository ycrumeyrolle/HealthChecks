// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Threading;
using System.Threading.Tasks;

namespace Microsoft.Extensions.HealthChecks
{
    public class InlineHealthCheck : CachedHealthCheck
    {
        protected InlineHealthCheck(Func<HealthCheckContext, ValueTask<IHealthCheckResult>> check)
        {
            Guard.ArgumentNotNull(nameof(check), check);

            Check = check;
        }

        protected Func<HealthCheckContext, ValueTask<IHealthCheckResult>> Check { get; }

        protected override ValueTask<IHealthCheckResult> ExecuteCheckAsync(HealthCheckContext context)
            => Check(context);

        public static InlineHealthCheck FromCheck(Func<HealthCheckContext, IHealthCheckResult> check)
            => new InlineHealthCheck(ctx => new ValueTask<IHealthCheckResult>(check(ctx)));

        public static InlineHealthCheck FromTaskCheck(Func<HealthCheckContext, Task<IHealthCheckResult>> check)
            => new InlineHealthCheck(ctx => new ValueTask<IHealthCheckResult>(check(ctx)));

        public static InlineHealthCheck FromValueTaskCheck(Func<HealthCheckContext, ValueTask<IHealthCheckResult>> check)
            => new InlineHealthCheck(ctx => check(ctx));
    }
}
