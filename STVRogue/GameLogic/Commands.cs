using System.Runtime.Serialization;
using System;
namespace STVRogue.GameLogic
{
    [DataContract(Name = "Command", Namespace = "STVRogue.GameLogic")]
    public class Command
    {
        [DataMember()]
        public int commandId;
        [DataMember()]
        public string commandString;
        [DataMember()]
        public bool valid;

        /**
         * 1: move to an adjacent node
         * 2: do nothing
         * 3: use a healing potion
         * 4: use magic crystal
         * 5: flee
         * 6: fight
         */

        public Command(int commandId)
        {
            this.commandId = commandId;
            this.valid = true;
            if (commandId == 1)
            {
                this.commandString = "moves to an adjacent node";
            }
            else if (commandId == 2)
            {
                this.commandString = "does nothing";
            }
            else if (commandId == 3)
            {
                this.commandString = "uses a healing potion";
            }
            else if (commandId == 4)
            {
                this.commandString = "uses magic crystal";
            }
            else if (commandId == 5)
            {
                this.commandString = "chooses to flee";
            }
            else if (commandId == 6)
            {
                this.commandString = "chooses to fight";
            }
            else
            {
                this.commandString = "'s move should be valid";
                this.valid = false;
            }
        }
        override public string ToString() { return commandString; }

    }
}
