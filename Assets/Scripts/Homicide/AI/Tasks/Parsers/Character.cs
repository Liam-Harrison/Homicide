
namespace Homicide.AI.Tasks.Parsers
{
	public class Character : Parser
	{
		private char @char;

		public Character(char @char)
		{
			this.@char = @char;
		}

		protected override Parser ImplementClone()
		{
			return new Character(@char);
		}

		protected override ParseResult ImplementParse(StringIterator inputStream)
		{
			if (inputStream.AtEnd || inputStream.Next != @char)
				return Fail("Expected " + @char);

			return new()
			{
				resultType = ResultType.Success,
				node = new() { nodeType = astNodeType, value = @char, location = inputStream },
				after = inputStream.Advance()
			};
		}
	}
}
