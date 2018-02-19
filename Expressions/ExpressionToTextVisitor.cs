using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using Starship.Core.Extensions;

namespace Starship.Core.Expressions {
    public class ExpressionToTextVisitor : ExpressionVisitor {
        public string GetText(Expression expression) {
            Result = new Stack<dynamic>();
            History = new Stack<Expression>();
            Processed = new List<Expression>();

            Visit(expression);

            var text = Result.Aggregate(string.Empty, (current, segment) => current + segment.value + " ").Trim();

            return text;
        }

        public override Expression Visit(Expression node) {

            History.Push(node);

            if (node is BinaryExpression) {
                var binary = node as BinaryExpression;
                Visit(binary.Right);
                VisitBinary(binary);
                Visit(binary.Left);
            }
            else if (node is MemberExpression) {
                var member = node as MemberExpression;

                if (!Processed.Contains(member)) {
                    if (!(member.Expression is ParameterExpression)) {
                        Visit(member.Expression);
                        Concatenate("of");
                    }

                    VisitMember(member);
                }
            }
            else {
                node = base.Visit(node);
            }

            History.Pop();

            return node;
        }

        protected override Expression VisitParameter(ParameterExpression node) {
            //Concatenate(node.Name, node);
            return base.VisitParameter(node);
        }

        protected override Expression VisitTypeBinary(TypeBinaryExpression node)
        {
            return base.VisitTypeBinary(node);
        }

        protected override Expression VisitConditional(ConditionalExpression node)
        {
            return base.VisitConditional(node);
        }

        protected override Expression VisitBinary(BinaryExpression node) {

            var prefix = "must be ";

            switch (node.NodeType)
            {
                case ExpressionType.GreaterThan:
                    Concatenate(prefix + "greater than", node);
                    break;
                case ExpressionType.GreaterThanOrEqual:
                    Concatenate(prefix + "greater than on equal to", node);
                    break;
                case ExpressionType.Equal:
                    Concatenate(prefix + "equal to", node);
                    break;
                case ExpressionType.LessThanOrEqual:
                    Concatenate(prefix + "less than on equal to", node);
                    break;
                case ExpressionType.LessThan:
                    Concatenate(prefix + "less than", node);
                    break;
                case ExpressionType.NotEqual:
                    Concatenate(prefix + "not equal to", node);
                    break;
            }

            return base.VisitBinary(node);
        }

        protected override Expression VisitMember(MemberExpression node) {
            Concatenate(node.Member.Name.SplitCamelCase().ToLower(), node);

            return base.VisitMember(node);
        }

        protected override Expression VisitConstant(ConstantExpression node) {
            Concatenate(node.Value.ToString(), node);
            return base.VisitConstant(node);
        }

        private void Concatenate(string value) {
            Result.Push(new { value });
        }

        private void Concatenate(string value, Expression expression) {
            if (Processed.Contains(expression)) {
                return;
            }

            Result.Push(new { value, expression, expression.NodeType });
            Processed.Add(expression);
        }

        private Stack<dynamic> Result { get; set; }

        private Stack<Expression> History { get; set; }

        private List<Expression> Processed { get; set; }
    }
}