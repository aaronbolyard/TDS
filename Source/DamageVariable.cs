using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TormentedDemonSimulator
{
	public abstract class DamageVariable
	{
		public abstract float GetDamage(float min, float max);
	}

	public class RandomDamageVariable : DamageVariable
	{
		Random random;

		public RandomDamageVariable(Random random)
		{
			this.random = random;
		}

		public override float GetDamage(float min, float max)
		{
			return (float)random.NextDouble() * (max - min) + min;
		}
	}
}
