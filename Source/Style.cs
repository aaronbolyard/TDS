using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TormentedDemonSimulator
{
	public class Style
	{
		List<Rotation> rotations = new List<Rotation>();

		Ability primaryBasic;
		public Ability PrimaryBasic
		{
			get { return primaryBasic; }
			set { primaryBasic = value; }
		}

		DamageType damageType = DamageType.None;
		public DamageType DamageType
		{
			get { return damageType; }
			set { damageType = value; }
		}

		public float AbilityDamage
		{
			get;
			set;
		}

		public float WeaponTier
		{
			get;
			set;
		}

		public Rotation GetPreferredRotation(Player player)
		{
			foreach (var rotation in rotations)
			{
				if (rotation.IsValid(player.Adrenaline))
					return rotation;
			}

			// No valid rotation! They're all on cooldown.
			return null;
		}

		public void AddRotation(Rotation rotation)
		{
			rotations.Add(rotation);
		}
	}
}
