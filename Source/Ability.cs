using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TormentedDemonSimulator
{
	public class Ability
	{
		/// <summary>
		/// The maximum damage as a percent of weapon damage.
		/// 
		/// If the ability damage is 157%, then this value should be 157.
		/// </summary>
		public float MaxDamage
		{
			get;
			private set;
		}

		/// <summary>
		/// The minimum damage as a percent of weapon damage.
		/// </summary>
		public float MinDamage
		{
			get;
			private set;
		}

		/// <summary>
		/// The duration of the ability, in ticks.
		/// 
		/// This is the extra time elapsed before the next ability can be fired.
		/// 
		/// For most abilities, this will be 3. However, some abilities have an
		/// extra duration; for example, Snipe effictively has a 6 tick duration 
		/// before the next ability can used.
		/// </summary>
		public int Duration
		{
			get;
			private set;
		}

		/// <summary>
		/// The ability cooldown, in ticks.
		/// </summary>
		public int Cooldown
		{
			get;
			private set;
		}

		/// <summary>
		/// Gets or sets the current cooldown, in ticks.
		/// 
		/// If this value is not zero, it means the ability cannot
		/// be currently used.
		/// </summary>
		public int CurrentCooldown
		{
			get;
			set;
		}

		/// <summary>
		/// The adrenaline cost for this ability.
		/// 
		/// Thresholds should have -15 as a value, for example, while
		/// basics should have 8. Maximum adrenaline is capped at 100
		/// and can not go below 0.
		/// </summary>
		public int Adrenaline
		{
			get;
			private set;
		}

		/// <summary>
		/// Gets if the ability is delayed (that is, the next ability
		/// will stack with this one).
		/// 
		/// For example, Bombardment is delayed; Shadow Tendrils will
		/// not be affected by the prayer switch.
		/// </summary>
		public bool Delayed
		{
			get;
			private set;
		}

		/// <summary>
		/// Gets if the ability is a threshold and thus requires more than 50 adrenaline.
		/// </summary>
		public bool IsThreshold
		{
			get { return Adrenaline == -15; }
		}

		/// <summary>
		/// Gets if the ability is a basic, and thus restores adrenaline.
		/// </summary>
		public bool IsBasic
		{
			get { return !IsThreshold; }
		}

		/// <summary>
		/// Gets the name of the ability.
		/// </summary>
		public string Name
		{
			get;
			private set;
		}

		/// <summary>
		/// Creates an ability with the provided parameters.
		/// </summary>
		/// <param name="cooldown">Cooldown should be in seconds and is converted to ticks.</param>
		public Ability(Simulation simulation, string name, float minDamage, float maxDamage, int duration, float cooldown, int adrenaline, bool delayed = false)
		{
			Name = name;
			MaxDamage = maxDamage;
			MinDamage = minDamage;
			Duration = duration;
			Cooldown = (int)Math.Ceiling(cooldown / 0.6f);
			Adrenaline = adrenaline;
			Delayed = delayed;

			simulation.AddAbility(this);
		}

		/// <summary>
		/// Calculates the base ability damage when used with the provided weapon.
		/// 
		/// This value is before modifiers like void and prayers.
		/// </summary>
		public AbilityDamage CalculateDamage(float weaponDamage)
		{
			return new AbilityDamage()
			{
				Minimum = (MinDamage / 100.0f) * weaponDamage,
				Maximum = (MaxDamage / 100.0f) * weaponDamage
			};
		}
	}
}
