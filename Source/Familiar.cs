using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TormentedDemonSimulator
{
	public abstract class Familiar
	{
		/// <summary>
		/// Gets the attack interval, in ticks.
		/// </summary>
		public int AttackInterval
		{
			get;
			protected set;
		}

		/// <summary>
		/// Gets the cost of a scroll, in summoning points.
		/// </summary>
		public int ScrollCost
		{
			get;
			protected set;
		}

		/// <summary>
		/// Gets the number of hits performed after using a scroll.
		/// </summary>
		public int ScrollHits
		{
			get;
			protected set;
		}

		/// <summary>
		/// Gets the hit chance, as a percent.
		/// 
		/// Each attack will have this percent chance of landing.
		/// </summary>
		public float HitChance
		{
			get;
			protected set;
		}

		/// <summary>
		/// The chance of the familiar being stupid
		/// and contributing nothing to a kill.
		/// </summary>
		public float StupidValue
		{
			get;
			protected set;
		}

		/// <summary>
		/// The max hit.
		/// </summary>
		public float MaxHit
		{
			get;
			protected set;
		}

		/// <summary>
		/// Gets the style used by the familiar when given a provided value.
		/// 
		/// This value should be weighted between the chance of using any
		/// particular style. However, a value of 0 should be the optimal
		/// style for this familiar (e.g., melee for an iron titan).
		/// </summary>
		public abstract CombatStyle GetStyle(float value, bool scroll);
	}

	public class IronTitanFamiliar : Familiar
	{
		public IronTitanFamiliar()
		{
			AttackInterval = 8;
			ScrollCost = 12;
			ScrollHits = 3;
			MaxHit = 1200;

			// From PURELY SCIENTIFIC ANALYSIS, it has a 50% chance of any
			// attack connecting.
			HitChance = 0.5f;

			// From even more PURE SCIENCE, the Iron Titan is very stupid.
			StupidValue = 0.33f;
		}

		public override CombatStyle GetStyle(float value, bool scroll)
		{
			if (scroll)
			{
				return CombatStyle.Melee;
			}
			else
			{
				if (value < 0.9f)
				{
					return CombatStyle.Melee;
				}
				else
				{
					return CombatStyle.Magic;
				}
			}
		}
	}

	// A steel titan placed strategically as per Stev's advice.
	public class StevTitanFamiliar : Familiar
	{
		public StevTitanFamiliar()
		{
			AttackInterval = 8;
			ScrollCost = 18;
			ScrollHits = 4;
			MaxHit = 1800;

			HitChance = 0.75f;
			StupidValue = 0.5f;
		}

		public override CombatStyle GetStyle(float value, bool scroll)
		{
			return CombatStyle.Melee;
		}
	}

	public class SteelTitanFamiliar : Familiar
	{
		public SteelTitanFamiliar()
		{
			AttackInterval = 8;
			ScrollCost = 18;
			ScrollHits = 4;
			MaxHit = 1800;

			HitChance = 0.75f;
			StupidValue = 0.1f;
		}

		public override CombatStyle GetStyle(float value, bool scroll)
		{
			if (scroll)
			{
				if (value < 0.6f)
				{
					return CombatStyle.Range;
				}
				else
				{
					return CombatStyle.Melee;
				}
			}
			else
			{
				if (value < 0.6f)
				{
					return CombatStyle.Range;
				}
				else if (value < 0.9f)
				{
					return CombatStyle.Melee;
				}
				else
				{
					return CombatStyle.Magic;
				}
			}
		}
	}
}
