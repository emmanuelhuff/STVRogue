﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using STVRogue.Utils;
using System.Runtime.Serialization;
using System.IO;

namespace STVRogue.GameLogic
{
    [DataContract(Name = "Game", Namespace = "STVRogue.GameLogic")]
    public class Game
    {
        [DataMember()]
        public Player player;
        [DataMember()]
        public Dungeon dungeon;
        [DataMember()]
        public List<Item> itemsToSeed;
        public Random random = RandomGenerator.rnd;
        [DataMember()]
        public bool turn = true; //true if player is playing, false if monster is playing
        [DataMember()]
        public int turnCount = 0;
        [DataMember()]
        public Pack activePack;
        [DataMember()]
        public MemoryStream initialGame; // used by gameplay
        //public int state;

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

            dungeon = new Dungeon(difficultyLevel, nodeCapacityMultiplier);//call dungeon constructor
            player = new Player();
            player.dungeon = dungeon;
            player.location = dungeon.startNode;
            int numberOfMonstersToPut = (int)numberOfMonsters; //a temporary variable to keep track of number of monsters to put in the dungeon
            int min = 1, max = 1; //used in while loop to define number of monsters in a pack
            int packId = 0;
            uint numberOfNodesInZone = 0; //a temporary variable to store number of nodes in a zone (in foreach loop)


            //Randomly seeds monsters into the dungeon
            //Currently puts all monsters in the dungeon at the creation
            Logger.log("Number of monsters to put in total : " + numberOfMonsters);
            while (numberOfMonstersToPut > 0)//while there are monsters to put in the dungeon
            {
                foreach (Zone z in dungeon.zones) //Seeds monsters zone by zone
                {

                    int monstersInZone = -1; // -1 is just for control does not have any meaning
                    if (z.id == difficultyLevel + 1)
                    {//if it is the last zone
                        z.monstersInZone = numberOfMonstersToPut; //put remainder monsters

                    }
                    else //else every zone gets proportioned number of monsters
                    {
                        z.monstersInZone = getProportion(numberOfMonsters, z.id, difficultyLevel); //gets number of monsters to put in this zone

                    }
                    monstersInZone = z.monstersInZone;
                    Logger.log("Will put " + z.monstersInZone + " monsters to the zone " + z.id);
                    numberOfNodesInZone = (uint)z.nodesInZone.Count; //get number of nodes (N)  

                    while (monstersInZone > 0) //while there are monsters to put in the zone
                    {
                        int nodeNumber = random.Next(0, (int)numberOfNodesInZone); //randomly pick which node to locate
                        Node nodeToLocate = z.nodesInZone.ElementAt<Node>(nodeNumber); //get this node instance

                        int nodeCapacity = (int)z.capacity - (nodeToLocate.currentNumberOfMonsters()); //number of monsters that can locate in that node
                                                                                                       //check the capacity nodeCapacity, if less than 1 try another node, else create a monster pack of size min=1 max=nodeCapacity   
                        Logger.log("Node to locate: " + nodeToLocate.id + " with capacity " + nodeCapacity);
                        if (nodeCapacity > 1)
                        {
                            //the upper limit for max is either the node's capacity or remaining number of monsters that should be located in this zone
                            if (nodeCapacity < z.monstersInZone) max = nodeCapacity; //if node capacity is less than remaining monsters to put, update max limit
                            else max = monstersInZone;

                            int monstersToLocate = random.Next(min, max + 1); //decide how many monsters will be in this monster-pack between this number limit                    
                            Pack newPack = new Pack("" + packId, (uint)monstersToLocate, this.dungeon); //Create a pack
                            Logger.log("Putting " + monstersToLocate + " monsters in pack" + packId + " locating in " + nodeToLocate.id);
                            newPack.location = z.nodesInZone.ElementAt<Node>(nodeNumber);//Assign this pack's location
                            packId++; //increase pack ID
                            z.nodesInZone.ElementAt<Node>(nodeNumber).packs.Add(newPack); //add pack to the node
                            monstersInZone -= monstersToLocate; //decrease number of monsters to be located in the zone

                            numberOfMonstersToPut -= monstersToLocate; //decrease number of monsters to be located in the dungeon
                            Logger.log("monsters to locate in zone: " + z.monstersInZone + " in dungeon: " + numberOfMonstersToPut);
                        }

                    }

                }
            }

