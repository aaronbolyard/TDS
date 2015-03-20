using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TormentedDemonSimulator
{
	public abstract class EffectVariable
	{
		public abstract bool GetOccurred(float chance);
	}

	public class RandomEffectVariable : EffectVariable
	{
		Random random;

		public RandomEffectVariable(Random random)
		{
			this.random = random;
		}

		public override bool GetOccurred(float chance)
		{
			return random.NextDouble() <= chance;
		}
	}
}
