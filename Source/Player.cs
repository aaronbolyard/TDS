using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TormentedDemonSimulator
{
	public class Player
	{
		/// <summary>
		/// Gets or sets how many summoning points the player currently has.
		/// </summary>
		public int SummoningPoints
		{
			get;
			set;
		}

		/// <summary>
		/// Gets or sets the adrenaline available.
		/// </summary>
		public int Adrenaline
		{
			get;
			set;
		}

		public Player()
		{
			SummoningPoints = 60;

			// Assumes we sip an adrenaline flask at the beginning
			// of the simulation.
			Adrenaline = 25;
		}
	}
}
