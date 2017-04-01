// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Threading;

namespace Microsoft.Extensions.HealthChecks
{
    public class HealthCheckContext
    {
        public HealthCheckContext(IHealthCheckRequirement requirement, CancellationToken cancellationToken)
        {
            Requirement = requirement;
            CancellationToken = cancellationToken;
        }

        public IHealthCheckRequirement Requirement { get; }

        public CancellationToken CancellationToken { get; }
    }
}