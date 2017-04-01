//// Copyright (c) .NET Foundation. All rights reserved.
//// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

//using System;
//using System.Threading;
//using System.Threading.Tasks;

//namespace Microsoft.Extensions.HealthChecks
//{
//    public static partial class HealthCheckBuilderExtensions
//    {
//        // Lambda versions of AddCheck for Func/Func<Task>/Func<ValueTask>

//        public static HealthCheckBuilder AddCheck(this HealthCheckBuilder builder, string name, Func<IHealthCheckRequirement, IHealthCheckResult> check)
//        {
//            Guard.ArgumentNotNull(nameof(builder), builder);

//            builder.AddCheck(InlineHealthCheck.FromCheck(check, builder.DefaultCacheDuration), new NamedRequirement(name));
//            return builder;
//        }

//        public static HealthCheckBuilder AddCheck(this HealthCheckBuilder builder, string name, Func<IHealthCheckRequirement, CancellationToken, IHealthCheckResult> check)
//        {
//            Guard.ArgumentNotNull(nameof(builder), builder);

//            builder.AddCheck(InlineHealthCheck.FromCheck(check, builder.DefaultCacheDuration), new NamedRequirement(name));
//            return builder;
//        }

//        public static HealthCheckBuilder AddCheck(this HealthCheckBuilder builder, string name, Func<IHealthCheckRequirement, IHealthCheckResult> check, TimeSpan cacheDuration)
//        {
//            Guard.ArgumentNotNull(nameof(builder), builder);

//            builder.AddCheck(InlineHealthCheck.FromCheck(check, cacheDuration), new NamedRequirement(name));
//            return builder;
//        }

//        public static HealthCheckBuilder AddCheck(this HealthCheckBuilder builder, string name, Func<IHealthCheckRequirement, CancellationToken, IHealthCheckResult> check, TimeSpan cacheDuration)
//        {
//            Guard.ArgumentNotNull(nameof(builder), builder);

//            builder.AddCheck(InlineHealthCheck.FromCheck(check, cacheDuration), new NamedRequirement(name));
//            return builder;
//        }

//        public static HealthCheckBuilder AddCheck(this HealthCheckBuilder builder, string name, Func<IHealthCheckRequirement, Task<IHealthCheckResult>> check)
//        {
//            Guard.ArgumentNotNull(nameof(builder), builder);

//            builder.AddCheck(InlineHealthCheck.FromTaskCheck(check, builder.DefaultCacheDuration), new NamedRequirement(name));
//            return builder;
//        }

//        public static HealthCheckBuilder AddCheck(this HealthCheckBuilder builder, string name, Func<IHealthCheckRequirement, CancellationToken, Task<IHealthCheckResult>> check)
//        {
//            Guard.ArgumentNotNull(nameof(builder), builder);

//            builder.AddCheck(InlineHealthCheck.FromTaskCheck(check, builder.DefaultCacheDuration), new NamedRequirement(name));
//            return builder;
//        }

//        public static HealthCheckBuilder AddCheck(this HealthCheckBuilder builder, string name, Func<IHealthCheckRequirement, Task<IHealthCheckResult>> check, TimeSpan cacheDuration)
//        {
//            Guard.ArgumentNotNull(nameof(builder), builder);

//            builder.AddCheck(InlineHealthCheck.FromTaskCheck(check, cacheDuration), new NamedRequirement(name));
//            return builder;
//        }

//        public static HealthCheckBuilder AddCheck(this HealthCheckBuilder builder, string name, Func<IHealthCheckRequirement, CancellationToken, Task<IHealthCheckResult>> check, TimeSpan cacheDuration)
//        {
//            Guard.ArgumentNotNull(nameof(builder), builder);

//            builder.AddCheck(InlineHealthCheck.FromTaskCheck(check, cacheDuration), new NamedRequirement(name));
//            return builder;
//        }

//        public static HealthCheckBuilder AddValueTaskCheck(this HealthCheckBuilder builder, string name, Func<IHealthCheckRequirement, ValueTask<IHealthCheckResult>> check)
//        {
//            Guard.ArgumentNotNull(nameof(builder), builder);

//            builder.AddCheck(InlineHealthCheck.FromValueTaskCheck(check, builder.DefaultCacheDuration), new NamedRequirement(name));
//            return builder;
//        }

//        public static HealthCheckBuilder AddValueTaskCheck(this HealthCheckBuilder builder, string name, Func<IHealthCheckRequirement, CancellationToken, ValueTask<IHealthCheckResult>> check)
//        {
//            Guard.ArgumentNotNull(nameof(builder), builder);

//            builder.AddCheck(InlineHealthCheck.FromValueTaskCheck(check, builder.DefaultCacheDuration), new NamedRequirement(name));
//            return builder;
//        }

//        public static HealthCheckBuilder AddValueTaskCheck(this HealthCheckBuilder builder, string name, Func<IHealthCheckRequirement, ValueTask<IHealthCheckResult>> check, TimeSpan cacheDuration)
//        {
//            Guard.ArgumentNotNull(nameof(builder), builder);

//            builder.AddCheck(InlineHealthCheck.FromValueTaskCheck(check, cacheDuration), new NamedRequirement(name));
//            return builder;
//        }

//        public static HealthCheckBuilder AddValueTaskCheck(this HealthCheckBuilder builder, string name, Func<IHealthCheckRequirement, CancellationToken, ValueTask<IHealthCheckResult>> check, TimeSpan cacheDuration)
//        {
//            Guard.ArgumentNotNull(nameof(builder), builder);

//            builder.AddCheck(InlineHealthCheck.FromValueTaskCheck(check, cacheDuration), new NamedRequirement(name));
//            return builder;
//        }
//    }
//}
