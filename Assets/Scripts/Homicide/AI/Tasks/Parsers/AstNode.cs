using System.Collections.Generic;

namespace Homicide.AI.Tasks.Parsers
{
	public class AstNode
	{
		public string nodeType = "unknown";
		public object value = null;
		public List<AstNode> children = new();
		public StringIterator location;

		public static AstNode WrapChild(string type, AstNode child)
		{
			var r = new AstNode();
			r.nodeType = type;
			r.children.Add(child);
			r.location = child.location;
			return r;
		}
	}
}
