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
		public List<Item> itemsToSeed;

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
			uint numberOfNodesInZone = 0;
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
					numberOfNodesInZone = (uint)z.nodesInZone.Count; //get number of nodes (N)  
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

			itemsToSeed = new List<Item>();
			int itemTotal = 0;
			Logger.log("Upper limit " + (0.8 * getHPM()));
			while (getItemsHP() <= (0.8 * getHPM()))
			{
				int decide = RandomGenerator.rnd.Next(0, 2); //0 or 1, 0 means create healing potion, 1 crystal
				if (decide == 0)
				{
					//create healing potion
					HealingPotion healingPotion = new HealingPotion("" + itemTotal);
					itemTotal++;
					itemsToSeed.Add(healingPotion);
					Logger.log("Created healing potion " + healingPotion.id);
				}
				else if (decide == 1)
				{
					//create crystal
					Crystal crystal = new Crystal("" + itemTotal);
					itemTotal++;
					itemsToSeed.Add(crystal);
					Logger.log("Created crystal " + crystal.id);
				}
				else
				{
					Logger.log("Something went wrong");
				}
				Logger.log("Current itemsToSeedHP " + getItemsHP());

			}//remove last created item for property to hold
			itemsToSeed.RemoveAt(itemsToSeed.Count - 1);

			Logger.log("Current getITemsHP " + getItemsHP());

			//RANDOMLY SEED ITEMS
			//Currently puts all items in the dungeon at the creation
			int numberOfItemsToPut = itemsToSeed.Count;
			Logger.log("Number of items to put in total : " + numberOfItemsToPut);
			int itemsInZone = (int)(numberOfItemsToPut / (int)difficultyLevel); //Check if it is integer division
			int itemsIndex = 0;
			while (numberOfItemsToPut > 0)//while there are monsters to put
			{
				foreach (Zone z in dungeon.zones)
				{

					if (z.id == difficultyLevel)
					{   //if it is the last zone
						itemsInZone = numberOfItemsToPut; //put remainder items

					}
                    
					Logger.log("Will put " + itemsInZone + " items to the zone " + z.id);
					numberOfNodesInZone = (uint)z.nodesInZone.Count; //get number of nodes (N)  
					for (int i = 0; i < itemsInZone;i++){
						int nodeNumber = RandomGenerator.rnd.Next(0, (int)numberOfNodesInZone); //randomly pick which node to locate
                        Node nodeToLocate = z.nodesInZone.ElementAt<Node>(nodeNumber);
						Item itemToAdd = itemsToSeed.ElementAt<Item>((int)(itemsIndex * itemsInZone + i)); //starts from 0 for level 1, 
						Logger.log("Putting item positioned " + (itemsIndex * itemsInZone + i)+ " to "+z.id);
						nodeToLocate.items.Add(itemToAdd);
						numberOfItemsToPut--;
					}
					itemsIndex++; 
                                   

				}
			}

			//RANDOMLY SEED ITEMS
			//while sum of HP values of healing potions in the dungeon < 0.8 * getHPM
			// create items in a way that the property holds
			//Decide randomly creating crystal or healing potion


			//randomly seed these items from itemlist
			// divide it equally into zones (last zone can have remainder)
			//for each zone place it randomly





		}

		/*
		 * A single update turn to the game. 
		 */
		public Boolean update(Command userCommand)
		{
			Logger.log("Player does " + userCommand);
			return true;
		}

		//ADDED
		public int getProportion(uint numberOfMonsters, int k, uint l)
		{
			double doubleVersion = ((2 * k * numberOfMonsters) / ((l + 1) * (l + 2)));
			return (int)doubleVersion;

		}

		//ADDED
		//Returns total hp values of itemsToSeed list
		public int getItemsHP()
		{
			int itemsHP = 0;
			foreach (Item i in itemsToSeed)
			{
				if (i.GetType() == typeof(HealingPotion))
				{
					itemsHP += (int)((HealingPotion)i).HPvalue;
				}
			}
			return itemsHP;
		}

		//ADDED
		public int getHPM()
		{
			int totalHPOfMonsters = 0;
			foreach (Zone z in this.dungeon.zones)
			{
				totalHPOfMonsters += z.getZoneHPValue();
			}
			return totalHPOfMonsters;
		}


		public class GameCreationException : Exception
		{
			public GameCreationException() { }
			public GameCreationException(String explanation) : base(explanation) { }
		}
	}
}
