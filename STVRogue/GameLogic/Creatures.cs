using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using STVRogue.Utils;

namespace STVRogue.GameLogic
{
	public abstract class Creature //contains common fields of Monster and Player
	{
		public String id;
		public String name;
		public int HP;
		public uint AttackRating = 1; //Attack Rating=1 means when it attacks, it decreases 1 hp value
		public Node location; //its location
		public abstract void Attack(Creature foe);      
	}

    public class Monster : Creature
    {
        public Pack pack; //each monster belongs to a pack

        /* Create a monster with a random HP */
        public Monster(String id)
        {
            this.id = id; name = "Orc";
            HP = 1 + RandomGenerator.rnd.Next(6); //random hp between 1-6
        }

		public override void Attack(Creature foe)
        {
            foe.HP = (int)Math.Max(0, foe.HP - AttackRating); //hp can not be less than 0
            //Logger.log("Creature's HP is " + foe.HP);
            String killMsg = foe.HP == 0 ? ", KILLING it" : ""; //if creature dies
            Logger.log("Creature " + id + " attacks " + foe.id + killMsg + ".");
        }

		public void setHP(int newHP){ //changes monster's hp
			this.HP = newHP;
		}
    }

    public class Player : Creature
    {
        public Dungeon dungeon;
        public int HPbase = 100;
        public Boolean accelerated = false; //true after player uses magic crystal
        public uint KillPoint = 0; //describes number of monsters beated
        public List<Item> bag = new List<Item>(); //list of items that player has
        public Player()
        {
            id = "player";
            AttackRating = 5;
			HP = HPbase;
        }
        /**
         * Returns true if player's bag contains magic crystal
         */

		public bool containsMagicCrystal(){
			foreach(Item i in bag){ //for each item in the bag
				if (i.GetType() == typeof(Crystal)) //if it is magic crystal
					return true; //return true
			}
			return false;
		}
		/**
         * Returns true if player's bag contains healing potion
         */
		public bool containsHealingPotion()
        {
            foreach (Item i in bag)
            {
				if (i.GetType() == typeof(HealingPotion)) //if it is healing potion
                    return true;
            }
            return false;
        }
        /**
         * Removes item from player's bag after it is used
         */

        public void use(Item item)
        {
            if (!bag.Contains(item) || item.used) throw new ArgumentException();
            item.use(this);
            bag.Remove(item); //this is added, item.used is not used
        }

        /**
         * returns sum of hp values of healing potions in player's bag
         */

		public int getHPValueOfBag(){
			int bagHPValue = 0;
			foreach(Item i in this.bag){ //for each item in the bag
				if(i.GetType()== typeof(HealingPotion)){ //if it is type healing potion
					bagHPValue += (int)((HealingPotion)i).HPvalue; //increase bag hp value
				}
			}
			return bagHPValue;
				
		}

        /**
         * Gets user input and returns command
         */

		public Command getNextCommand(){
			
			string userInput ="";
			userInput = Console.ReadLine();
			int command = -1;
			if(int.TryParse(userInput,out command)){ //user input should be int
				if (command != 1 && command != 2 && command != 3 && command != 4)
                {
                    Logger.log("Unknown command");
					command = -1;
				}
			}else{
				Logger.log("Input should be an integer.");
			}
            
			Command userCommand = new Command(command); //key press numbers for known commands, -1 for unknown commands
            return userCommand;
		}

        /**
         * Returns true if player can flee.
         * Player can only flee if a not contested adjacent node exists
         */

		public Boolean flee()
        {
			
			Node currentLocation = this.location; //current location of the player
			List<Node> adjacentNodes = currentLocation.neighbors; //adjacent nodes
			foreach(Node adjNode in adjacentNodes){            
					//check if it will be contested, if then do not flee to that node
					if (adjNode.packs.Count == 0) //if there is no pack
					{
						
						this.location = adjNode;//change location and flee
						Logger.log("Player fleed from " + currentLocation + " to " + this.location);
						this.collectItems(); //collect items in the new node
						return true;
					}
				
			}
			return false;

        }

		/*OLD FLEE, are we sure that player can not flee to a node from the diff zone?
        public Boolean flee()
        {

            Node currentLocation = this.location;
            int currentLevel = currentLocation.level;
            List<Node> adjacentNodes = currentLocation.neighbors;
            int zoneLevel;
            foreach (Node adjNode in adjacentNodes)
            {
                zoneLevel = adjNode.level;
                if (currentLevel == zoneLevel)
                {
                    //check if it will be contested, if then do not flee to that node
                    if (adjNode.packs.Count == 0)
                    {
                        //change location and flee
                        this.location = adjNode;
                        Logger.log("Player fleed from " + currentLocation + " to " + this.location);
                        this.collectItems();
                        return true;
                    }
                } //else do nothing, it can not flee to a node from the different zone
            }
            return false;

        }*/

