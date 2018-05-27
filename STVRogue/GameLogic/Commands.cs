using System;
namespace STVRogue.GameLogic
{
    public class Command
    {
		public int commandId;      
        public Command() { }
		public Command(int commandId) {
			this.commandId = commandId;
		}
        override public string ToString() { return "no-action"; }
      
	}
}
