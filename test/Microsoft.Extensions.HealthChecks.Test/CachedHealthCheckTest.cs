// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Threading;
using System.Threading.Tasks;
using NSubstitute;
using Xunit;

namespace Microsoft.Extensions.HealthChecks
{
    public class CachedHealthCheckTest
    {
        [Fact]
        public async void FirstCallReadsCheck()
        {
            var requirement = Substitute.For<IHealthCheckRequirement>();
            var checkResult = Substitute.For<IHealthCheckResult>();
            var check = Substitute.For<Func<HealthCheckContext, ValueTask<IHealthCheckResult>>>();
            check(new HealthCheckContext(requirement, default(CancellationToken))).ReturnsForAnyArgs(new ValueTask<IHealthCheckResult>(checkResult));
            var healthCheck = new TestableCachedHealthCheck(check);

            var result = await healthCheck.CheckAsync(new HealthCheckContext(requirement, default(CancellationToken)));

            Assert.Same(checkResult, result);
        }

        [Fact]
        public async void SecondCallUsesCachedValue()
        {
            var requirement = Substitute.For<IHealthCheckRequirement>();
            requirement.CacheDuration = TimeSpan.FromSeconds(5);
            var checkResult1 = Substitute.For<IHealthCheckResult>();
            var checkResult2 = Substitute.For<IHealthCheckResult>();
            var check = Substitute.For<Func<HealthCheckContext, ValueTask<IHealthCheckResult>>>();
            check(new HealthCheckContext(requirement, default(CancellationToken))).ReturnsForAnyArgs(new ValueTask<IHealthCheckResult>(checkResult1), new ValueTask<IHealthCheckResult>(checkResult2));
            var healthCheck = new TestableCachedHealthCheck(check);

            var result1 = await healthCheck.CheckAsync(new HealthCheckContext(requirement, default(CancellationToken)));
            var result2 = await healthCheck.CheckAsync(new HealthCheckContext(requirement, default(CancellationToken)));

            Assert.Same(checkResult1, result1);
            Assert.Same(checkResult1, result2);
        }

        [Fact]
        public async void CachedValueRefreshedAfterTimeout()
        {
            var requirement = Substitute.For<IHealthCheckRequirement>();
            requirement.CacheDuration = TimeSpan.Zero;
            var checkResult1 = Substitute.For<IHealthCheckResult>();
            var checkResult2 = Substitute.For<IHealthCheckResult>();
            var check = Substitute.For<Func<HealthCheckContext, ValueTask<IHealthCheckResult>>>();
            check(new HealthCheckContext(requirement, default(CancellationToken))).ReturnsForAnyArgs(new ValueTask<IHealthCheckResult>(checkResult1), new ValueTask<IHealthCheckResult>(checkResult2));
            var healthCheck = new TestableCachedHealthCheck(check);
            var now = DateTimeOffset.UtcNow;

            healthCheck.SetUtcNow(now);
            var result1 = await healthCheck.CheckAsync(new HealthCheckContext(requirement, default(CancellationToken)));
            healthCheck.SetUtcNow(now + TimeSpan.FromSeconds(1));
            var result2 = await healthCheck.CheckAsync(new HealthCheckContext(requirement, default(CancellationToken)));

            Assert.Same(checkResult1, result1);
            Assert.Same(checkResult2, result2);
        }

        [Fact]
        public async void MultipleCallersDuringRefreshPeriodOnlyResultInASingleValue()
        {
            var requirement = Substitute.For<IHealthCheckRequirement>();
            requirement.CacheDuration = TimeSpan.FromSeconds(5);
            var checkResult1 = Substitute.For<IHealthCheckResult>();
            var checkResult2 = Substitute.For<IHealthCheckResult>();
            var check = Substitute.For<Func<HealthCheckContext, ValueTask<IHealthCheckResult>>>();
            var waiter = new TaskCompletionSource<int>();
            var firstTask = new ValueTask<IHealthCheckResult>(((Func<Task<IHealthCheckResult>>)(async () =>
            {
                await waiter.Task;
                return checkResult1;
            }))());
            var secondTask = new ValueTask<IHealthCheckResult>(checkResult2);
            check(new HealthCheckContext(requirement, default(CancellationToken)))
                .ReturnsForAnyArgs(firstTask, secondTask);
            var healthCheck = new TestableCachedHealthCheck(check);

            var task1 = healthCheck.CheckAsync(new HealthCheckContext(requirement, default(CancellationToken)));
            var task2 = healthCheck.CheckAsync(new HealthCheckContext(requirement, default(CancellationToken)));
            waiter.SetResult(0);
            var result1 = await task1;
            var result2 = await task2;

            Assert.Same(checkResult1, result1);
            Assert.Same(checkResult1, result2);
        }

        class TestableCachedHealthCheck : CachedHealthCheck
        {
            private readonly Func<HealthCheckContext, ValueTask<IHealthCheckResult>> _check;
            private DateTimeOffset _utcNow = DateTimeOffset.UtcNow;

            public TestableCachedHealthCheck(Func<HealthCheckContext, ValueTask<IHealthCheckResult>> check)
            {
                _check = check;
            }

            protected override DateTimeOffset UtcNow => _utcNow;

            public void SetUtcNow(DateTimeOffset utcNow)
                => _utcNow = utcNow;

            protected override ValueTask<IHealthCheckResult> ExecuteCheckAsync(HealthCheckContext context)
                => _check(context);
        }
    }
}
