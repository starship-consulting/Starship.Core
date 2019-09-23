using System;
using System.Collections.Generic;
using Starship.Core.Interfaces;

namespace Starship.Core.Security {
    public interface IsSecurityContext : HasIdentity<string> {

        bool HasGroup(string id);

        void RemoveGroup(string id);

        void AddGroup(string id);

        List<string> Groups { get; set; }
    }
}