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
			int numberOfMonstersToPut = (int)numberOfMonsters;
			int min = 1, max = 1; //used in while loop to define number of monsters in a pack
			int packId = 0;
			// TO-DO: randomly seed monster-packs and items into the dungeon

			//RANDOMLY SEED MONSTERS
            //Currently puts all monsters in the dungeon at the creation
			Logger.log("Number of monsters to put in total : " + numberOfMonsters);
			while (numberOfMonstersToPut > 0)//while there are monsters to put
			{
				foreach (Zone z in dungeon.zones)
				{

					int monstersInZone = -1;
					if (z.id == difficultyLevel)
					{//if it is the last zone
						monstersInZone = numberOfMonstersToPut; //put remainder monsters
					}
					else
					{
						monstersInZone = getProportion(numberOfMonsters, z.id, difficultyLevel); //gets number of monsters to put in this zone
					}
					Logger.log("Will put " + monstersInZone + " monsters to the zone " + z.id);
					uint numberOfNodesInZone = (uint)z.nodesInZone.Count; //get number of nodes (N)  
					while (monstersInZone > 0)
					{
						int nodeNumber = RandomGenerator.rnd.Next(0, (int)numberOfNodesInZone); //randomly pick which node to locate
						Node nodeToLocate = z.nodesInZone.ElementAt<Node>(nodeNumber);

						int nodeCapacity = (int)z.capacity - (nodeToLocate.currentNumberOfMonsters()); //number of monsters that can locate
																									   //check the capacity nodeCapacity, if less than 1 try another node, else create a monster pack of 
						Logger.log("Node to locate: " + nodeToLocate.id + " with capacity " + nodeCapacity);                                                                               //size min=1 max=nodeCapacity   
						if (nodeCapacity > 1)
						{
							if (nodeCapacity < monstersInZone) max = nodeCapacity;
							else max = monstersInZone;
							int monstersToLocate = RandomGenerator.rnd.Next(min, max + 1); //decide how many monsters will be in this monster-pack                        
							Pack newPack = new Pack("" + packId, (uint)monstersToLocate); //Create a pack
							Logger.log("Putting " + monstersToLocate + " monsters in pack" + packId + " locating in " + nodeToLocate.id);
							packId++; //increase pack ID
							z.nodesInZone.ElementAt<Node>(nodeNumber).packs.Add(newPack); //add pack to the node
							monstersInZone -= monstersToLocate; //decrease number of monsters to be located in the zone
							numberOfMonstersToPut -= monstersToLocate; //decrease number of monsters to be located in the dungeon
							Logger.log("monsters to locate in zone: " + monstersInZone + " in dungeon: " + numberOfMonstersToPut);
						}

					}

				}
			}



			//RANDOMLY SEED ITEMS





		}

		/*
		 * A single update turn to the game. 
		 */
		public Boolean update(Command userCommand)
		{
			Logger.log("Player does " + userCommand);
			return true;
		}

		public int getProportion(uint numberOfMonsters, int k, uint l)
		{
			double doubleVersion = ((2 * k * numberOfMonsters) / ((l + 1) * (l + 2)));
			return (int)doubleVersion;

		}




		public class GameCreationException : Exception
		{
			public GameCreationException() { }
			public GameCreationException(String explanation) : base(explanation) { }
		}
	}
}
