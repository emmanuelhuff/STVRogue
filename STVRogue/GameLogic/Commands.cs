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

		/*
            Command List:
            1- Move to next node
            2- Use Magic Crystal
            3- Use Healing Potion
            4- Attack
            */
		public Command getUserCommand(){
			int command = Console.Read(); //get user input
			if(command==1 ||command == 2 ||command == 3 ||command == 4 ){
				commandId = command; //if it is a known command, change the id
			}else{
				commandId = -1; //unknown command
			}
			Command userCommand = new Command(commandId); //key press numbers for known commands, -1 for unknown commands
			return userCommand; //erturn userCommand
		}
	}
}
