# TDS, a Tormented Demon Simulator
Unlike most monsters in RuneScape, Tormented Demons (TDs) are pretty
predictable. This simulator is a basic attempt at finding the most efficient
ability rotations and styles.

The calculator assumes perfect reaction, but this can be tweaked by editing
Program.cs (in fact, the entire program can be tweaked that way).

Keep in mind, the more simulations configured, the longer you'll have to wait
for results. The simulation should be tick-accurate based on much research into
the behavior of TDs.

# Configuration
To prepare the simulation, edit CreateSimulation. Generally, the defaults
provided are fine.

To alter specific rotations and priorities, edit the
Add[TwoHanded/DualWield][Magic/Range/Melee] methods. If you find behavior that
is not accounted for, then various properties in the constructor of either
Ability or Rotation will have to be modified.

For example, Wild Magic (prior to the combat 'responsiveness' update) would only
have the first hit land when using two-handed magic weapons if the first hit
triggered the TD's damage threshold. After the responsiveness update, both hits
will hit before the prayer switch regardless of weapon. There's other examples
that can be gleaned from the calculator.

You can also tweak the ability, weapon styles, accuracy, and damage, equipment,
etc. View the Simulate method to see a full list with some documentation.

# Output
The simulator will give a pretty accurate indicator to the efficiency of a
certain setup once run. You have to optimize for maximum kills per hour by
tweaking the rotations and equipment. Generally, the provided rotations were
some of the most optimal prior to the responsiveness update.

# License
The simulator is provided under the MIT license. View LICENSE for more
information.