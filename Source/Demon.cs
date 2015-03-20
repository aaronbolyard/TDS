using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TormentedDemonSimulator
{
	public class Demon
	{
		public float Health
		{
			get;
			set;
		}

		public CombatStyle Prayer
		{
			get;
			set;
		}

		public DamageType Weakness
		{
			get;
			set;
		}

		public float CurrentMageDamage
		{
			get;
			set;
		}

		public float CurrentRangeDamage
		{
			get;
			set;
		}

		public float CurrentMeleeDamage
		{
			get;
			set;
		}

		public float DefenseLevel
		{
			get;
			set;
		}

		public Demon()
		{
			DefenseLevel = 85;
			Health = 20000;
			Prayer = CombatStyle.Melee;
		}
	}
}
