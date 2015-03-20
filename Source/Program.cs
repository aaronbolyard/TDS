using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TormentedDemonSimulator
{
	class Program
	{
		/// <summary>
		/// Used by all simulated styles.
		/// </summary>
		/// <returns></returns>
		static Simulation CreateSimulation()
		{
			// Parameters for the simulation.
			Random random = new Random();

			// Manages the damage spread.
			// Random damage would be evenly distributed between min and max.
			DamageVariable damage = new RandomDamageVariable(random);

			// Determines how long it takes to switch gear between rotations.
			//IntervalVariable gear = new RandomIntervalVariable(random, 4, 6);
			IntervalVariable gear = new RandomIntervalVariable(random, 1, 1);

			// Determines the cooldown between killing Tormented Demons.
			//IntervalVariable idle = new RandomIntervalVariable(random, 5, 7);
			IntervalVariable idle = new RandomIntervalVariable(random, 1, 1);

			// Accuracy!
			AccuracyVariable accuracy = new RandomAccuracyVariable(random);

			// Effects, for things like Asylum surgeon's ring and the Tormented Demon's weakness.
			EffectVariable effect = new RandomEffectVariable(random);

			// Optional. Determines a familiar's behavior.
			FamiliarVariable familiar = new RandomFamiliarVariable(random);

			// Apply the parameters.
			Simulation simulation = new Simulation(damage, accuracy, gear, idle, effect, familiar);

			// Hahaha...
			simulation.LimbDrop = new RandomLimbDropperVariable(random);

			return simulation;
		}

		static void AddTwoHandedMagic(Simulation simulation)
		{
			// Magic basic abilities.
			Ability DragonBreath = new Ability(simulation, "Dragon Breath", 37.6f, 188, 3, 10, 8);
			Ability Wrack = new Ability(simulation, "Wrack", 18.8f, 94, 3, 3, 8);
			Ability SonicWave = new Ability(simulation, "Sonic Wave", 31.4f, 157, 3, 5, 8);

			// Magic thresholds.
			Ability WildMagic1 = new Ability(simulation, "Wild Magic", 50, 215, 3, 20, -15);
			Ability WildMagic2 = new Ability(simulation, "Wild Magic", 50, 215, 0, 0, 0, true);
			Ability Asphyxiate1 = new Ability(simulation, "Asphyxiate", 37.6f, 188, 3, 20, -15);
			Ability Asphyxiate2 = new Ability(simulation, "Asphyxiate", 37.6f, 188, 0, 0, 0, true);

			// Magic rotations, in order of preference.
			Rotation mage1 = new Rotation(simulation, CombatStyle.Magic, WildMagic1, WildMagic2, Wrack);
			Rotation mage2 = new Rotation(simulation, CombatStyle.Magic, Asphyxiate1, Asphyxiate2, Wrack);
			Rotation mage3 = new Rotation(simulation, CombatStyle.Magic, SonicWave, DragonBreath);

			// Ability used when the rotation does not cause a switch to Protect from Magic.
			simulation.SetPrimaryBasic(CombatStyle.Magic, Wrack);
		}

		/// <summary>
		/// Adds the abilities for dual-wielded magic.
		/// </summary>
		static void AddDualWieldMagic(Simulation simulation)
		{
			// Magic basic abilities.
			Ability DragonBreath = new Ability(simulation, "Dragon Breath", 37.6f, 188, 3, 10, 8);
			Ability Wrack = new Ability(simulation, "Wrack", 18.8f, 94, 3, 3, 8);
			Ability ConcentratedBlast1 = new Ability(simulation, "Concentrated Blast", 15, 75, 3, 5, 8);
			Ability ConcentratedBlast2 = new Ability(simulation, "Concentrated Blast", 16.4f, 82, 0, 0, 0);

			// Magic thresholds.
			Ability WildMagic1 = new Ability(simulation, "Wild Magic", 50, 215, 3, 20, -15, true);
			Ability WildMagic2 = new Ability(simulation, "Wild Magic", 50, 215, 0, 0, 0);
			Ability Asphyxiate1 = new Ability(simulation, "Asphyxiate", 37.6f, 188, 3, 20, -15);
			Ability Asphyxiate2 = new Ability(simulation, "Asphyxiate", 37.6f, 188, 0, 0, 0, true);
			//Ability Asphyxiate2 = new Ability(simulation, "Asphyxiate", 37.6f, 188, 0, 0, 0, false);

			// Magic rotations, in order of preference.
			Rotation mage1 = new Rotation(simulation, CombatStyle.Magic, ConcentratedBlast1, ConcentratedBlast2, WildMagic1, WildMagic2);
			Rotation mage2 = new Rotation(simulation, CombatStyle.Magic, Asphyxiate1, Asphyxiate2, Wrack);
			Rotation mage3 = new Rotation(simulation, CombatStyle.Magic, ConcentratedBlast1, ConcentratedBlast2, DragonBreath);

			// Ability used when the rotation does not cause a switch to Protect from Magic.
			simulation.SetPrimaryBasic(CombatStyle.Magic, Wrack);
		}

		static void AddTwoHandedRange(Simulation simulation)
		{
			// Range basic abilities.
			Ability DazingShot = new Ability(simulation, "Dazing Shot", 31.4f, 157, 3, 5, 8);
			Ability Snipe = new Ability(simulation, "Snipe", 125, 219, 6, 10, 8);
			Ability PiercingShot = new Ability(simulation, "Piercing Shot", 18.8f, 94, 3, 3, 8);

			// Range thresholds.
			Ability Snapshot1 = new Ability(simulation, "Snapshot", 100, 120, 3, 20, -15, true);
			Ability Snapshot2 = new Ability(simulation, "Snapshot", 100, 210, 0, 0, 0);
			Ability Bombardment = new Ability(simulation, "Bombardment", 43.8f, 219, 3, 30, -15, true);
			Ability ShadowTendrils = new Ability(simulation, "Shadow Tendrils", 66, 500, 3, 45, -15);

			// Range rotations, in order of preference.
			Rotation range1 = new Rotation(simulation, CombatStyle.Range, DazingShot, Bombardment, ShadowTendrils);
			Rotation range2 = new Rotation(simulation, CombatStyle.Range, DazingShot, Snapshot1, Snapshot2);
			Rotation range3 = new Rotation(simulation, CombatStyle.Range, DazingShot, Bombardment, PiercingShot);
			Rotation range4 = new Rotation(simulation, CombatStyle.Range, DazingShot, Snipe);

			// Ability used when the rotation does not cause a switch to Protect from Range.
			simulation.SetPrimaryBasic(CombatStyle.Range, PiercingShot);
		}

		/// <summary>
		/// Adds the abilities for dual-wielded range.
		/// </summary>
		static void AddDualWieldRange(Simulation simulation)
		{
			// Range basic abilities.
			Ability NeedleStrike = new Ability(simulation, "Needle Strike", 31.4f, 157, 3, 5, 8);
			Ability Snipe = new Ability(simulation, "Snipe", 125, 219, 6, 10, 8);
			Ability PiercingShot = new Ability(simulation, "Piercing Shot", 18.8f, 94, 3, 3, 8);

			// Range thresholds.
			Ability Snapshot1 = new Ability(simulation, "Snapshot", 100, 120, 3, 20, -15, true);
			Ability Snapshot2 = new Ability(simulation, "Snapshot", 100, 210, 0, 0, 0);
			Ability Bombardment = new Ability(simulation, "Bombardment", 43.8f, 219, 3, 30, -15, true);
			Ability ShadowTendrils = new Ability(simulation, "Shadow Tendrils", 66, 500, 3, 45, -15);

			// Range rotations, in order of preference.
			Rotation range1 = new Rotation(simulation, CombatStyle.Range, NeedleStrike, Bombardment, ShadowTendrils);
			Rotation range2 = new Rotation(simulation, CombatStyle.Range, NeedleStrike, Snapshot1, Snapshot2);
			Rotation range3 = new Rotation(simulation, CombatStyle.Range, NeedleStrike, Bombardment, PiercingShot);
			Rotation range4 = new Rotation(simulation, CombatStyle.Range, NeedleStrike, Snipe);

			// Ability used when the rotation does not cause a switch to Protect from Range.
			simulation.SetPrimaryBasic(CombatStyle.Range, PiercingShot);
		}

		static void AddTwoHandedMelee(Simulation simulation)
		{
			// Melee basic abilities.
			Ability Slice = new Ability(simulation, "Slice", 22, 110, 3, 3, 8);
			Ability Sever = new Ability(simulation, "Sever", 37.6f, 188, 3, 15, 8);
			Ability Fury1 = new Ability(simulation, "Fury", 15, 75, 3, 5, 8);
			Ability Fury2 = new Ability(simulation, "Fury", 16.4f, 82, 0, 0, 0);

			// Melee thresholds.
			Ability Assault1 = new Ability(simulation, "Assault", 43.8f, 219, 3, 30, -15, true);
			Ability Assault2 = new Ability(simulation, "Assault", 43.8f, 219, 0, 0, 0);
			Ability Hurricane = new Ability(simulation, "Hurricane", 4.8f, 219, 3, 20, -15);

			// Melee rotations, on order of preference.
			Rotation melee1 = new Rotation(simulation, CombatStyle.Melee, Assault1, Assault2);
			Rotation melee2 = new Rotation(simulation, CombatStyle.Melee, Hurricane, Sever);
			Rotation melee3 = new Rotation(simulation, CombatStyle.Melee, Fury1, Fury2, Slice);

			// Ability used when the rotation does not cause a switch to Protect from Melee
			simulation.SetPrimaryBasic(CombatStyle.Melee, Slice);
		}

		/// <summary>
		/// Adds the abilities for dual-wielded melee.
		/// </summary>
		/// <param name="simulation"></param>
		static void AddDualWieldMelee(Simulation simulation)
		{
			// Melee basic abilities.
			Ability Slice = new Ability(simulation, "Slice", 22, 110, 3, 3, 8);
			Ability Sever = new Ability(simulation, "Sever", 37.6f, 188, 3, 15, 8);
			Ability Fury1 = new Ability(simulation, "Fury", 15, 75, 3, 5, 8);
			Ability Fury2 = new Ability(simulation, "Fury", 16.4f, 82, 0, 0, 0);

			// Melee thresholds.
			Ability Assault1 = new Ability(simulation, "Assault", 43.8f, 219, 3, 30, -15, true);
			Ability Assault2 = new Ability(simulation, "Assault", 43.8f, 219, 0, 0, 0);
			Ability Destroy1 = new Ability(simulation, "Destroy", 37.6f, 188, 3, 20, -15);
			Ability Destroy2 = new Ability(simulation, "Destroy", 37.6f, 188, 0, 0, 0);

			// Melee rotations, on order of preference.
			Rotation melee1 = new Rotation(simulation, CombatStyle.Melee, Assault1, Assault2);
			Rotation melee2 = new Rotation(simulation, CombatStyle.Melee, Destroy1, Destroy2, Sever);
			Rotation melee3 = new Rotation(simulation, CombatStyle.Melee, Fury1, Fury2, Slice);

			// Ability used when the rotation does not cause a switch to Protect from Melee
			simulation.SetPrimaryBasic(CombatStyle.Melee, Slice);
		}

		/// <summary>
		/// Enables sacrifice at the provided threshold.
		/// </summary>
		static void EnableSacrifice(Simulation simulation, float threshold)
		{
			Ability Sacrifice = new Ability(simulation, "Sacrifice", 20, 100, 3, 30, 8);
			simulation.EnableSacrifice(400, Sacrifice);
		}

		static SimulationData Simulate()
		{
			Simulation simulation = CreateSimulation();

			// Abilities, ability rotations, and weapon parameters.
			// Dual-wielded weapons.
			//  - Seismic wand + singularity
			AddDualWieldMagic(simulation);
			simulation.SetWeapon(CombatStyle.Magic, DamageType.Fire, 1608, 80);
			////  - Main-hand ascension crossbow + off-hand ascension crossbow
			AddDualWieldRange(simulation);
			simulation.SetWeapon(CombatStyle.Range, DamageType.Bolt, 1752, 90);
			////  - Main-hand drygore mace + off-hand drygore mace
			//AddDualWieldMelee(simulation);
			//simulation.SetWeapon(CombatStyle.Melee, DamageType.Crush, 1752, 90);

			// Two-handed weapons.
			// - Noxious staff
			//AddTwoHandedMagic(simulation);
			//simulation.SetWeapon(CombatStyle.Magic, DamageType.Fire, 1752, 90);
			// - Noxious bow
			//AddTwoHandedRange(simulation);
			//simulation.SetWeapon(CombatStyle.Range, DamageType.Arrow, 1752, 90);
			// - Noxious scythe
			//AddTwoHandedMelee(simulation);
			//simulation.SetWeapon(CombatStyle.Melee, DamageType.Slash, 1752, 90);

			EnableSacrifice(simulation, 600.0f);

			// Weapon parameters.

			// Hybrid helm bonus (e.g., slayer helm or void armour).
			//
			// Full slayer helmet:
			//   12.5% accuracy, 12.5% damage
			//
			// Void:
			//   5% bonus damage for elite, 7% for superior elite
			//   3% accuracy for all as of now.
			simulation.SetHelmBonus(1.00f, 1.00f);
			
			// Boost from potions.
			//
			// Overload is +17 at 99 stats.
			// Supreme overload is +19 at 99 stats.
			simulation.SetLevelBoost(17);

			// Various other useful equipment.
			simulation.SetAsylumSurgeonRing(true);

			// Familiars.
			//
			// IronTitanFamiliar behaves like a normal iron titan.
			// StevTitanFamiliar behaves like a 100% optimal Steel Titan.
			//
			// For making Steel Titan 100% optimal, see:
			//   http://forum.tip.it/topic/325010-wip-150-kph-tormented-demons/#entry5500161
			//
			simulation.SetFamiliar(new IronTitanFamiliar(), 5);

			// Final parameters.
			bool EnableLogging = true;
			bool EnableDurationLogging = true;
			int SimulationTime = 6000; // 6000 ticks = one hour

			if (EnableLogging)
				simulation.EnableLog(EnableDurationLogging);

			return simulation.Simulate(SimulationTime);
		}

		static void Main(string[] args)
		{
			// Simulation 'X' hours of Tormented Demons.
			int MaxSimulations = 1;
			bool OutputIndividualSimulations = false;

			List<SimulationData> simulationData = new List<SimulationData>();
			float averageKills = 0, averageDamage = 0, averageOverkill = 0, averageSoulSplitHealth = 0;
			float averageHits = 0, averageMisses = 0;
			int totalLimbs = 0;
			int fastestKill = Int32.MaxValue;
			int slowestKill = Int32.MinValue;
			int maxKills = Int32.MinValue;
			int minKills = Int32.MaxValue;

			for (int i = 0; i < MaxSimulations; ++i)
			{
				SimulationData data = Simulate();

				averageKills += data.Kills;
				averageDamage += data.TotalDamage;
				averageOverkill += data.OverkillDamage;
				averageSoulSplitHealth += data.SoulSplitHealing;
				averageHits += data.AttacksHit;
				averageMisses += data.AttacksMissed;
				totalLimbs += data.Limbs;

				fastestKill = Math.Min(fastestKill, data.FastestKill);
				slowestKill = Math.Max(slowestKill, data.SlowestKill);

				maxKills = Math.Max(data.Kills, maxKills);
				minKills = Math.Min(data.Kills, minKills);

				simulationData.Add(data);
			}

			averageKills /= MaxSimulations;
			averageDamage /= MaxSimulations;
			averageOverkill /= MaxSimulations;
			averageSoulSplitHealth /= MaxSimulations;
			averageHits /= MaxSimulations;
			averageMisses /= MaxSimulations;

			Console.WriteLine("{0} hour(s) of Tormented Demon slaying results in the averages of...", MaxSimulations);
			Console.WriteLine("... {0} kills per hour", (int)averageKills);
			Console.WriteLine("... {0:n0} damage per hour", (int)averageDamage);
			Console.WriteLine("... {0:n0} overkill damage per hour", (int)averageOverkill);
			Console.WriteLine("... {0:n0} health possibly restored by Soul Split", (int)averageSoulSplitHealth);
			Console.WriteLine("... {0:n0} attacks connected, while {1:n0} were off the mark.", averageHits, averageMisses);
			Console.WriteLine("... {0:n0} max kills in an hour, {1:n0} min kills.", maxKills, minKills);
			Console.WriteLine();
			Console.WriteLine("The fastest kill took {0} ticks ({1:n1} seconds).", fastestKill, fastestKill * 0.6f);
			Console.WriteLine("On the other hand, the slowest kill took {0} ticks ({1:n1} seconds).", slowestKill, slowestKill * 0.6f);

			if (OutputIndividualSimulations)
			{
				for (int i = 0; i < MaxSimulations; ++i)
				{
					Console.WriteLine();
					Console.WriteLine("Simulation {0:d4} resulted in {1} dragon limb(s) and:", i, simulationData[i].Limbs);
					Console.WriteLine("\t- {0} kill(s) (fastest was {1} second(s), slowest was {2} second(s))", simulationData[i].Kills, simulationData[i].FastestKill * 0.6, simulationData[i].SlowestKill * 0.6);
					Console.WriteLine("\t- {0:n0} damage dealt and {1:n0} overkill", (int)simulationData[i].TotalDamage, (int)simulationData[i].OverkillDamage);
					Console.WriteLine("\t- {0:n0} health restored by Soul Split", (int)simulationData[i].SoulSplitHealing);
					Console.WriteLine("\t- {0:n0} attack(s) landed, while {1:n0} missed.", simulationData[i].AttacksHit, simulationData[i].AttacksMissed);
					Console.WriteLine("\t- {0} inefficient action(s) were selected.", simulationData[i].InefficiencyRating);
				}
			}

			if (MaxSimulations == 1 && simulationData[0].Log != null)
			{
				System.IO.File.WriteAllLines("Simulation.log", simulationData[0].Log);
			}

			Console.WriteLine();
			Console.WriteLine("Oh, and with those parameters...");
			Console.WriteLine("...an average of {0} limb drop(s) were simulated.", totalLimbs);

			Console.ReadKey();
		}
	}
}
