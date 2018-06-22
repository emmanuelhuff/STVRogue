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
        public static Game game = new Game(5, 2, 20);

        static void consoleLogger(Creature c)
        {
            Logger.log("Press 1 to move an adjacent node. Current node: " + c.location.id); //TO-DO: Add you can move these nodes, select the node you want to move..
            Logger.log("Adjacent nodes: ");
            foreach (Node n in c.location.neighbors)
            {
                Logger.log(n.id);
            }
            Logger.log("Press 2 to do nothing.");
            if (game.player.containsMagicCrystal())
                Logger.log("Press 3 to use a Healing Potion");
            if (game.player.containsHealingPotion() && game.isContested())
                Logger.log("Press 4 to use a Magic Crystal");
            if (game.isContested())
            { //currently consoleLogger is not called when game is constested

                Logger.log("Press 5 to flee");
                Logger.log("Press 6 to fight");


            }


        }

        /*static void consoleLoggerPack(Pack p)
        {
            //Logger.log("Press 1 to move an adjacent node."); //TO-DO: Add you can move these nodes, select the node you want to move..
            //Logger.log("Adjacent nodes: ");
            foreach (Node n in p.location.neighbors)
            {
                Logger.log(n.id);
            }
            //Logger.log("Press 2 to do nothing.");

            if (game.isContested() && p.location.id == game.player.location.id) //currently consoleLoggerPack is not called when game is constested
            {
                Logger.log("Press 5 to flee");
                Logger.log("Press 6 to fight");
            }


        }*/

        static void Main(string[] args)
        {
            //game = new Game(5, 2, 20); //Initializes the game
            game.player.location = game.dungeon.startNode; //sets player's first location in the game
            game.player.collectItems(); //player collects items in its position(startnode)

            int nextState = 0;


            GamePlay gp = new GamePlay();
            gp.saveInitialGame(game);

            // take out later??
            
            gp.saveToFile("test.xml", game);
            //GamePlay gp2 = new GamePlay("testOneTurn.xml");
            //gp2.reset();
            //game = gp2.getState();
            

            uint x = game.player.dungeon.difficultyLevel;
            //The game continues while the player is alive or the player reaches the exit node
            while (game.player.HP > 0 && game.player.location != game.dungeon.exitNode)
            {
                Logger.log("Turn is " + game.turnCount);

                if (game.isContested())
                {

                    Logger.log("CONTESTED!");
                    Logger.log("You have encountered a pack of Orcs! A battle commences...");

                    while (game.isContested())
                    {
                        Logger.log("Calling with state " + nextState);
                        nextState = game.player.location.fight(game.player, nextState);
                        if (nextState == -1 || nextState == 5 || nextState == 6)
                        {
                            nextState = 0;
                            Logger.log("Not contested anymore");
                            foreach (Zone z in game.dungeon.zones)
                            {
                                z.onR_Alert = false;
                            }
                            //br
                        }
                        Logger.log("The packs that are not combatting are playing:");
                        //not combating packs play
                        List<Pack> packList = game.dungeon.GetPacks();

                        foreach (Pack p in packList)
                        {
                            Node packLocation = p.location;
                            if (packLocation.id != game.player.location.id)
                            { //not combatting
                                Logger.log("Pack " + p.id + "'s turn, pack node: " + packLocation.id);

                                game.activePack = p;
                                //consoleLoggerPack(p);
                                Command command = game.player.getNextCommand();

                                while (!game.update(command, game.turn, game.dungeon.zones.ElementAt(packLocation.level - 1)))
                                {
                                    Logger.log("enter a valid command");
                                    command = game.player.getNextCommand();
                                }

                            }


                        }

                        game.turnCount++;

                    }
                    //game is not contested anymore, player's turn to play
                    game.turn = true;


                }
                else
                {

                    if (game.turn)
                    { //player's turn
                        Logger.log("Player's turn");
                        consoleLogger(game.player);
                        Command command = game.player.getNextCommand();

                        while (command.commandId == -1)
                        {
                            command = game.player.getNextCommand();
                        }
                        game.update(command, game.turn, null);



                        game.turn = false;
                        game.turnCount++;
                    }
                    else
                    { //monster pack's turn
                        Logger.log("Pack's turn: ");

                        List<Pack> packList = game.dungeon.GetPacks();
                        foreach (Pack p in packList)
                        {
                            Logger.log(p.id);
                        }
                        Logger.log("Press 1 to move packs"); 
                        Redo:
                            Command command = game.player.getNextCommand();
                        if (command.commandId != 1)
                        {
                            Logger.log("Invalid command. Try again");
                            goto Redo;
                        }
                        else
                        {
                            foreach (Pack p in packList)
                            {
                                Node packLocation = p.location;
                                Logger.log("Pack " + p.id + "'s turn, pack node: " + packLocation.id);

                                game.activePack = p;
                                //consoleLoggerPack(p);
                                game.update(command, game.turn, game.dungeon.zones.ElementAt(packLocation.level - 1));
                                
                            }
                        }




                        game.turn = true;
                        game.turnCount++;

                        /*//only selecting one pack at a time
						int zoneIndex = game.player.location.level - 1;


						//Boolean found = false;
						Zone packZone = game.dungeon.zones.ElementAt(zoneIndex);
						Pack pack;
						if(packZone.monstersInZone > 0){
							//a pack in this zone can either move or do nothing
							foreach (Node n in packZone.nodesInZone)
                            {
                                if (n.packs.Count > 0)
                                {
                                    pack = n.packs.First();
                                    if (pack == null)
                                    {
                                        Logger.log("Pack is NULL?"); //ideally raise an exception
                                    }
                                    //we have the pack
                                    game.activePack = pack;
                                    consoleLoggerPack(pack);
                                    break;
                                }
                            }
							Command command = game.player.getNextCommand();
                            while (!game.update(command, game.turn))
                            {
                                Logger.log("enter a valid command");
                                command = game.player.getNextCommand();
                            }

                            game.turn = true;
                            game.turnCount++;

						}else{
							Logger.log("No monster pack in the zone, turn passes to player");
							game.turn = true;
                            game.turnCount++;
						}*/

                    }
                    //gp.saveToFile("testOneTurn.xml", game);

                }

            }
            if (game.player.location == game.dungeon.exitNode)
            {
                Logger.log("CONGRATULATIONS, you reached the end!");
            }
            else if (game.player.HP <= 0)
            {
                Logger.log("Player died..");
            }
            Logger.log("The game ends.");


        }

    }
}