            itemsToSeed = new List<Item>(); //stores the list of items to be seeded in the dungeon
            int itemTotal = 0;
            //There is a constraint for HP value of the player & HP values of items that player has in the bag
            //and HP values of items exist in the dungeon
            Logger.log("Upper limit " + (0.8 * getHPM()));
            while (getItemsHP() <= (0.8 * getHPM())) //while this constraint is satisfied, it creates items
            {
                int decide = random.Next(0, 2); //0 or 1, 0 means create healing potion, 1 means create magic crystal 
                if (decide == 0)
                {
                    //create healing potion
                    HealingPotion healingPotion = new HealingPotion("" + itemTotal); //create it with id
                    itemTotal++;
                    itemsToSeed.Add(healingPotion); //add it into the list
                    Logger.log("Created healing potion " + healingPotion.id);
                }
                else if (decide == 1)
                {
                    //create crystal
                    Crystal crystal = new Crystal("" + itemTotal);
                    itemTotal++;
                    itemsToSeed.Add(crystal); //add it into the list
                    Logger.log("Created crystal " + crystal.id);
                }
                else
                {
                    Logger.log("Something went wrong");
                }
                Logger.log("Current itemsToSeedHP " + getItemsHP());

            }
            //since it leaves the while loop just after this constraint is passed
            //remove last created item for property to hold
            itemsToSeed.RemoveAt(itemsToSeed.Count - 1);

            Logger.log("Current getITemsHP " + getItemsHP());

            //Randomly seed items in the itemsToSeed list
            //Currently puts all items in the dungeon at the creation
            int numberOfItemsToPut = itemsToSeed.Count; //number of items to seed is the length of the list
            Logger.log("Number of items to put in total : " + numberOfItemsToPut);
            int itemsInZone = (int)(numberOfItemsToPut / ((int)difficultyLevel + 1)); //Equally partition the number of items, except the last zone
            int normalItemsInZone = itemsInZone; //used for indexing the items in itemsToSeed for the last level
                                                 //because itemsInZone for the last level changes, indexing changes
            int itemsIndex = 0; //index of the item in the itemsToSeed list
            while (numberOfItemsToPut > 0)//while there are items to put in the dungeon
            {
                foreach (Zone z in dungeon.zones) //for each zone
                {

                    if (z.id == difficultyLevel + 1)
                    {   //if it is the last zone
                        itemsInZone = numberOfItemsToPut; //put remainder items

                    }

                    Logger.log("Will put " + itemsInZone + " items to the zone " + z.id);
                    numberOfNodesInZone = (uint)z.nodesInZone.Count; //get number of nodes (N) 

                    for (int i = 0; i < itemsInZone; i++)
                    { //for each item to put in this zone
                        int nodeNumber = random.Next(0, (int)numberOfNodesInZone); //randomly pick which node to locate
                        Node nodeToLocate = z.nodesInZone.ElementAt<Node>(nodeNumber); //get this node
                        Item itemToAdd = itemsToSeed.ElementAt<Item>((int)(itemsIndex * normalItemsInZone + i)); //starts from 0 for level 1, 0+number of items put in each zone for level 2
                                                                                                                 //increases by number of items put in zone for every level
                        Logger.log("Putting item positioned " + (itemsIndex * normalItemsInZone + i) + " to " + nodeToLocate.id);
                        nodeToLocate.items.Add(itemToAdd); //add the item to this node
                        numberOfItemsToPut--; //Decrease number of items to put
                    }
                    itemsIndex++; //increase items index 


                }
            }





        }