		/**
		 * Player moves in the dungeon
		 */

        public void move()
        {         
            Node currentLocation = this.location;
            List<Node> adjacentNodes = currentLocation.neighbors; //get neighbors of the location
			int nodeIndex = RandomGenerator.rnd.Next(0, adjacentNodes.Count); //randomly decide which node to move
			this.location = adjacentNodes.ElementAt(nodeIndex); //change player's location
			Logger.log("Player moved from "+currentLocation.id+" to " + this.location.id);

			this.collectItems();//Collect items in this location
        }

        /**
         * Player automatically collects colocated items and puts in the bag
         */

		public void collectItems(){
			Node currentLocation = this.location;
			foreach(Item i in currentLocation.items.ToList()){ //for each item in the same location
				currentLocation.items.Remove(i); //remove them from node's item list
				this.bag.Add(i); //add into player's bag
				Logger.log("Collected item " + i.id);
			}
		}

        
        public override void Attack(Creature foe)
        {
            if (!(foe is Monster)) throw new ArgumentException();
            Monster foe_ = foe as Monster; //player can only attack a monster

            Pack tempPack = foe_.pack; //monster's pack

            Node packLocation = tempPack.location; //same as player's location

            //gives information to the user
            Logger.log("Location is " + tempPack.location.id);
            Logger.log("All monsters in the pack: ");
            foreach (Monster m in tempPack.members)
            {
                Logger.log(m.id);
            }
            Logger.log("All packs in location ");
            foreach (Pack p in packLocation.packs)
            {
                Logger.log(p.id);
            }
            if (!accelerated) //if user is not accelerated, player only attacks to parameter monster
            {
				foe.HP = (int)Math.Max(0, foe.HP - AttackRating); //HP can not be less than 0
                String killMsg = foe.HP == 0 ? ", KILLING it" : ""; //monster dies
                Logger.log("Creature " + id + " attacks " + foe.id + killMsg + ".");
                if (foe.HP == 0) //if the monster died
                {
                    
                    tempPack.members.Remove(foe_); //remove monster from its pack
                    
                    if(tempPack.members.Count == 0) //check if the pack is empty
                    {
						Logger.log("Pack is now empty' pack id " + tempPack.id);
						Logger.log("Remaining packs in the location:");

						foreach (Pack pack in packLocation.packs)
                        {
                            Logger.log("Pack " + pack.id + " in node " + packLocation);
                        }

						//THIS PART SHOULD BE TESTED, I forgot why Chris and I checked for tempPack==null
                        // We probably forgot to change it back after debugging
                        //commented null comparison

                        //pack is removed from the node, regarding what AttackBool returns to the main flow

                        /*
                        if (tempPack == null)
                        {
                            Logger.log("it is null");
                            foreach (Pack pack in packLocation.packs)
                            {
                                Logger.log("Pack " + pack.id + " in node " + packLocation);
                            }
                        }
                        else {
                            Logger.log("It is not null");
                            foreach (Pack pack in packLocation.packs)
                            {
                                Logger.log("Pack " + pack.id + " in node " + packLocation);
                            }
                        }*/
                        //packLocation.packs.Remove(tempPack);
                        //Logger.log("Killed the pack' commented remove");
                    }
                    KillPoint++;
                }
            }
            else
            { //player attacks every monster in the pack
                int packCount = foe_.pack.members.Count;
                foe_.pack.members.RemoveAll(target => target.HP <= 0); //already dead monsters?
                KillPoint += (uint) (packCount - foe_.pack.members.Count) ;
				// Added the following
				for (int i = packCount - 1; i >= 0; i--){ //for each monster in the pack
					Monster target = foe_.pack.members.ElementAt(i);

					target.HP = (int)Math.Max(0, target.HP - AttackRating); //player attacks to the monster
					String killMsg = target.HP == 0 ? ", KILLING it" : "";
					Logger.log("Creature " + id + " attacks " + target.id + killMsg + ".");
					//base.Attack(target);
					if(target.HP==0){ //if the monster dies
						foe_.pack.members.Remove(target);//remove it from the list
						KillPoint++;

					}
				}
                
                accelerated = false; //player not accelerated anymore
            }


            
        }

        /**
         * Attack method that calls the original attack method
         * and returns true if all monsters in the pack die
         */

		public bool AttackBool(Creature foe){
			Attack(foe); //original attack method

			if (!(foe is Monster)) throw new ArgumentException();
            Monster foe_ = foe as Monster;

			if (foe_.pack.members.Count == 0) //if all monsters in the pack died 
            { //delete this pack from player's node
				return true; //pack is beated
			}else{
				return false; //pack is still alive
			}
		}
        /*ADDED
        public int getAction() {
            return 1; // A  test in which 1 means attack
        }*/
    }
}
