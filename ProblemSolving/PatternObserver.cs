using System;
using System.Collections.Generic;

namespace Starship.Core.ProblemSolving {
    public abstract class PatternObserver<T> {

        public abstract void GetObservations(T fact, List<Observation> observations);
    }
}