        //NEVER USED, may be it should be used?
        /*
		 * A single update turn to the game. 
		 */
        public Boolean update(Command userCommand, bool turn2, Zone packZone)
        {
            if (this.turn)
            {
                Logger.log("Player " + userCommand);
            }
            else
            {
                Logger.log("Pack " + userCommand);
            }

            if (!userCommand.valid)
                return false;
            if (userCommand.commandId == 1)
            {
                if (this.turn)
                {
                    this.player.move();
                }
                else
                {

                    if (this.player.location.level == this.dungeon.difficultyLevel && packZone.id == this.dungeon.difficultyLevel)
                    {
                        Logger.log("Pack moves towards to the player because they are both at the last zone.");
                        this.activePack.MoveTowards(this.player.location);

                    }
                    else
                    {
                        if (packZone.onR_Alert)
                        {
                            Logger.log("Pack moves towards to the player because the zone is alerted.");
                            this.activePack.MoveTowards(this.player.location); //can be changed
                        }
                        else
                        {
                            Logger.log("Pack moves to its random neighbour, the zone is not alerted.");
                            this.activePack.Move(this.activePack.location.neighbors.First()); //Else moves to first neighbour, can be changed
                        }
                    }




                }

            }
            else if (userCommand.commandId == 2)
            {
                //nothing
            }
            else if (userCommand.commandId == 3 && turn)
            {
                if (!this.player.containsHealingPotion())
                {
                    Logger.log("Player has no healing potion, press a valid command");

                }
                else
                {
                    foreach (Item i in this.player.bag)
                    {
                        if (i.GetType() == typeof(HealingPotion))
                        {
                            this.player.use(i);
                            Logger.log("Player used potion");
                            break; //break the for loop
                        }

                    }
                }
            }
            else if (userCommand.commandId == 4 && turn)
            {
                if (!this.isContested())
                {
                    Logger.log("Player is not in a combat, can not use magic crystal");
                }
                else
                {
                    if (!this.player.containsMagicCrystal())
                    {
                        Logger.log("Player has no magic crystal, press a valid command");

                    }
                    else
                    {
                        //player uses magic crystal
                        foreach (Item i in this.player.bag)
                        {
                            if (i.GetType() == typeof(Crystal))
                            {
                                this.player.use(i);
                                Logger.log("Player used a crystal");
                                break; //break the for loop
                            }

                        }
                    }
                }

            }
            else if (userCommand.commandId == 5 && turn)
            {
                if (!this.isContested())
                {
                    Logger.log("Player is not in a combat,can not flee");
                }

            }
            else if (userCommand.commandId == 6 && turn)
            {
                if (!this.isContested())
                {
                    Logger.log("Player is not in a combat,can not fight");
                }

            }
            else
            {
                Logger.log("INVALID");
                return false;
            }

            return true;
        }


        /*
         * Returns the number of monsters that this level should have regarding the proportion 
         */

        public int getProportion(uint numberOfMonsters, int k, uint l)
        {
            double doubleVersion = ((2 * k * numberOfMonsters) / ((l + 1) * (l + 2))); //proportion specified in the document
            return (int)doubleVersion; //returns the bottom value |_ _|

        }

        //Returns total hp values of itemsToSeed list
        public int getItemsHP()
        {
            int itemsHP = 0;
            foreach (Item i in itemsToSeed) //for each item in the list
            {
                if (i.GetType() == typeof(HealingPotion)) //if item is a healing potion
                {
                    itemsHP += (int)((HealingPotion)i).HPvalue; //Add its HP value
                }
            }
            return itemsHP; //return total hp of the itemsToSeed list
        }


        /*
         * Returns total hp of all monsters in the dungeon
        */
        public int getHPM()
        {
            int totalHPOfMonsters = 0;
            foreach (Zone z in this.dungeon.zones) //for each zone
            {
                totalHPOfMonsters += z.getZoneHPValue(); //adds up zone HP values
            }
            return totalHPOfMonsters; //Returns total hp of all monsters in the dungeon
        }

        public Boolean isContested()
        {
            return this.player.location.packs.Count > 0;
        }

        /**
         * Thrown when game creation fails
         */

        public class GameCreationException : Exception
        {
            public GameCreationException() { }
            public GameCreationException(String explanation) : base(explanation) { }
        }
    }
}
