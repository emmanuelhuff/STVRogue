using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using STVRogue.Utils;

namespace STVRogue.GameLogic
{
	public abstract class Creature
	{
		public String id;
		public String name;
		public int HP;
		public uint AttackRating = 1;
		public Node location;
		public Creature() { }
		public abstract void Attack(Creature foe);

	}

    public class Monster : Creature
    {
        public Pack pack;

        /* Create a monster with a random HP */
        public Monster(String id)
        {
            this.id = id; name = "Orc";
            HP = 1 + RandomGenerator.rnd.Next(6);
        }

		public override void Attack(Creature foe)
        {
            foe.HP = (int)Math.Max(0, foe.HP - AttackRating);
            //Logger.log("Creature's HP is " + foe.HP);
            String killMsg = foe.HP == 0 ? ", KILLING it" : "";
            Logger.log("Creature " + id + " attacks " + foe.id + killMsg + ".");
        }

        //ADDED
		public void setHP(int newHP){
			this.HP = newHP;
		}
    }

    public class Player : Creature
    {
        public Dungeon dungeon;
        public int HPbase = 100;
        public Boolean accelerated = false;
        public uint KillPoint = 0;
        public List<Item> bag = new List<Item>();
        public Player()
        {
            id = "player";
            AttackRating = 5;
			HP = HPbase;
        }
        //ADDED
		public bool containsMagicCrystal(){
			foreach(Item i in bag){
				if (i.GetType() == typeof(Crystal))
					return true;
			}
			return false;
		}
        //ADDED
		public bool containsHealingPotion()
        {
            foreach (Item i in bag)
            {
				if (i.GetType() == typeof(HealingPotion))
                    return true;
            }
            return false;
        }

        public void use(Item item)
        {
            if (!bag.Contains(item) || item.used) throw new ArgumentException();
            item.use(this);
            bag.Remove(item);
        }

		public int getHPValueOfBag(){
			int bagHPValue = 0;
			foreach(Item i in this.bag){
				if(i.GetType()== typeof(HealingPotion)){
					bagHPValue += (int)((HealingPotion)i).HPvalue;
				}
			}
			return bagHPValue;
				
		}

		public Command getNextCommand(){
            string userInput = Console.ReadLine();
            int command = int.Parse(userInput);
            if (command != 1 && command != 2 && command != 3 && command != 4)
            {
				Logger.log("Unknown command");
				command = -1;
            }
			Command userCommand = new Command(command); //key press numbers for known commands, -1 for unknown commands
            return userCommand;
		}

        /*ADDED*/
		public Boolean flee()
        {
			
			Node currentLocation = this.location;
			List<Node> adjacentNodes = currentLocation.neighbors;
			foreach(Node adjNode in adjacentNodes){            
					//check if it will be contested, if then do not flee to that node
					if (adjNode.packs.Count == 0)
					{
						//change location and flee
						this.location = adjNode;
						Logger.log("Player fleed from " + currentLocation + " to " + this.location);
						this.collectItems();
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

		/*ADDED*/
        public void move()
        {         
            Node currentLocation = this.location;
            List<Node> adjacentNodes = currentLocation.neighbors;
			int nodeIndex = RandomGenerator.rnd.Next(0, adjacentNodes.Count);
			this.location = adjacentNodes.ElementAt(nodeIndex);
			Logger.log("Player moved from "+currentLocation.id+" to " + this.location.id);
			//Collect items in this location
			this.collectItems();
        }

		public void collectItems(){
			Node currentLocation = this.location;
			foreach(Item i in currentLocation.items.ToList()){
				currentLocation.items.Remove(i);
				this.bag.Add(i);
				Logger.log("Collected item " + i.id);
			}
		}

        public override void Attack(Creature foe)
        {
            if (!(foe is Monster)) throw new ArgumentException();
            Monster foe_ = foe as Monster;

            Pack tempPack = foe_.pack;

            Node packLocation = tempPack.location;

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
            if (!accelerated)
            {
				foe.HP = (int)Math.Max(0, foe.HP - AttackRating);
                String killMsg = foe.HP == 0 ? ", KILLING it" : "";
                Logger.log("Creature " + id + " attacks " + foe.id + killMsg + ".");
                //base.Attack(foe);
                if (foe.HP == 0)
                {
                    
                    tempPack.members.Remove(foe_);
                    Logger.log("All monsters in the pack: ");
                    foreach (Monster m in tempPack.members)
                    {
                        Logger.log(m.id);
                    }
                    Logger.log("Player attacked" + foe.id);
                    
                    if(tempPack.members.Count == 0)
                    {
                        Logger.log("Pack is now empty' pack id "+tempPack.id);
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
                        }
                        //packLocation.packs.Remove(tempPack);
                        Logger.log("Killed the pack' commented remove");
                    }
                    //foe_.pack.members.Remove(foe_);
                    KillPoint++;
                }
            }
            else
            {
                int packCount = foe_.pack.members.Count;
                foe_.pack.members.RemoveAll(target => target.HP <= 0);
                KillPoint += (uint) (packCount - foe_.pack.members.Count) ;
				// ADDED IMPLEMENTATION
				for (int i = packCount - 1; i >= 0; i--){
					Monster target = foe_.pack.members.ElementAt(i);

					target.HP = (int)Math.Max(0, target.HP - AttackRating);
					String killMsg = target.HP == 0 ? ", KILLING it" : "";
					Logger.log("Creature " + id + " attacks " + target.id + killMsg + ".");
					//base.Attack(target);
					if(target.HP==0){
						foe_.pack.members.Remove(target);
						KillPoint++;

					}
				}
                
                accelerated = false;
            }


            
        }
		public bool AttackBool(Creature foe){
			Attack(foe);
			Logger.log("here101");
			if (!(foe is Monster)) throw new ArgumentException();
            Monster foe_ = foe as Monster;
			Logger.log("here102");
			if (foe_.pack.members.Count == 0)
            { //delete this pack from player's node
				return true; //pack is beated
			}else{
				return false;
			}
		}
        /*ADDED
        public int getAction() {
            return 1; // A  test in which 1 means attack
        }*/
    }
}
