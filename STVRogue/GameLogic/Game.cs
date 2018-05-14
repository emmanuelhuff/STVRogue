using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using STVRogue.Utils;

namespace STVRogue.GameLogic
{
	public class Game
	{
		public Player player;
		public Dungeon dungeon;

		/* This creates a player and a random dungeon of the given difficulty level and node-capacity
         * The player is positioned at the dungeon's starting-node.
         * The constructor also randomly seeds monster-packs and items into the dungeon. The total
         * number of monsters are as specified. Monster-packs should be seeded as such that
         * the nodes' capacity are not violated. Furthermore the seeding of the monsters
         * and items should meet the balance requirements stated in the Project Document.
         */
		public Game(uint difficultyLevel, uint nodeCapacityMultiplier, uint numberOfMonsters)
		{
			Logger.log("Creating a game of difficulty level " + difficultyLevel + ", node capacity multiplier "
					   + nodeCapacityMultiplier + ", and " + numberOfMonsters + " monsters.");
			//call dungeon constructor
			dungeon = new Dungeon(difficultyLevel, nodeCapacityMultiplier);
			player = new Player();
			player.location = dungeon.startNode;
			// TO-DO: randomly seed monster-packs and items into the dungeon
			while (player.HP != 0)
			{ //&& monsters exist
			  //RANDOMLY SEED MONSTERS
			  //while there is monsters to put
				foreach (Zone z in dungeon.zones)
				{ //Start from last zone
				  //put 1 monster-pack in a time               
				  //get number of nodes (N)               
				  //randomly pick which node to locate
				  //check if capacity is enough if not try to find another node (for 4 times)

					//min = 2, max = nodeCapacity signifies the range for number of monsters in a pack
					//decide how many monsters will be in this monster-pack
					// put them in this zone
					// subtract number of monsters from total number of monsters or add the subtracted numbers into an int, and check it at the while loop
					// pass to other zone
				}

				//RANDOMLY SEED ITEMS


			}


		}

		/*
         * A single update turn to the game. 
         */
		public Boolean update(Command userCommand)
		{
			Logger.log("Player does " + userCommand);
			return true;
		}




		public class GameCreationException : Exception
		{
			public GameCreationException() { }
			public GameCreationException(String explanation) : base(explanation) { }
		}
	}
}
