using System;
namespace STVRogue.GameLogic
{
    public class Command
    {
        public Command() { }
        override public string ToString() { return "no-action"; }

		internal static int getUserInput()
		{
			Console.WriteLine("Press 4 for .., press 5 for ..");
            int command = Console.Read();
            return command;
		}
	}
}
