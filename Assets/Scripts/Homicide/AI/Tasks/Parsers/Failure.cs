using System.Collections.Generic;

namespace Homicide.AI.Tasks.Parsers
{
	public class Failure
	{
		public string Message { get; internal set; }
		public Parser FailedAt { get; internal set; }

		public Failure(Parser failedAt, string message)
		{
			this.FailedAt = failedAt;
			this.Message = message;
		}

		public static CompoundFailure Compound(Failure a, Failure b)
		{
			var r = new CompoundFailure();
			if (a is CompoundFailure) r.compoundedFailures.AddRange((a as CompoundFailure).compoundedFailures);
			else r.compoundedFailures.Add(a);
			if (b is CompoundFailure) r.compoundedFailures.AddRange((b as CompoundFailure).compoundedFailures);
			else r.compoundedFailures.Add(b);
			return r;
		}
	}

	public class CompoundFailure : Failure
	{
		internal List<Failure> compoundedFailures = new();

		public CompoundFailure() : base(null, "COMPOUND FAILURE") { }

		internal void AddFailure(Failure subFailure)
		{
			compoundedFailures.Add(subFailure);
		}
	}
}
