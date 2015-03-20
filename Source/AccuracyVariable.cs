using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TormentedDemonSimulator
{
	public enum StyleAccuracy
	{
		Strong,
		Neutral,
		Weak,
		ExtraWeak,
		Paper // or 'extra-weak'
	}

	public abstract class AccuracyVariable
	{
		// These methods are mapped to RuneScape wiki article on accuracy!
		// See: http://runescape.wikia.com/wiki/Hit_chance
		public float F(float a)
		{
			return 0.0008f * (float)Math.Pow(a, 3) + 4 * a + 40;
		}

		public float Accuracy(float level, float weaponLevel)
		{
			return F(level) + 2.5f * F(weaponLevel);
		}

		public float WeaknessModifier(StyleAccuracy accuracy)
		{
			switch (accuracy)
			{
				case StyleAccuracy.Strong:
					return 7.5f;
				case StyleAccuracy.Neutral:
					return 5.5f;
				case StyleAccuracy.Weak:
					return 4.0f;
				case StyleAccuracy.ExtraWeak:
					return 10.0f / 3.0f;
				case StyleAccuracy.Paper:
				default:
					// No defense.
					return 0.0f;
			}
		}

		public float Defense(float level, StyleAccuracy accuracy)
		{
			return F(level) * WeaknessModifier(accuracy);
		}

		public float HitChance(float playerLevel, float playerWeaponLevel, float bonusAccuracy, float opponentDefense, StyleAccuracy style)
		{
			float accuracy = (float)Math.Floor(Accuracy(playerLevel, playerWeaponLevel)) * bonusAccuracy;
			float defense = (float)Math.Floor(Defense(opponentDefense, style));

			if (defense < 1.0f)
			{
				return 1.0f;
			}
			else
			{
				return accuracy / defense;
			}
		}

		public abstract bool GetHit(float hitChance);
	}

	public class RandomAccuracyVariable : AccuracyVariable
	{
		Random random;

		public RandomAccuracyVariable(Random random)
		{
			this.random = random;
		}

		public override bool GetHit(float hitChance)
		{
			return random.NextDouble() <= hitChance;
		}
	}
}
