using System;

namespace Starship.Core.ProblemSolving.Constraints {
    public class BooleanConstraint<T> : Constraint<T> {

        public BooleanConstraint(Constraint<T> operand) {
            Operand = operand;
        }

        public override bool SatisfiesConstraint(T input) {
            var result = Operand.SatisfiesConstraint(input);

            if (result) {
                if (IfTrue != null) {
                    return IfTrue.SatisfiesConstraint(input);
                }
            }
            else {
                if (IfFalse != null) {
                    return IfFalse.SatisfiesConstraint(input);
                }
            }

            return result;
        }


        public Constraint<T> Operand { get; set; }

        public Constraint<T> IfTrue { get; set; }

        public Constraint<T> IfFalse { get; set; }
    }
}