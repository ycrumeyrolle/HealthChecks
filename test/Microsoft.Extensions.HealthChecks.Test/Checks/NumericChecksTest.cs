// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Linq;
using System.Threading;
using Microsoft.Extensions.DependencyInjection;
using NSubstitute;
using Xunit;

namespace Microsoft.Extensions.HealthChecks.Checks
{
    public class NumericChecksTest
    {
        HealthCheckBuilder builder = new HealthCheckBuilder(new ServiceCollection());

        public class AddMinValueCheck : NumericChecksTest
        {
            [Fact]
            public void GuardClauses()
            {
                Assert.Throws<ArgumentNullException>("builder", () => HealthCheckBuilderExtensions.AddMinValueCheck(null, "name", 42, () => 2112));
                Assert.Throws<ArgumentNullException>("name", () => HealthCheckBuilderExtensions.AddMinValueCheck(builder, null, 42, () => 2112));
                Assert.Throws<ArgumentException>("name", () => HealthCheckBuilderExtensions.AddMinValueCheck(builder, " ", 42, () => 2112));
                Assert.Throws<ArgumentNullException>("currentValueFunc", () => HealthCheckBuilderExtensions.AddMinValueCheck(builder, "name", 42, null));
            }

            [Theory]
            [InlineData(-1, CheckStatus.Unhealthy)]
            [InlineData(1, CheckStatus.Healthy)]
            public async void RegistersCheck(int monitoredValue, CheckStatus expectedStatus)
            {
                var requirement = Substitute.For<IHealthCheckRequirement>();
                
                builder.AddMinValueCheck("CheckName", 0, () => monitoredValue);
                
                var descriptor = builder.Build().FirstOrDefault(c => c.Requirement.Name == "CheckName");
                var check = (IHealthCheck)builder.Services.First(s => s.ServiceType == descriptor.Type).ImplementationInstance;

                var result = await check.CheckAsync(new HealthCheckContext(requirement, CancellationToken.None));
                Assert.Equal(expectedStatus, result.CheckStatus);
                Assert.Equal($"CheckName: min=0, current={monitoredValue}", result.Description);
                Assert.Collection(result.Data.OrderBy(kvp => kvp.Key),
                    kvp =>
                    {
                        Assert.Equal("current", kvp.Key);
                        Assert.Equal(monitoredValue, kvp.Value);
                    },
                    kvp =>
                    {
                        Assert.Equal("min", kvp.Key);
                        Assert.Equal(0, kvp.Value);
                    }
                );
            }
        }

        public class AddMaxValueCheck : NumericChecksTest
        {
            [Fact]
            public void GuardClauses()
            {
                Assert.Throws<ArgumentNullException>("builder", () => HealthCheckBuilderExtensions.AddMaxValueCheck(null, "name", 42, () => 2112));
                Assert.Throws<ArgumentNullException>("name", () => HealthCheckBuilderExtensions.AddMaxValueCheck(builder, null, 42, () => 2112));
                Assert.Throws<ArgumentException>("name", () => HealthCheckBuilderExtensions.AddMaxValueCheck(builder, " ", 42, () => 2112));
                Assert.Throws<ArgumentNullException>("currentValueFunc", () => HealthCheckBuilderExtensions.AddMaxValueCheck(builder, "name", 42, null));
            }

            [Theory]
            [InlineData(1, CheckStatus.Unhealthy)]
            [InlineData(-1, CheckStatus.Healthy)]
            public async void RegistersCheck(int monitoredValue, CheckStatus expectedStatus)
            {
                var requirement = Substitute.For<IHealthCheckRequirement>();
                builder.AddMaxValueCheck("CheckName", 0, () => monitoredValue);

                var descriptor = builder.Build().FirstOrDefault(c => c.Requirement.Name == "CheckName");
                var check = (IHealthCheck)builder.Services.First(s => s.ServiceType == descriptor.Type).ImplementationInstance;

                var result = await check.CheckAsync(new HealthCheckContext(requirement, CancellationToken.None));
                Assert.Equal(expectedStatus, result.CheckStatus);
                Assert.Equal($"CheckName: max=0, current={monitoredValue}", result.Description);
                Assert.Collection(result.Data.OrderBy(kvp => kvp.Key),
                    kvp =>
                    {
                        Assert.Equal("current", kvp.Key);
                        Assert.Equal(monitoredValue, kvp.Value);
                    },
                    kvp =>
                    {
                        Assert.Equal("max", kvp.Key);
                        Assert.Equal(0, kvp.Value);
                    }
                );
            }
        }
    }
}
