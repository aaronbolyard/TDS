using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TormentedDemonSimulator
{
	public abstract class IntervalVariable
	{
		public abstract int GetInterval();
	}

	public class RandomIntervalVariable : IntervalVariable
	{
		Random random;
		int max, min;

		public RandomIntervalVariable(Random random, int min, int max)
		{
			this.random = random;
			this.min = min;
			this.max = max;
		}

		public override int GetInterval()
		{
			return random.Next(min, max);
		}
	}
}
