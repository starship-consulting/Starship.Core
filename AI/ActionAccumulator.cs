using System;
using System.Collections.Generic;
using Starship.Core.AI.Planning;

namespace Starship.Core.AI {
    public abstract class ActionAccumulator {
        public abstract List<PlanAction> GetPossibleActions(StateContainer state);
    }
}