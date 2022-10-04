
namespace Homicide.AI.Tasks.Parsers
{
	public enum ResultType
	{
		Success,
		Failure,
		HardError
	}

	public struct ParseResult
	{
		public ResultType resultType;
		public AstNode node;
		public StringIterator after;
		public Failure failReason;
	}
}
