using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using STVRogue.Utils;

namespace STVRogue.GameLogic
{
	public class Pack
	{
		public String id; //id of the pack
		public List<Monster> members; //pack members
		public int startingHP = 0; //total of starting hps of all members
		public Node location; //pack location
		public Dungeon dungeon; //game dungeon
        
        /**
         * Creates a pack 
         * @param name="id" id of the pack
         * @param name="n" number of monsters in the pack
         * @param name="dungeon" dungeon of the game
        */
		public Pack(String id, uint n, Dungeon dungeon)
		{
			this.members = new List<Monster>(); 
			this.dungeon = dungeon; //get the dungeon
			this.id = id;
			for (int i = 0; i < n; i++)
			{
				Monster m = new Monster("" + id + "_" + i); //create a new member
				this.members.Add(m); //add the monster
				startingHP += m.HP; //pack starting hp is sum of starting hps of member monsters
				m.pack = this; //monster also knows its pack
			}
		}
		/**
		 * Move the pack to an adjacent node. 
		 * @param name="u" node should be adjacent
        */
		public void Move(Node u)
		{

			if (!location.neighbors.Contains(u)) throw new ArgumentException();
			int capacity = (int)(dungeon.M * (dungeon.level(u) + 1)); //each zone has a specified capacity
			// count monsters already in the node:
			foreach (Pack Q in location.packs) //for each pack in the node
			{
				capacity = capacity - Q.members.Count; //decrease the capacity by number of pack members
			}
			// capacity now expresses how much space the node has left
			if (members.Count > capacity)
			{
				Logger.log("Pack " + id + " is trying to move to a full node " + u.id + ", but this would cause the node to exceed its capacity. Rejected.");
				return;
			}
            //else move is successful
			this.location = u;
			u.packs.Add(this);
			Logger.log("Pack " + id + " moves to the node " + u.id + ". Not rejected.");
		}

        /**
         * All pack members attack player
         */

		public void Attack(Player p)
		{
			foreach (Monster m in members)
			{
				m.Attack(p);
				if (p.HP == 0) break;
			}
		}


		/* Move the pack one node further along a shortest path to u. */
		public void MoveTowards(Node u)
		{

			List<Node> path = this.dungeon.shortestpath(this.location, u); //take the shortest path between them
			Logger.log("will move to " + path[0].id);
			this.Move(path[0]);
			Logger.log("Moved to " + this.location.id);
		}
        
		/**
		 * Pack either flees or attacks, returns the action with specified possibility
		 */

		public int getAction()
		{
			int H = this.getPackHPValue(); //pack's current hp value
			int orgH = this.startingHP; //pack's starting hp 

			if (RandomGenerator.rnd.Next(101) < (((1 - (H / orgH)) / 2) * 100))
			{ //specified proportion
				return 2; //it flees
			}
			else
			{
				return 1; //it attacks 
			}
		}

        /**
         * Returns Pack's hp value
         */

		public int getPackHPValue()
		{
			int packHpValue = 0;
			foreach (Monster m in this.members) //for each member
			{
				packHpValue += m.HP;//adds each member's hp
			}
			return packHpValue;
		}


		/**
		 * Pack flees to an adjacent node with enough capacity
		 */

		public bool flee()
		{
			/*is there an adjacent node? if so, remove pack, add to other node. To do so, Node class neighbors that is not a bridge*/
			/* Pack.location is the node*/
			// Fleeing is only possible if there is an adjacent node, with enough capacity left,
			// to accommodate O.O is then moved into this node.
			/*is there an adjacent node? if so, remove pack, add to other node. To do so, Node class neighbors that is not a bridge*/
			/* Pack.location is the node*/
			Node currentLocation = this.location; //get the current location of the pack
			int currentLevel = currentLocation.level; //get the current level(needed for capacity)
			List<Node> adjacentNodes = currentLocation.neighbors; //get the adjacent nodes
			int zoneLevel, currentMonsters = 0;
			uint adjNodeCapacity;
			foreach (Node adjNode in adjacentNodes) //it can only flee to an adjacent node
			{
				zoneLevel = adjNode.level; //get the node's level
				adjNodeCapacity = dungeon.zones.ElementAt<Zone>(zoneLevel - 1).capacity; //get node's capacity
				foreach (Pack p in adjNode.packs) //calculate how many monsters are already at this node
				{
					currentMonsters += p.members.Count; //current monster number at the node
				}
				if (adjNodeCapacity - currentMonsters >= this.members.Count) //if capacity is enough it can flee
				{
					currentLocation.packs.Remove(this);//remove this pack from its current location
					adjNode.packs.Add(this); //Add it to the new location
					this.location = adjNode; //change pack's location to the new node
					return true;

				}
            

			}
			return false;

		}

		/*OLD FLEE, are we sure that it cant flee to next level?
        public bool flee()
        {
            //is there an adjacent node? if so, remove pack, add to other node. To do so, Node class neighbors that is not a bridge/
            // Pack.location is the node/
            // Fleeing is only possible if there is an adjacent node, with enough capacity left,
            // to accommodate O.O is then moved into this node.
            //is there an adjacent node? if so, remove pack, add to other node. To do so, Node class neighbors that is not a bridge/
            // Pack.location is the node
            Node currentLocation = this.location;
            int currentLevel = currentLocation.level;
            List<Node> adjacentNodes = currentLocation.neighbors;
            int zoneLevel, currentMonsters = 0;
            uint adjNodeCapacity;
            foreach (Node adjNode in adjacentNodes)
            {
                zoneLevel = adjNode.level;
                if (zoneLevel == currentLevel)
                {
                    adjNodeCapacity = dungeon.zones.ElementAt<Zone>(zoneLevel - 1).capacity;
                    foreach (Pack p in adjNode.packs)
                    {
                        currentMonsters += p.members.Count;
                    }
                    if (adjNodeCapacity - currentMonsters >= this.members.Count)
                    {
                        //it can flee to that node
                        currentLocation.packs.Remove(this);//remove this pack from its current location
                        adjNode.packs.Add(this); //Add it to the new location
                        this.location = adjNode; //change pack's location to the new node
                        return true;

                    }
                }
                else
                {
                    //it can not go to that node
                }


            }
            return false;
            // For each node in adjacentNodes, get the level() and the capacity of the node (from its zone)
            // Get number of monsters in that node
            // if there is enough capacity, move to that node
            // if that node is found return true
            // if no node is found return false
        }*/
	}
}
