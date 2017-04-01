﻿// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Threading;
using System.Threading.Tasks;
using NSubstitute;
using Xunit;

namespace Microsoft.Extensions.HealthChecks
{
    public class HealthCheckTest
    {
        [Fact]
        public void GuardClauses()
        {
            Func<CancellationToken, ValueTask<IHealthCheckResult>> check = _ => new ValueTask<IHealthCheckResult>(default(IHealthCheckResult));

            Assert.Throws<ArgumentNullException>("check", () => new TestableHealthCheck(null));
        }

        [Fact]
        public async void FirstCallReadsCheck()
        {
            var requirement = Substitute.For<IHealthCheckRequirement>();
            var checkResult = Substitute.For<IHealthCheckResult>();
            var check = Substitute.For<Func<HealthCheckContext, ValueTask<IHealthCheckResult>>>();
            var context = new HealthCheckContext(requirement, CancellationToken.None);
            check(context).ReturnsForAnyArgs(new ValueTask<IHealthCheckResult>(checkResult));
            var healthCheck = new TestableHealthCheck(check);

            var result = await healthCheck.CheckAsync(context);

            Assert.Same(checkResult, result);
        }

        [Fact]
        public async void SecondCallUsesCachedValue()
        {
            var requirement = Substitute.For<IHealthCheckRequirement>();
            requirement.CacheDuration = TimeSpan.FromSeconds(1);
            var checkResult1 = Substitute.For<IHealthCheckResult>();
            var checkResult2 = Substitute.For<IHealthCheckResult>();
            var check = Substitute.For<Func<HealthCheckContext, ValueTask<IHealthCheckResult>>>();
            var context = new HealthCheckContext(requirement, CancellationToken.None);
            check(context).ReturnsForAnyArgs(new ValueTask<IHealthCheckResult>(checkResult1), new ValueTask<IHealthCheckResult>(checkResult2));
            var healthCheck = new TestableHealthCheck(check);

            var result1 = await healthCheck.CheckAsync(context);
            var result2 = await healthCheck.CheckAsync(context);

            Assert.Same(checkResult1, result1);
            Assert.Same(checkResult1, result2);
        }

        [Fact]
        public async void CachedValueRefreshedAfterTimeout()
        {
            var requirement = Substitute.For<IHealthCheckRequirement>();
            requirement.CacheDuration = TimeSpan.FromSeconds(1);
            var checkResult1 = Substitute.For<IHealthCheckResult>();
            var checkResult2 = Substitute.For<IHealthCheckResult>();
            var check = Substitute.For<Func<HealthCheckContext, ValueTask<IHealthCheckResult>>>();
            var context = new HealthCheckContext(requirement, CancellationToken.None);
            check(context).ReturnsForAnyArgs(new ValueTask<IHealthCheckResult>(checkResult1), new ValueTask<IHealthCheckResult>(checkResult2));
            var healthCheck = new TestableHealthCheck(check);
            var now = DateTimeOffset.UtcNow;

            healthCheck.SetUtcNow(now);
            var result1 = await healthCheck.CheckAsync(context);
            healthCheck.SetUtcNow(now + TimeSpan.FromSeconds(1));
            var result2 = await healthCheck.CheckAsync(context);

            Assert.Same(checkResult1, result1);
            Assert.Same(checkResult2, result2);
        }

        [Fact]
        public async void MultipleCallersDuringRefreshPeriodOnlyResultInASingleValue()
        {
            var requirement = Substitute.For<IHealthCheckRequirement>();
            requirement.CacheDuration = TimeSpan.FromSeconds(1);
            var checkResult1 = Substitute.For<IHealthCheckResult>();
            var checkResult2 = Substitute.For<IHealthCheckResult>();
            var check = Substitute.For<Func<HealthCheckContext, ValueTask<IHealthCheckResult>>>();
            var waiter = new TaskCompletionSource<int>();
            var firstTask = new ValueTask<IHealthCheckResult>(((Func<Task<IHealthCheckResult>>)(async () =>
            {
                await waiter.Task;
                return checkResult1;
            }))());
            var context = new HealthCheckContext(requirement, CancellationToken.None);
            var secondTask = new ValueTask<IHealthCheckResult>(checkResult2);
            check(context).ReturnsForAnyArgs(firstTask, secondTask);
            var healthCheck = new TestableHealthCheck(check);

            var task1 = healthCheck.CheckAsync(context);
            var task2 = healthCheck.CheckAsync(context);
            waiter.SetResult(0);
            var result1 = await task1;
            var result2 = await task2;

            Assert.Same(checkResult1, result1);
            Assert.Same(checkResult1, result2);
        }

        class TestableHealthCheck : InlineHealthCheck
        {
            private DateTimeOffset _utcNow = DateTimeOffset.UtcNow;

            public TestableHealthCheck(Func<HealthCheckContext, ValueTask<IHealthCheckResult>> check)
                : base(check) { }

            protected override DateTimeOffset UtcNow => _utcNow;

            public void SetUtcNow(DateTimeOffset utcNow)
                => _utcNow = utcNow;
        }
    }
}
