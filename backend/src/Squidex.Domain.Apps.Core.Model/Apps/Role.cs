﻿// ==========================================================================
//  Squidex Headless CMS
// ==========================================================================
//  Copyright (c) Squidex UG (haftungsbeschränkt)
//  All rights reserved. Licensed under the MIT license.
// ==========================================================================

using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Linq;
using Squidex.Infrastructure;
using Squidex.Infrastructure.Security;
using P = Squidex.Shared.Permissions;

namespace Squidex.Domain.Apps.Core.Apps
{
    [Equals(DoNotAddEqualityOperators = true)]
    public sealed class Role : Named
    {
        public const string Editor = "Editor";
        public const string Developer = "Developer";
        public const string Owner = "Owner";
        public const string Reader = "Reader";

        public PermissionSet Permissions { get; }

        [IgnoreDuringEquals]
        public bool IsDefault
        {
            get { return Roles.IsDefault(this); }
        }

        public Role(string name, PermissionSet permissions)
            : base(name)
        {
            Guard.NotNull(permissions, nameof(permissions));

            Permissions = permissions;
        }

        public Role(string name, params string[] permissions)
            : this(name, new PermissionSet(permissions))
        {
        }

        [Pure]
        public Role Update(string[] permissions)
        {
            return new Role(Name, new PermissionSet(permissions));
        }

        public bool Equals(string name)
        {
            return name != null && name.Equals(Name, StringComparison.Ordinal);
        }

        public Role ForApp(string app)
        {
            var result = new HashSet<Permission>
            {
                P.ForApp(P.AppCommon, app)
            };

            if (Permissions.Any())
            {
                var prefix = P.ForApp(P.App, app).Id;

                foreach (var permission in Permissions)
                {
                    result.Add(new Permission(string.Concat(prefix, ".", permission.Id)));
                }
            }

            return new Role(Name, new PermissionSet(result));
        }
    }
}
