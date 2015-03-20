using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TormentedDemonSimulator
{
	public struct SimulationData
	{
		/// <summary>
		/// The total amount of damage dealt during the simulation.
		/// </summary>
		public float TotalDamage;

		/// <summary>
		/// The overkill damage dealt during the simulation.
		/// 
		/// If a demon has 2000 health left and 6000 damage is dealt,
		/// the overkill will be incremented by 4000 (damageDealt - remainingHealth).
		/// </summary>
		public float OverkillDamage;

		/// <summary>
		/// Max potential soul split healing.
		/// 
		/// This is if soul split was used on every att ack.
		/// </summary>
		public float SoulSplitHealing;

		/// <summary>
		/// Player attacks that missed, due to accuracy (not prayer switching).
		/// </summary>
		public int AttacksMissed;

		/// <summary>
		/// Player attacks that hit.
		/// </summary>
		public int AttacksHit;

		/// <summary>
		/// The number of Tormented Demons killed during the simulation.
		/// </summary>
		public int Kills;

		/// <summary>
		/// The fastest kill, in ticks.
		/// </summary>
		public int FastestKill;

		/// <summary>
		/// The slowest kill, in ticks.
		/// </summary>
		public int SlowestKill;

		/// <summary>
		/// The number of limbs dropped...
		/// </summary>
		public int Limbs;

		/// <summary>
		/// This rating goes up for every inefficient action that had to be taken.
		/// 
		/// For example, if all supplied rotations are unavailable, this value goes up.
		/// 
		/// This number should be zero in most cases.
		/// </summary>
		public int InefficiencyRating;

		/// <summary>
		/// A log, detailing the events that occurred while fighting the Tormented Demons
		/// for the duration.
		/// </summary>
		public string[] Log;
	}

	public class Simulation
	{
		List<Ability> abilities = new List<Ability>();
		Dictionary<CombatStyle, Style> styles = new Dictionary<CombatStyle, Style>();

		// Used to try and heal some.
		float sacrificeThreshold;
		Ability sacrifice;

		// Player data.
		float prayerBonus = 1.1f;
		float helmetDamageBonus = 1.0f;
		float helmetAccuracyBonus = 1.0f;
		float potionBonus = 0.0f;
		bool hasAsylumSurgeonRing;

		bool useFamiliarScrolls;
		Familiar familiar;
		int familiarScrollCooldown;

		int currentTicks;

		bool isLogging = false;
		bool countTicks = false;
		List<string> log = new List<string>();

		void WriteEvent(string s, params object[] objs)
		{
			if (isLogging)
			{
				if (countTicks)
					log.Add(String.Format("[{0}]: {1}", currentTicks, String.Format(s, objs)));
				else
					log.Add(String.Format(s, objs));
			}
		}

		public void EnableLog(bool countTicks)
		{
			isLogging = true;
			this.countTicks = countTicks;
		}

		/// <summary>
		/// Modifiers the range of damage.
		/// </summary>
		public DamageVariable Damage
		{
			get;
			private set;
		}

		/// <summary>
		/// Modifies accuracy.
		/// </summary>
		public AccuracyVariable Accuracy
		{
			get;
			private set;
		}

		/// <summary>
		/// Time it takes to switch gear.
		/// </summary>
		public IntervalVariable GearSwitch
		{
			get;
			private set;
		}

		/// <summary>
		/// Time between Tormented Demons.
		/// </summary>
		public IntervalVariable Idle
		{
			get;
			private set;
		}

		/// <summary>
		/// Chance of an effect occuring.
		/// 
		/// Includes:
		///   - Asylum surgeon's ring activiting if enabled (10%)
		///   - Tormented Demon's weakness switching (50% to bolt, 50% to fire)
		/// </summary>
		public EffectVariable Effect
		{
			get;
			private set;
		}

		/// <summary>
		/// Modifies the behavior of a familiar.
		/// </summary>
		public FamiliarVariable FamiliarBehavior
		{
			get;
			private set;
		}

		/// <summary>
		/// For the funnies!
		/// </summary>
		public LimbDropperVariable LimbDrop
		{
			get;
			set;
		}

		public Simulation(DamageVariable damage, AccuracyVariable accuracy, IntervalVariable gear, IntervalVariable idle, EffectVariable effect, FamiliarVariable familiar)
		{
			styles.Add(CombatStyle.Magic, new Style());
			styles.Add(CombatStyle.Range, new Style());
			styles.Add(CombatStyle.Melee, new Style());

			Damage = damage;
			Accuracy = accuracy;
			GearSwitch = gear;
			Idle = idle;
			Effect = effect;
			FamiliarBehavior = familiar;
		}

		public void SetPrimaryBasic(CombatStyle style, Ability ability)
		{
			styles[style].PrimaryBasic = ability;
		}

		public void EnableSacrifice(float healthThreshold, Ability ability)
		{
			sacrificeThreshold = healthThreshold;
			sacrifice = ability;
		}

		public void SetWeapon(CombatStyle style, DamageType damageType, float abilityDamage, float weaponTier)
		{
			Style s = styles[style];
			s.AbilityDamage = abilityDamage;
			s.DamageType = damageType;
			s.WeaponTier = weaponTier;
		}

		public void SetHelmBonus(float accuracy, float damage)
		{
			helmetAccuracyBonus = accuracy;
			helmetDamageBonus = damage;
		}

		public void SetAsylumSurgeonRing(bool enabled)
		{
			hasAsylumSurgeonRing = enabled;
		}

		public void SetLevelBoost(float value)
		{
			potionBonus = value;
		}

		public void SetFamiliar(Familiar familiar, float scrollTimer)
		{
			this.familiar = familiar;

			if (scrollTimer == 0)
				useFamiliarScrolls = false;

			this.familiarScrollCooldown = (int)(scrollTimer / 0.6f);
		}

		public void AddAbility(Ability ability)
		{
			abilities.Add(ability);
		}

		public void AddRotation(Rotation rotation)
		{
			styles[rotation.Style].AddRotation(rotation);
		}

		void UpdateAbilities()
		{
			foreach (Ability ability in abilities)
			{
				if (ability.CurrentCooldown > 0)
				{
					--ability.CurrentCooldown;
				}
			}
		}

		// Player and demon data.
		Player player = new Player();
		Demon demon = new Demon();

		CombatStyle currentGearStyle;
		CombatStyle nextGearStyle;
		int currentGlobalCooldown, currentIdleCooldown;
		int demonAge;

		void SwitchGear(CombatStyle newGear, bool instant = false)
		{
			WriteEvent("Switching to {0} gear.", newGear);

			currentSimulationState = SimulationState.SwitchingGear;

			if (!instant)
			{
				currentIdleCooldown = GearSwitch.GetInterval();
				nextGearStyle = newGear;
			}
			else
			{
				currentGearStyle = newGear;
			}
		}

		void SwapGear()
		{
			currentRotation = GetNextPreferredRotation(player, currentGearStyle);
			currentPlayerAbility = 0;

			if (currentRotation == null)
			{
				if (currentSimulationState != SimulationState.TrySwitchGear)
				{
					WriteEvent("[Warning] All rotations are unavailable; idling...");
				}

				currentSimulationState = SimulationState.TrySwitchGear;
				++simulationData.InefficiencyRating;
			}
			else
			{
				SwitchGear(currentRotation.Style);
			}
		}

		int currentPlayerAbility;
		Rotation currentRotation;

		bool CanSwitch()
		{
			return currentGearStyle == demon.Prayer;
		}

		bool NextRotation()
		{
			// Find preferred rotation based on current style.
			currentRotation = GetPreferredRotation(player);

			if (currentRotation != null)
			{
				currentPlayerAbility = 0;
				currentSimulationState = SimulationState.AttackingDemon;

				return true;
			}

			// Otherwise, idle. This shouldn't happen with the rotations at the moment,
			// but with trial-and-error of new rotations in the future, it may...
			return false;
		}

		void AttackDemon()
		{
			Ability ability = null;
			if (currentPlayerAbility < currentRotation.Count)
			{
				ability = currentRotation[currentPlayerAbility++];
			}
			else if (demon.Health < sacrificeThreshold && sacrifice != null && sacrifice.CurrentCooldown == 0)
			{
				WriteEvent("The demon is low on health; using Sacrifice...");

				ability = sacrifice;
			}
			else if (!CanSwitch())
			{
				WriteEvent("The rotation failed to force a prayer switch; using primary basic...");

				// The rotation is finished, but the demon is still protecting against our
				// next style. Finish with basics.
				ability = styles[currentGearStyle].PrimaryBasic;
			}
			else
			{
				// Switch gear and return.
				SwapGear();

				return;
			}

			float weaponDamage = styles[currentGearStyle].AbilityDamage;

			if (ability == null)
			{
				// ??? Technically this shouldn't happen unless melee gets implemented and this
				// method doesn't get updated. Just do nothing.
				return;
			}

			ability.CurrentCooldown = ability.Cooldown;
			currentGlobalCooldown = ability.Duration;

			// First check for a miss.
			StyleAccuracy styleAccuracy = styles[currentRotation.Style].DamageType.Compare(demon.Weakness);
			float hitChance = Accuracy.HitChance(99 + potionBonus, styles[currentGearStyle].WeaponTier, helmetAccuracyBonus, demon.DefenseLevel, styleAccuracy);
			bool missed = !Accuracy.GetHit(hitChance);

			int adrenaline = ability.Adrenaline;

			if (ability.IsThreshold && hasAsylumSurgeonRing && Effect.GetOccurred(0.1f))
			{
				WriteEvent("Your ring glows brightly; you do not lose any adrenaline for triggering this attack!");
				adrenaline = 0;
			}

			if (player.Adrenaline >= 50)
				WriteEvent("Adrenaline stable, currently at {0}.", player.Adrenaline);
			else
				WriteEvent("Adrenaline low, currently at {0}.", player.Adrenaline);

			player.Adrenaline = Math.Min(player.Adrenaline + adrenaline, 100);

			if (missed)
			{
				WriteEvent("{0} was blocked by the Tormented Demon's defense...", ability.Name);
				++simulationData.AttacksMissed;

				return;
			}
			else
			{
				++simulationData.AttacksHit;
			}

			AbilityDamage abilityDamage = ability.CalculateDamage(weaponDamage);

			abilityDamage.Minimum = abilityDamage.Minimum * helmetDamageBonus * prayerBonus;
			abilityDamage.Maximum = abilityDamage.Maximum * helmetDamageBonus * prayerBonus;

			float potionBonusMin = potionBonus * 4;
			float potionBonusMax = potionBonus * 8;

			float baseDamage = Damage.GetDamage(abilityDamage.Minimum, abilityDamage.Maximum);
			float potionDamage = Damage.GetDamage(potionBonusMin, potionBonusMax);
			float finalDamage = baseDamage + potionDamage;

			float soulSplitDamage = Math.Min(finalDamage, demon.Health);
			float soulSplitHealing = 0.0f;
			bool attackMissed = false;

			if (currentGearStyle == demon.Prayer)
			{
				WriteEvent("{0} was blocked by the Tormented Demon's prayer...", ability.Name);
				attackMissed = true;
			}
			else
			{
				WriteEvent("The Tormented Demon took {0:n0} damage from {1}.", Math.Min(finalDamage, demon.Health), ability.Name);

				// Health restoration stages:
				// - 10% for initial 1 to 2000 damage
				// - 5% for 2001 to 4000 damage
				// - 1.25% for remaining damage
				float stage1 = Math.Min(soulSplitDamage, 2000) * 0.1f;
				float stage2 = Math.Min(Math.Max(soulSplitDamage - 2000, 0), 2000) * 0.05f;
				float stage3 = Math.Max(soulSplitDamage - 4000, 0) * 0.0125f;
				soulSplitHealing = (float)Math.Floor(stage1 + stage2 + stage3);
			}

			bool wouldKillDemon = Math.Floor(finalDamage) >= demon.Health;
			if (!attackMissed)
			{
				if (wouldKillDemon)
				{
					soulSplitHealing = soulSplitDamage * 0.25f;
				}

				WriteEvent("Restored {0:n} health with Soul Split.", soulSplitHealing);
				simulationData.SoulSplitHealing += soulSplitHealing;
			}

			DamageDemon(finalDamage, currentGearStyle, false, ability.Delayed);
		}

		void UpdatePlayer()
		{
			currentGlobalCooldown = Math.Max(currentGlobalCooldown - 1, 0);
			currentIdleCooldown = Math.Max(currentIdleCooldown - 1, 0);

			if (currentSimulationState == SimulationState.AttackingDemon)
			{
				if (currentGlobalCooldown == 0)
				{
					AttackDemon();
				}
			}
			else if (currentSimulationState == SimulationState.Idling)
			{
				if (currentIdleCooldown == 0)
				{
					if (NextRotation())
					{
						currentSimulationState = SimulationState.AttackingDemon;
					}
					else
					{
						++simulationData.InefficiencyRating;
						WriteEvent("[Warning] Idling after kill because all range rotations are unavailable.");
					}
				}
			}
			else if (currentSimulationState == SimulationState.SwitchingGear)
			{
				if (currentIdleCooldown == 0)
				{
					currentGearStyle = nextGearStyle;
					currentSimulationState = SimulationState.AttackingDemon;
				}
			}
			else if (currentSimulationState == SimulationState.TrySwitchGear)
			{
				SwapGear();
			}
		}

		void ChangeDemonWeakness()
		{
			if (Effect.GetOccurred(0.5f))
				demon.Weakness = DamageType.Fire;
			else
				demon.Weakness = DamageType.Bolt;
		}

		void NextDemon()
		{
			WriteEvent("The Tormented Demon was slain in {0} ticks ({1} seconds)!", demonAge, demonAge * 0.6);

			demon = new Demon();
			ChangeDemonWeakness();

			simulationData.FastestKill = Math.Min(simulationData.FastestKill, demonAge);
			simulationData.SlowestKill = Math.Max(simulationData.SlowestKill, demonAge);
			demonAge = 0;

			if (this.familiar != null)
			{
				isFamiliarStupid = FamiliarBehavior.GetStupid(this.familiar.StupidValue);

				if (isFamiliarStupid)
					WriteEvent("THe familiar is loafing around!");
				else
					WriteEvent("The familiar is ready for the next Tormented Demon!");
			}

			SwitchGear(CombatStyle.Range, true);

			// Usually idling should take longer, but regardless,
			// take whichever would be the longer event. They happen
			// at the same time, so the longer will always be the
			// total duration.
			int idle1 = GearSwitch.GetInterval();
			int idle2 = Idle.GetInterval();
			int idle = Math.Max(idle1, idle2);
			currentSimulationState = SimulationState.Idling;

			WriteEvent("Idling for {0} ticks ({1} seconds).", idle, idle * 0.6f);

			currentIdleCooldown = idle;
		}

		bool DamageDemon(float damage, CombatStyle style, bool familiar, bool delayed)
		{
			damage = (float)Math.Floor(damage);
			bool killed = false;

			if (style != demon.Prayer)
			{
				float currentDamage = 0;

				if (style == CombatStyle.Magic)
				{
					demon.CurrentMageDamage += Math.Max(damage, 200);
					currentDamage = demon.CurrentMageDamage;
				}
				else if (style == CombatStyle.Range)
				{
					demon.CurrentRangeDamage += Math.Max(damage, 200);
					currentDamage = demon.CurrentRangeDamage;
				}
				else if (style == CombatStyle.Melee)
				{
					demon.CurrentMeleeDamage += Math.Max(damage, 200);
					currentDamage = demon.CurrentMeleeDamage;
				}

				bool resetCurrentDamage = false;

				if (!delayed)
				{
					if (familiar)
					{
						if (currentDamage >= 800)
						{
							demon.Prayer = style;
							resetCurrentDamage = true;
						}
					}
					else
					{
						if (currentDamage >= 3100)
						{
							demon.Prayer = style;
							resetCurrentDamage = true;
						}
					}
				}

				demon.Health -= damage;

				if (demon.Health <= 0)
				{
					killed = true;
					++simulationData.Kills;

					if (LimbDrop != null && LimbDrop.GetLimb())
					{
						WriteEvent("A golden beam shines over one of your items.");

						++simulationData.Limbs;
					}

					simulationData.OverkillDamage += Math.Abs(demon.Health);

					NextDemon();
				}
				else if (resetCurrentDamage)
				{
					demon.CurrentMageDamage = 0;
					demon.CurrentMeleeDamage = 0;
					demon.CurrentRangeDamage = 0;
					ChangeDemonWeakness();

					WriteEvent("The Tormented Demon switched to Protect from {0} after taking {1} total damage from that style!", style, currentDamage);

					// Assume we don't instantly know that a familiar attack will
					// switch prayers. Also, if we are in a rotation, skip switching gear.
					// This adds a small but realistic delay.
					if (!familiar && currentPlayerAbility == currentRotation.Count || currentPlayerAbility == 0)
					{
						SwapGear();
					}
				}

				simulationData.TotalDamage += damage;
			}

			return killed;
		}

		// Familiar data.
		bool isFamiliarStupid;
		int currentFamiliarAttackCooldown = 0;
		int currentFamiliarScrollCooldown = 0;
		int summoningPointsRestore;

		void UpdateFamiliar()
		{
			summoningPointsRestore++;

			if (summoningPointsRestore >= 50 && player.SummoningPoints < 60)
			{
				// 15 points every 30 seconds (50 ticks)
				player.SummoningPoints = Math.Min(player.SummoningPoints + 15, 60);

				WriteEvent("Restored up to 15 special move points, now at {0} total.", player.SummoningPoints);
			}

			if (familiar != null && currentSimulationState != SimulationState.Idling)
			{
				currentFamiliarAttackCooldown = Math.Max(currentFamiliarAttackCooldown - 1, 0);
				currentFamiliarScrollCooldown = Math.Max(currentFamiliarScrollCooldown - 1, 0);

				if (!isFamiliarStupid && currentFamiliarAttackCooldown == 0)
				{
					bool useScroll = (currentFamiliarScrollCooldown == 0 && player.SummoningPoints >= familiar.ScrollCost && useFamiliarScrolls);

					if (useScroll)
					{
						player.SummoningPoints -= familiar.ScrollCost;
					}

					// Perform attack.
					CombatStyle style = familiar.GetStyle(FamiliarBehavior.GetStyleValue(), useScroll);

					if (useScroll)
						WriteEvent("The familiar is readying itself for a powerful {0} attack!", style);

					int hits = 1;
					if (useScroll)
						hits = familiar.ScrollHits;

					float damage = 0;
					for (int i = 0; i < hits; ++i)
					{
						if (FamiliarBehavior.GetHit(familiar.HitChance))
							damage += Damage.GetDamage(1, familiar.MaxHit);
					}

					if (damage < 1 || style == demon.Prayer)
						WriteEvent("The familiar missed...");
					else
						WriteEvent("The familiar did {0:n0} {1} damage.", damage, style);

					DamageDemon(damage, style, true, false);

					if (useScroll)
					{
						currentFamiliarScrollCooldown = familiarScrollCooldown;
						player.SummoningPoints -= familiar.ScrollCost;
					}

					currentFamiliarAttackCooldown = familiar.AttackInterval;
				}
			}
		}

		void Tick()
		{
			demonAge++;
			currentTicks++;

			UpdateAbilities();
			UpdateFamiliar();
			UpdatePlayer();
		}

		Rotation GetPreferredRotation(Player player)
		{
			return styles[currentGearStyle].GetPreferredRotation(player);
		}

		Rotation GetNextPreferredRotation(Player player, CombatStyle currentStyle)
		{
			Style a, b;

			if (currentStyle == CombatStyle.Magic)
			{
				a = styles[CombatStyle.Range];
				b = styles[CombatStyle.Melee];
			}
			else if (currentStyle == CombatStyle.Range)
			{
				a = styles[CombatStyle.Magic];
				b = styles[CombatStyle.Melee];
			}
			else
			{
				a = styles[CombatStyle.Magic];
				b = styles[CombatStyle.Range];
			}

			Rotation x = a.GetPreferredRotation(player);
			Rotation y = b.GetPreferredRotation(player);

			if (x == null)
				return y;
			else if (y == null)
				return x;
			else if (x.GetAverageDamage(a.AbilityDamage) > y.GetAverageDamage(b.AbilityDamage))
				return x;
			else
				return y;
		}

		SimulationData simulationData = new SimulationData();

		enum SimulationState
		{
			AttackingDemon,
			TrySwitchGear,
			SwitchingGear,
			Idling
		}

		SimulationState currentSimulationState = SimulationState.Idling;

		/// <summary>
		/// Performs a simulation for the provided number of ticks.
		/// </summary>
		public SimulationData Simulate(int ticks = 6000)
		{
			simulationData.FastestKill = Int32.MaxValue;
			simulationData.SlowestKill = Int32.MinValue;

			currentGearStyle = CombatStyle.Range;
			NextRotation();

			if (familiar != null)
			{
				currentFamiliarAttackCooldown = familiar.AttackInterval;
			}

			// Randomize weakness.
			ChangeDemonWeakness();

			while (ticks-- > 0)
			{
				Tick();
			}

			if (isLogging)
			{
				simulationData.Log = log.ToArray();
			}

			return simulationData;
		}
	}
}
