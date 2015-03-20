using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TormentedDemonSimulator
{
	public struct AbilityDamage
	{
		/// <summary>
		/// The minimum damage dealt by this ability.
		/// </summary>
		public float Minimum;

		/// <summary>
		/// The maximum damage dealt by this ability.
		/// </summary>
		public float Maximum;

		/// <summary>
		/// The average damage dealt by this ability.
		/// </summary>
		public float Average
		{
			get { return (Maximum + Minimum) / 2.0f; }
		}
	}
}
