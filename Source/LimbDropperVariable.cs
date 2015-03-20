using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TormentedDemonSimulator
{
	public abstract class LimbDropperVariable
	{
		public abstract bool GetLimb();
	}

	public class RandomLimbDropperVariable : LimbDropperVariable
	{
		Random random;

		public RandomLimbDropperVariable(Random random)
		{
			this.random = random;
		}

		public override bool GetLimb()
		{
			return random.Next(250) == 0;
		}
	}
}
