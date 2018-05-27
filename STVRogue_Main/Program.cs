using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using STVRogue.GameLogic;
using STVRogue.Utils;

namespace STVRogue
{
    /* A dummy top-level program to run the STVRogue game */
    class Program
    {
        static void Main(string[] args)
        {
            Game game = new Game(5, 2, 20); //Initializes the game
			game.player.location = game.dungeon.startNode; //sets player's first location in the game
			game.player.collectItems(); //player collects items in its position(startnode)

			int nextState = 0;
			int command = -1;


            //The game continues while the player is alive
			while (game.player.HP>0 && game.player.location != game.dungeon.exitNode)
            {
				
				 //Could not debug it without terminal, Visual studio community does not have integrated terminal
				 
				 
				//While it is not contested,
				//  player can move in the dungeon,
				// can use an item
				Logger.log("Press 1 to move an adjacent node."); //TO-DO: Add you can move these nodes, select the node you want to move..
				Logger.log("Adjacent nodes:");
				foreach(Node n in game.player.location.neighbors)
                {
                    Logger.log(n.id);
                }
				if(game.player.containsMagicCrystal())
				    Logger.log("Press 2 to use a Crystal");
				if (game.player.containsHealingPotion())
				    Logger.log("Press 3 to use a Healing Potion");
                command = game.player.getNextCommand().commandId;
				while(command==-1){
					Logger.log("Waiting for the user input");
					command = game.player.getNextCommand().commandId;
				}

                if (command == 1)
                {
					game.player.move();
                }
                else if (command == 2)
                {
					if (!game.player.containsMagicCrystal())
                    {
                        Logger.log("Player has no magic crystal, press a valid command");

					}else{
						//player uses magic crystal
                        foreach (Item i in game.player.bag)
                        {
                            if (i.GetType() == typeof(Crystal))
                            {
                                game.player.use(i);
                                Logger.log("Player used a crystal");
                                break; //break the for loop
                            }
                            Logger.log("Could not be able to execute this");

                        }
					}
                    
                }
                else if (command == 3)
                {
					if (!game.player.containsHealingPotion())
                    {
                        Logger.log("Player has no healing potion, press a valid command");

					}else{
						foreach (Item i in game.player.bag)
                        {
                            if (i.GetType() == typeof(HealingPotion))
                            {
                                game.player.use(i);
                                Logger.log("Player used potion");
                                break; //break the for loop
                            }
                            Logger.log("Could not be able to execute this");

                        }
					}
                    
                }
				while(game.player.location.packs.Count>0 && game.player.HP>0){//node is contested
					Logger.log("Calling with state " + nextState);
					Logger.log("You have encountered a pack of Orcs! A battle commences...");
					nextState = game.player.location.fight(game.player, nextState); //it calls the fight function with the current state
					if(nextState==-1 || nextState == 5 || nextState == 6){
						nextState = 0;
						break;
					}


				} //REMOVE THIS COMMENT */
            }
			Logger.log("Player died, the game ends..");


        }
    }
}
