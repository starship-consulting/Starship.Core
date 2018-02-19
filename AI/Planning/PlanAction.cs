using System;

namespace Starship.Core.AI.Planning {
  public abstract class PlanAction {

      public abstract void Simulate(StateContainer state);

      public abstract void Execute();
  }
}