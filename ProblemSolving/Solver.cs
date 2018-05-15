using System;
using System.Collections.Generic;

namespace Starship.Core.ProblemSolving {
    public class Solver<PROBLEM, SOLUTION> {

        public Solver() {
            Observations = new List<Observation>();
            Constraints = new List<Constraint<SOLUTION>>();
            Analyzers = new List<Analyzer<SOLUTION>>();
            Observers = new List<PatternObserver<SOLUTION>>();
        }

        public void DefineProperty<T>() {
        }

        public void Add(PatternObserver<SOLUTION> observer) {
            Observers.Add(observer);
        }

        public void Add(PROBLEM input, SOLUTION output) {
            foreach (var observer in Observers) {
                var observations = new List<Observation>();
                //observer.GetObservations(fact, observations);

                Observations.AddRange(observations);
            }

            var constraints = new List<Constraint<SOLUTION>>();

            // Use observations to form conclusions
            foreach (var observation in Observations) {
                
            }

            Constraints = constraints;
        }

        public void Map(Func<PROBLEM, object> expression) {
        }

        public SOLUTION Solve(PROBLEM problem) {
            
            foreach (var constraint in Constraints) {
                
            }

            return default(SOLUTION);
        }

        private List<PatternObserver<SOLUTION>> Observers { get; set; }

        private List<Analyzer<SOLUTION>> Analyzers { get; set; }

        private List<Observation> Observations { get; set; }

        private List<Constraint<SOLUTION>> Constraints { get; set; }
    }
}