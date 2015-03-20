using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TormentedDemonSimulator
{
	public enum CombatStyle
	{
		Magic,
		Range,
		Melee,
		None
	}

	public enum DamageType
	{
		// Melee
		Stab,
		Slash,
		Crush,

		// Range
		Bolt,
		Arrow,
		Thrown,

		// Magic
		Air,
		Water,
		Earth,
		Fire,

		// None
		None
	}

	public static class DamageTypeExtensions
	{
		static public CombatStyle Style(this DamageType type)
		{
			switch (type)
			{
				case DamageType.Stab:
				case DamageType.Slash:
				case DamageType.Crush:
					return CombatStyle.Melee;
				case DamageType.Bolt:
				case DamageType.Arrow:
				case DamageType.Thrown:
					return CombatStyle.Range;
				case DamageType.Air:
				case DamageType.Water:
				case DamageType.Earth:
				case DamageType.Fire:
					return CombatStyle.Magic;
				default:
					return CombatStyle.None;
			}
		}

		// Gets the StyleAccuracy value between two damage types.
		//  - when offense == defense, returns ExtraWeak
		//  - when Style(offense) == Style(defense), returns Neutral
		//  - when Style(offense) > Style(defense), returns Weak
		//  - when Style(offense) < Style(defense), returns Strong
		static public StyleAccuracy Compare(this DamageType offense, DamageType weakness)
		{
			if (offense == weakness)
			{
				return StyleAccuracy.ExtraWeak;
			}
			else
			{
				CombatStyle offenseBaseStyle = Style(offense);
				CombatStyle weaknessBaseStyle = Style(weakness);

				// The monster is melee based.
				if (weaknessBaseStyle == CombatStyle.Magic)
				{
					if (offenseBaseStyle == CombatStyle.Magic)
					{
						return StyleAccuracy.Weak;
					}
					else if (offenseBaseStyle == CombatStyle.Range)
					{
						return StyleAccuracy.Strong;
					}
					else if (offenseBaseStyle == CombatStyle.Melee)
					{
						return StyleAccuracy.Neutral;
					}
				}
				// The monster is magic based.
				else if (weaknessBaseStyle == CombatStyle.Range)
				{
					if (offenseBaseStyle == CombatStyle.Magic)
					{
						return StyleAccuracy.Neutral;
					}
					else if (offenseBaseStyle == CombatStyle.Range)
					{
						return StyleAccuracy.Weak;
					}
					else if (offenseBaseStyle == CombatStyle.Melee)
					{
						return StyleAccuracy.Strong;
					}
				}
				// The monster is range based.
				else if (weaknessBaseStyle == CombatStyle.Melee)
				{
					if (offenseBaseStyle == CombatStyle.Magic)
					{
						return StyleAccuracy.Strong;
					}
					else if (offenseBaseStyle == CombatStyle.Range)
					{
						return StyleAccuracy.Neutral;
					}
					else if (offenseBaseStyle == CombatStyle.Melee)
					{
						return StyleAccuracy.Weak;
					}
				}

				// There is no weakness.
				return StyleAccuracy.Neutral;
			}
		}
	}
}
