using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using STVRogue.Utils;

namespace STVRogue.GameLogic
{
    public class Dungeon
    {
		public Predicates predicates;
        public Node startNode;
        public Node exitNode;
        public uint difficultyLevel;
        /* a constant multiplier that determines the maximum number of monster-packs per node: */
        public uint M;

        /* To create a new dungeon with the specified difficult level and capacity multiplier */
        public Dungeon(uint level, uint nodeCapacityMultiplier)
        {
            Logger.log("Creating a dungeon of difficulty level " + level + ", node capacity multiplier " + nodeCapacityMultiplier + ".");
            difficultyLevel = level;
            M = nodeCapacityMultiplier;
			int numberOfNodesInZone = 0;
			STVRogue.Utils.RandomGenerator.initializeWithSeed(5); //predictably randomized
			List<Node> nodesInZone = new List<Node>();
			string preId = "";
			string lastId = "";
			string nodeId = "";
			for (int i = 1; i < level+1; i++){ //i signifies zone level
				preId = ""+i; //every node is named with its zone number followed by its number in a zone (zoneId nodeId)
				numberOfNodesInZone = STVRogue.Utils.RandomGenerator.rnd.Next(2,7) ; //randomly decide between 2-6 nodes 
				Logger.log("number of nodes in zone "+i+" is " + numberOfNodesInZone);

				for (int j = 0; j < numberOfNodesInZone; j++){ //create and add nodes to the list nodesInZone
					lastId = "" + j;
					nodeId = preId + lastId;
					Node newNode = new Node(nodeId);
					nodesInZone.Add(newNode);
				}
                //nodesInZone stores every node in this zone
				if(i == 1){ // connect start node to some nodes in the zone
					
				}else{ //connect bridge to some nodes in the zone
					
				}


			}

            throw new NotImplementedException();
        }

        /* Return a shortest path between node u and node v */
        public  List<Node> shortestpath(Node u, Node v) { 
            List<Node> queue = new List<Node>(); //list of nodes to visit
            Dictionary<string,uint> nodeDist = new Dictionary<string,uint>(); //node id's and their distances 
            //nodeDist dictionary contains all reachable nodes from node u
			List<Node> reachableNodesFromU = predicates.reachableNodes(u);
            //make their distances int max
            foreach(Node nd in reachableNodesFromU){
                nodeDist.Add(nd.id,Int32.MaxValue); //include math to use INT_MAX?, using System? for Int32.MaxValue
                nd.visited=false;
                nd.pred=null;
            } 
            //source node u is the first to be visited, change distance to 0, make it visited, add it to queue
            u.visited=true;
            nodeDist.Add(u.id,0);
            queue.Add(u);
            uint tempDistance=0;

            while(queue.Count != 0){ //while queue is not empty
                Node nd = queue.First();
                queue.RemoveAt(0); //delete queue's first element
                tempDistance = nodeDist["nd"]; //get the distance value

                /**for each neighbour of the nd if the neighbour node is not visited, 
                make it visited
                make its distance = distance of nd+1
                make its predecessor = nd
                add it into queue to explore

                if the node is node v, stop BFS 
                */
				foreach (Node tempNode in nd.neighbors){
                    if(!tempNode.visited){
                        tempNode.visited=true;
                        nodeDist.Add(tempNode.id,tempDistance+1);
                        tempNode.pred = nd;
                        queue.Add(tempNode);
                    }
                    if(tempNode == v) break; //not sure if 'tempNode == v' is a valid compare
                }
                
            }
            //now each reachable nodes have predecessor node information
            //can find the shortest path starting from Node v
            List<Node> path = new List<Node>(); //stores the path
            //path push back Node v
            Node current = v;// temp node

            path.Add(current); // *****DOES IT REALLY WORK like push_back?

            while(current.pred!=null){
                path.Add(current.pred); //path push back current.pred
                current = current.pred; //current = current.pred;
            }
			path.Reverse(); //****PROBABLY it should return reverse
			return path; 
            //return path.Reverse();
            //now the list path has the shortest path from v to u
            //return it in reverse order
        }


        /** ADDED */
        /* To disconnect a bridge from the rest of the zone the bridge is in. */
        public  void disconnect(Bridge b)
        {
            Logger.log("Disconnecting the bridge " + b.id + " from its zone.");
            
			b.disconnectFromSameZone();  /** ADDED */
			startNode = b;
            //throw new NotImplementedException();
        }

        /** ADDED */
        /* To calculate the level of the given node. */
        public  uint level(Node d) { 
            //get shortest path from starting node to node d
            //check list of nodes, increment the level with the number of bridge nodes
            uint nodeLevel = 0;
            List<Node> pathFromStartNode = shortestpath(startNode,d);
            foreach(Node nd in pathFromStartNode)
            {
				if(predicates.isBridge(startNode,exitNode,nd))   nodeLevel++;
            }
            return nodeLevel;
            //throw new NotImplementedException(); 
        }

    }

    public class Node
    {
        public String id;
        public List<Node> neighbors = new List<Node>();
        public List<Pack> packs = new List<Pack>();
        public List<Item> items = new List<Item>();
        public bool visited;
        public Node pred;

        public Node() { }
        public Node(String id) { 
            this.id = id; 
            this.visited=false;
            this.pred = null;
        }

        /* To connect this node to another node. */
        public void connect(Node nd)
        {
            neighbors.Add(nd); nd.neighbors.Add(this);
        }

        /* To disconnect this node from the given node. */
        public void disconnect(Node nd)
        {
            neighbors.Remove(nd); nd.neighbors.Remove(this);
        }

        /* Execute a fight between the player and the packs in this node.
         * Such a fight can take multiple rounds as describe in the Project Document.
         * A fight terminates when either the node has no more monster-pack, or when
         * the player's HP is reduced to 0. 
         */
        public void fight(Player player)
        {
            // while there exists a pack and player's HP is higher than 0
            while(this.packs.Count != 0 && player.HP!=0 ){
                //to be implemented
            }
            throw new NotImplementedException();
        }
    }

    public class Bridge : Node
    {
        List<Node> fromNodes = new List<Node>();
        List<Node> toNodes = new List<Node>();
        public Bridge(String id) : base(id) { }

        /* Use this to connect the bridge to a node from the same zone. */
        public void connectToNodeOfSameZone(Node nd)
        {
            base.connect(nd);
            fromNodes.Add(nd);
        }

        /* Use this to connect the bridge to a node from the next zone. */
        public void connectToNodeOfNextZone(Node nd)
        {
            base.connect(nd);
            toNodes.Add(nd);
        }

        /** ADDED */
        public void disconnectFromSameZone(){
            //for each node in fromNodes list, remove the connection between the bridge (call base.disconnect(Node n))
            foreach (Node nd in this.fromNodes)
            {
                base.disconnect(nd);
            }

        }
    }
}
