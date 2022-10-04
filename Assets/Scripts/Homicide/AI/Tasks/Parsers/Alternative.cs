using System.Collections.Generic;

namespace Homicide.AI.Tasks.Parsers
{
	public class Alternative : Parser
	{
        public List<Parser> subParsers;

        public Alternative(params Parser[] subParsers)
        {
            this.subParsers = new(subParsers);
        }

        public static Alternative operator |(Alternative lhs, Parser rhs)
        {
            var r = lhs.Clone() as Alternative;
            r.subParsers.Add(rhs);
            return r;
        }

        protected override Parser ImplementClone()
        {
            return new Alternative(subParsers.ToArray());
        }

        protected override ParseResult ImplementParse(StringIterator inputStream)
        {
            CompoundFailure failureReason = null;

            foreach (var sub in subParsers)
            {
                var subResult = sub.Parse(inputStream);
                if (subResult.resultType == ResultType.HardError)
                    return Error("Hard failure in child parser", subResult.failReason);
                else if (subResult.resultType == ResultType.Success)
                    return WrapChild(subResult);
                else if (subResult.failReason != null)
                {
                    if (failureReason == null) failureReason = new();
                    failureReason.compoundedFailures.Add(subResult.failReason);
                }
            }

            return Fail("No alternatives matched", failureReason);
        }
    }
}
