
namespace Homicide.AI.Tasks.Parsers
{
	public class KeyWord : Parser
	{
        private string word;

        public KeyWord(string word)
        {
            this.word = word;
        }

        protected override Parser ImplementClone()
        {
            return new KeyWord(word);
        }

        protected override ParseResult ImplementParse(StringIterator inputStream)
        {
            var text = inputStream.Peek(word.Length);

            if (text == word)
                return new()
                {
                    resultType = ResultType.Success,
                    node = new() { nodeType = astNodeType, value = word, location = inputStream },
                    after = inputStream.Advance(word.Length)
                };
            else
                return Fail("KeyWord not found");
        }
    }
}
