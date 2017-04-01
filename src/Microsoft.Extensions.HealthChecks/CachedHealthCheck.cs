﻿// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Threading;
using System.Threading.Tasks;

namespace Microsoft.Extensions.HealthChecks
{
    public abstract class CachedHealthCheck : IHealthCheck
    {
        private DateTimeOffset _cacheExpiration;
        private volatile int _writerCount;
        
        protected IHealthCheckResult CachedResult { get; private set; }
        
        protected virtual DateTimeOffset UtcNow => DateTimeOffset.UtcNow;

        public async ValueTask<IHealthCheckResult> CheckAsync(HealthCheckContext context)
        {
            while (_cacheExpiration <= UtcNow)
            {
                // Can't use a standard lock here because of async, so we'll use this flag to determine when we should write a value,
                // and the waiters who aren't allowed to write will just spin wait for the new value.
                if (Interlocked.Exchange(ref _writerCount, 1) != 0)
                {
                    await Task.Delay(5, context.CancellationToken).ConfigureAwait(false);
                    continue;
                }

                try
                {
                    CachedResult = await ExecuteCheckAsync(context).ConfigureAwait(false);
                    _cacheExpiration = UtcNow + context.Requirement.CacheDuration;
                    break;
                }
                finally
                {
                    _writerCount = 0;
                }
            }

            return CachedResult;
        }

        /// <summary>
        /// Override to provide the health check implementation. The results will
        /// automatically be cached based on <see cref="CacheDuration"/>, and if
        /// needed, the previously cached value is available via <see cref="CachedResult"/>.
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        protected abstract ValueTask<IHealthCheckResult> ExecuteCheckAsync(HealthCheckContext context);
    }
}
