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
            public String id;
            public List<Monster> members;
            public int startingHP = 0;
            public Node location;
            public Dungeon dungeon;

		public Pack(String id, uint n,Dungeon dungeon)
            {
                this.members = new List<Monster>();
			    this.dungeon = dungeon;
                this.id = id;
                for (int i = 0; i < n; i++)
                {
                    Monster m = new Monster("" + id + "_" + i);
                    this.members.Add(m);
                    startingHP += m.HP;
                    m.pack = this;
                }
            }

            public void Attack(Player p)
            {
                foreach (Monster m in members)
                {
                    m.Attack(p);
                    if (p.HP == 0) break;
                }
            }



            /* Move the pack to an adjacent node. */
            public void move(Node u)
            {
			System.Diagnostics.Debug.WriteLine("here");
                if (!location.neighbors.Contains(u)) throw new ArgumentException();
    			int capacity = (int) (dungeon.M * (dungeon.level(u) + 1));
                // count monsters already in the node:
                foreach (Pack Q in location.packs) {
                    capacity = capacity - Q.members.Count;
                }
			System.Diagnostics.Debug.WriteLine("here");
                // capacity now expresses how much space the node has left
                if (members.Count > capacity)
                {
                    Logger.log("Pack " + id + " is trying to move to a full node " + u.id + ", but this would cause the node to exceed its capacity. Rejected.");
                    return;
                }
                this.location = u;
			System.Diagnostics.Debug.WriteLine("here");
                u.packs.Add(this);
                Logger.log("Pack " + id + " moves to the node " + u.id + ". Not rejected.");

            }

            /* Move the pack one node further along a shortest path to u. */
            public void moveTowards(Node u)
            {
			
			List<Node> path = new List<Node>();
			path = this.dungeon.shortestpath(this.location, u);
			Logger.log("will move to " + path[0].id);
			this.move((Node)path[0]);
			Logger.log("Moved to " + this.location.id);
            }
            
            /*ADDED, USED?
            public Monster getMonster() {
                foreach (Monster m in members)
                {
                    if (m.HP > 0){
                        return m;
                    }
                }
                throw new ArgumentException(); //*this is when no monsters are alive
            }*/

            /*ADDED*/ 
            public int getAction() {
			int H = this.getPackHPValue();
    		int orgH = this.startingHP;
  
			if (RandomGenerator.rnd.Next(101) < (((1- (H/orgH))/2)*100)){
                    return 2; //it flees
                } else {
                    return 1; //it attacks /* A  test in which 1 means attack, 2 means flee*/
                }
            }
            
		public int getPackHPValue(){
			int packHpValue = 0;
			foreach(Monster m in this.members){
				packHpValue += m.HP;
			}
			return packHpValue;
		}
	
        
            /*ADDED*/ 
    		public bool flee() {
                /*is there an adjacent node? if so, remove pack, add to other node. To do so, Node class neighbors that is not a bridge*/
                /* Pack.location is the node*/            
    			// Fleeing is only possible if there is an adjacent node, with enough capacity left,
                // to accommodate O.O is then moved into this node.
                /*is there an adjacent node? if so, remove pack, add to other node. To do so, Node class neighbors that is not a bridge*/
                /* Pack.location is the node*/
                Node currentLocation = this.location;
    			int currentLevel = currentLocation.level;
                List<Node> adjacentNodes = currentLocation.neighbors;
                int zoneLevel, currentMonsters=0;
                uint adjNodeCapacity;
    			foreach (Node adjNode in adjacentNodes)
                {
                    zoneLevel = adjNode.level;
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
    			return false;
                // For each node in adjacentNodes, get the level() and the capacity of the node (from its zone)
                // Get number of monsters in that node
                // if there is enough capacity, move to that node
                // if that node is found return true
                // if no node is found return false
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
