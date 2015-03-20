using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TormentedDemonSimulator
{
	public class Rotation
	{
		List<Ability> abilities = new List<Ability>();

		public CombatStyle Style
		{
			get;
			private set;
		}

		public Ability this[int index]
		{
			get { return abilities[index]; }
		}

		public int Count
		{
			get { return abilities.Count; }
		}

		public Rotation(Simulation simulation, CombatStyle style, params Ability[] abilities)
		{
			Style = style;
			this.abilities.AddRange(abilities);

			simulation.AddRotation(this);
		}

		public float GetAverageDamage(float weaponDamage)
		{
			float averageDamage = 0.0f;

			foreach (var ability in abilities)
			{
				averageDamage += ability.CalculateDamage(weaponDamage).Average;
			}

			return averageDamage;
		}

		public bool IsValid(int adrenaline)
		{
			int estimatedAdrenaline = adrenaline;
			int accumDuration = 0;

			foreach (var ability in abilities)
			{
				if (ability.CurrentCooldown > accumDuration)
					return false;

				if (ability.IsThreshold && estimatedAdrenaline < 50)
					return false;

				estimatedAdrenaline += ability.Adrenaline;
				accumDuration += ability.Duration;
			}

			return true;
		}
	}
}
