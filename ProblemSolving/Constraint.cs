using System;

namespace Starship.Core.ProblemSolving {
    public abstract class Constraint<T> {
        public abstract bool SatisfiesConstraint(T input);
    }
}