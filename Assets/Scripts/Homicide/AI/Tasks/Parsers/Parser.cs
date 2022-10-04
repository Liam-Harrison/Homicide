using System;

namespace Homicide.AI.Tasks.Parsers
{
	public abstract class Parser
	{
		public string astNodeType = "UNNAMED";
		Func<AstNode, AstNode> astMutator = (a) => a;

		private ParseResult Fail(string message, ResultType result)
		{
			var reason = new Failure(this, message);

			return new()
            {
				resultType = result,
				failReason = reason
			};
		}
        private ParseResult Fail(string message, ResultType resultType, Failure subFailure)
        {
            var failReason = new CompoundFailure();
            failReason.FailedAt = this;
            failReason.Message = message;
            failReason.AddFailure(subFailure);

            return new()
            {
                resultType = resultType,
                failReason = failReason
            };
        }

        public static string CollapseEscapeSequences(string value)
        {
            var r = "";
            var itr = new StringIterator(value);

            bool escaping = false;
            while (!itr.AtEnd)
            {
                if (escaping)
                {
                    if (itr.Next == 'n')
                        r += '\n';
                    else if (itr.Next == 'r')
                        r += '\r';
                    else if (itr.Next == 't')
                        r += '\t';
                    else
                        r += itr.Next;
                    itr = itr.Advance();
                    escaping = false;
                }
                else if (itr.Next == '\\')
                {
                    escaping = true;
                    itr = itr.Advance();
                }
                else
                {
                    r += itr.Next;
                    itr = itr.Advance();
                }
            }

            return r;
        }

        protected ParseResult Fail(string message) { return Fail(message, ResultType.Failure); }
        protected ParseResult Fail(string message, Failure subFailure) { return Fail(message, ResultType.Failure, subFailure); }
        protected ParseResult Error(string message) { return Fail(message, ResultType.HardError); }
        protected ParseResult Error(string message, Failure subFailure) { return Fail(message, ResultType.HardError, subFailure); }

        protected ParseResult WrapChild(ParseResult childResult)
        {
            return new()
            {
                resultType = ResultType.Success,
                after = childResult.after,
                node = AstNode.WrapChild(astNodeType, childResult.node)
            };
        }

        public Parser Ast(string nodeType)
        {
            var r = this.Clone();
            r.astNodeType = nodeType;
            return r;
        }

        public Parser WithMutator(Func<AstNode, AstNode> mutator)
        {
            var r = this.Clone();
            r.astMutator = (a) => mutator(astMutator(a));
            return r;
        }

        public Parser WithoutMutators()
        {
            var r = this.Clone();
            r.astMutator = (a) => a;
            return r;
        }

        protected Parser Clone()
        {
            var r = this.ImplementClone();
            r.astNodeType = astNodeType;
            r.astMutator = astMutator;
            return r;
        }

        public ParseResult Parse(StringIterator inputStream)
        {
            var r = ImplementParse(inputStream);
            if (r.resultType == ResultType.Success)
                r.node = astMutator(r.node);
            return r;
        }

        protected abstract ParseResult ImplementParse(StringIterator inputStream);
        protected abstract Parser ImplementClone();

        public static Sequence operator +(Parser lhs, Parser rhs)
        {
            return new(lhs, rhs);
        }

        public static Sequence operator +(Parser lhs, char rhs)
        {
            return new(lhs, new Character(rhs));
        }

        public static Sequence operator +(char lhs, Parser rhs)
        {
            return new(new Character(lhs), rhs);
        }

        public static Sequence operator +(Parser lhs, string rhs)
        {
            return new(lhs, new KeyWord(rhs));
        }

        public static Alternative operator |(Parser lhs, Parser rhs)
        {
            return new(lhs, rhs);
        }

        public static Alternative operator |(Parser lhs, char rhs)
        {
            return new(lhs, new Character(rhs));
        }
    }
}
