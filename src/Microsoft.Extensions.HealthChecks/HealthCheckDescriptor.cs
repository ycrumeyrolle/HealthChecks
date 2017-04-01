// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;

namespace Microsoft.Extensions.HealthChecks
{
    public class HealthCheckDescriptor
    {
        public HealthCheckDescriptor(Type type, IHealthCheckRequirement requirement)
        {
            Type = type;
            Requirement = requirement;
        }

        public Type Type { get; }

        public IHealthCheckRequirement Requirement { get;}
    }
}
