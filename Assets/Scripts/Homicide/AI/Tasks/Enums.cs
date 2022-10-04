using System;

namespace Homicide.AI.Tasks
{
	public enum TaskPriority
	{
		NotSet = -1,
		Eventually = 0,
		Low = 1,
		Medium = 2,
		High = 3,
		Urgent = 4
	}

	[Flags]
	public enum TaskCategory : uint
	{
		None		= 0,
		Other		= 1 << 0,
		Dig			= 1 << 1,
		Chop		= 1 << 2,
		Harvest		= 1 << 3,
		Attack		= 1 << 4,
		Hunt		= 1 << 5,
		Research	= 1 << 6,
		BuildBlock	= 1 << 7,
		BuildObject = 1 << 8,
		BuildZone	= 1 << 9,
		CraftItem	= 1 << 10,
		Cook		= 1 << 11,
		TillSoil	= 1 << 12,
		Gather		= 1 << 13,
		Guard		= 1 << 14,
		Wrangle		= 1 << 15,
		Plant		= 1 << 16,
		Brew		= 1 << 17
	}

	public enum Feasibility
	{
		Feasible,
		Infeasible,
		Unknown
	}
}
