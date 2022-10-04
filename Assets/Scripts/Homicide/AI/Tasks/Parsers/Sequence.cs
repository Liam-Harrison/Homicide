using System.Collections.Generic;

namespace Homicide.AI.Tasks.Parsers
{
	public class Sequence : Parser
	{
        private List<Parser> subParsers;

        public Sequence(params Parser[] subParsers)
        {
            this.subParsers = new(subParsers);
        }

        public static Sequence operator +(Sequence lhs, Parser rhs)
        {
            var r = lhs.Clone() as Sequence;
            r.subParsers.Add(rhs);
            return r;
        }

        public static Sequence operator +(Sequence lhs, char rhs)
        {
            var r = lhs.Clone() as Sequence;
            r.subParsers.Add(new Character(rhs));
            return r;
        }

        public static Sequence operator +(Sequence lhs, string rhs)
        {
            var r = lhs.Clone() as Sequence;
            r.subParsers.Add(new KeyWord(rhs));
            return r;
        }

        protected override Parser ImplementClone()
        {
            return new Sequence(subParsers.ToArray());
        }

        protected override ParseResult ImplementParse(StringIterator inputStream)
        {
            var r = new AstNode { nodeType = astNodeType, location = inputStream };

            foreach (var sub in subParsers)
            {
                var subResult = sub.Parse(inputStream);

                if (subResult.resultType == ResultType.HardError)
                    return Error("Child produced hard error", subResult.failReason);
                else if (subResult.resultType == ResultType.Failure)
                    return Fail("Child failed", subResult.failReason);

                inputStream = subResult.after;

                if (subResult.node != null)
                    r.children.Add(subResult.node);
            }

            return new()
            {
                resultType = ResultType.Success,
                after = inputStream,
                node = r
            };
        }
    }
}
