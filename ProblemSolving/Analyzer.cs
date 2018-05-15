using System.Collections.Generic;

namespace Starship.Core.ProblemSolving {
    public abstract class Analyzer<T> {

        public List<Constraint<T>> Analyze(List<Observation> observations) {
            var conclusions = new List<Constraint<T>>();
            Analyze(observations, conclusions);
            return conclusions;
        }

        protected abstract void Analyze(List<Observation> observations, List<Constraint<T>> conclusions);
    }
}