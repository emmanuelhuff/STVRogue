using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using STVRogue.Utils;

namespace STVRogue.GameLogic
{
    public class Creature
    {
        public String id;
        public String name;
        public int HP;
        public uint AttackRating = 1;
        public Node location;
        public Creature() { }
        virtual public void Attack(Creature foe)
        {
            foe.HP = (int)Math.Max(0, foe.HP - AttackRating);
			//Logger.log("Creature's HP is " + foe.HP);
            String killMsg = foe.HP == 0 ? ", KILLING it" : "";
            Logger.log("Creature " + id + " attacks " + foe.id + killMsg + ".");
        }
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
			int command = Console.Read();
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
			int currentLevel = currentLocation.level;
			List<Node> adjacentNodes = currentLocation.neighbors;
			int zoneLevel;
			foreach(Node adjNode in adjacentNodes){
				zoneLevel = adjNode.level;
				if(currentLevel == zoneLevel){
					//check if it is contested, if it is not flee to that node
					if(!adjNode.contested(this)){
						//change location and flee
						this.location = adjNode;
						Logger.log("Player fleed from "+currentLocation+" to " + this.location);
						this.collectItems();
						return true;
					}
				} //else do nothing, it can not flee to a node from the different zone
			}
			return false;

        }

		/*ADDED*/
        public void move()
        {         
            Node currentLocation = this.location;
            List<Node> adjacentNodes = currentLocation.neighbors;
			int nodeIndex = RandomGenerator.rnd.Next(0, adjacentNodes.Count);
			this.location = adjacentNodes.ElementAt(nodeIndex);
			Logger.log("Player moved from "+currentLocation.id+" to " + this.location);
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

        override public void Attack(Creature foe)
        {
            if (!(foe is Monster)) throw new ArgumentException();
            Monster foe_ = foe as Monster;
            if (!accelerated)
            {
                base.Attack(foe);
                if (foe_.HP == 0)
                {
                    foe_.pack.members.Remove(foe_);
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
					base.Attack(target);
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
			if (!(foe is Monster)) throw new ArgumentException();
            Monster foe_ = foe as Monster;
			if (foe_.pack.members.Count == 0)
            { //delete this pack from player's node
				return true; //pack is beated
			}else{
				return false;
			}
		}
        /*ADDED*/ 
        public int getAction() {
            return 1; /* A  test in which 1 means attack*/
        }
    }
}
