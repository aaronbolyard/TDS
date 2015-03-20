using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TormentedDemonSimulator
{
	public abstract class FamiliarVariable
	{
		public abstract float GetStyleValue();

		public abstract bool GetHit(float hitChance);

		public abstract bool GetStupid(float stupid);
	}

	public class RandomFamiliarVariable : FamiliarVariable
	{
		Random random;

		public RandomFamiliarVariable(Random random)
		{
			this.random = random;
		}

		public override float GetStyleValue()
		{
			return (float)random.NextDouble();
		}

		public override bool GetHit(float hitChance)
		{
			return (float)random.NextDouble() <= hitChance;
		}

		public override bool GetStupid(float stupid)
		{
			return (float)random.NextDouble() <= stupid;
		}
	}
}
