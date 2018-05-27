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
        public List<Bridge> bridges; //stores every bridge instance to access previously implemented ones in for loop
                                     /* a constant multiplier that determines the maximum number of monster-packs per node: */
        public List<Zone> zones;
        public uint M;

        /* To create a new dungeon with the specified difficult level and capacity multiplier */
        //TO-DO: check average connectivity predicate
        //TO-DO: improve the algorithm for building connections between nodes in the same zone
		public Dungeon(uint level, uint nodeCapacityMultiplier, Random random)
        {
            Logger.log("Creating a dungeon of difficulty level " + level + ", node capacity multiplier " + nodeCapacityMultiplier + ".");
            difficultyLevel = level; //assign dungeon difficulty level
            bridges = new List<Bridge>(); //List of bridges initialized
            predicates = new Predicates();
            zones = new List<Zone>(); //initialize the zones
            M = nodeCapacityMultiplier; //initialize node capacity multiğlier
            int numberOfNodesInZone = 0;
            int connected = 0;

            //every node is named with its zone number followed by its number in a zone (nodeId = preId+lastId)
            string preId, lastId, nodeId = "";
            startNode = new Node("" + 10, 1); //start node id is set
            Logger.log("Set startNode Id: 10");
            for (int i = 1; i < level + 1; i++) //i signifies zone level, for each zone
            {
                Zone newZone = new Zone(i, nodeCapacityMultiplier); //create a new zone
                zones.Add(newZone); //add it in dungeon.zones list
                Logger.log("Creating level " + i);
                
                preId = "" + i; // preId is zone level
				numberOfNodesInZone = random.Next(2, 5); //randomly decide between 2-4 nodes in a zone
                                                                      //if you change number of nodes can be in a zone, be careful with the dependent statements below
                Logger.log("Number of nodes in zone " + i + " is " + numberOfNodesInZone);

                for (int j = 1; j <= numberOfNodesInZone; j++) //for each node in a zone
                { //create and add nodes to the list nodesInZone
                    lastId = "" + j; 
                    nodeId = preId + lastId; //merge preId and lastId to create nodeId
                    Node newNode = new Node(nodeId, i); //create node
                    Logger.log("Created node with id " + nodeId);
                    newZone.nodesInZone.Add(newNode); //add node to the list
                }

                //zone's nodesInZone list stores every node in this zone
                int numberOfNodesToConnect; // temp variable to decide how many nodes to connect for startNode or for bridges
                Node zoneFirstNode; //holds start node or starting bridge for the zone 
                                    //(starting bridge for a zone is the bridge which is connecting it to the previous zone)

                if (i == 1) //for the first level
                { // connect start node to some nodes in the zone
                    zoneFirstNode = startNode;
					numberOfNodesToConnect = random.Next(1, numberOfNodesInZone + 1); //randomly decide btw 1-4 nodes
                                                                                                   //rnd operation is exclusive for the max number, numberofNodesInZone can be 4 at most, thus it is safe this way
                    Logger.log("Connecting startNode to " + numberOfNodesToConnect + " nodes in the zone ");
                    for (int j = 0; j < numberOfNodesToConnect; j++) //for each nodes to connect
                    { //connect them with the start node
						int nodeIndex = random.Next(0, numberOfNodesInZone); //randomly get node index to connect
                        while (startNode.alreadyConnected(newZone.nodesInZone.ElementAt(nodeIndex)))
                        { //if the chosen node is already connected
							nodeIndex = random.Next(0, numberOfNodesInZone); //choose a new one
                        }
                        startNode.connect(newZone.nodesInZone.ElementAt(nodeIndex)); //connect start node with that node
                        Logger.log("Connected to node " + newZone.nodesInZone.ElementAt(j).id);
                    }
                }
                else
                { //connect bridge to some nodes in the next zone


                    Bridge startBridge = bridges.ElementAt(i - 2); //bridge is already created in previous loop
                    zoneFirstNode = (Node)startBridge;
                    int maxConnect = 4 - startBridge.neighbors.Count; //maximum number of connections that bridge can make

                    if (numberOfNodesInZone < maxConnect) //maximum number of connections are constrained by
                        maxConnect = numberOfNodesInZone;   //number of zones in the zone
                    numberOfNodesToConnect = random.Next(1, maxConnect + 1); //Decide how many connections it should make
                    Logger.log("Connecting bridge " + startBridge.id + " to " + numberOfNodesToConnect + " nodes in the next zone ");
                    for (int j = 0; j < numberOfNodesToConnect; j++)
                    { //connect them with the bridge node
                        int nodeIndex = random.Next(0, numberOfNodesInZone); //randomly decide the node index
                        while (startBridge.alreadyConnected(newZone.nodesInZone.ElementAt(nodeIndex)))
                        {
							nodeIndex = random.Next(0, numberOfNodesInZone);
                        }
                        startBridge.connectToNodeOfNextZone(newZone.nodesInZone.ElementAt(nodeIndex)); //connect bridge with the next zone
                        Logger.log("Connected to node " + newZone.nodesInZone.ElementAt(j).id);
                    }




                }
                Logger.log("Connecting nodes in the same zone");

                //connect nodes in the same zone
                //TO-DO improve the algorithm
                //Currently it exits the algorithm once every node is accessible from the starting node of the zone            
				while (!allReachable(newZone.nodesInZone, zoneFirstNode)) //while there exists not reachable nodes 
                {

					Node n1 = newZone.nodesInZone.ElementAt(random.Next(0, numberOfNodesInZone)); //randomly choose node1
                    while (n1.isFullyConnected()) //if it is fully connected node
                    {
						n1 = newZone.nodesInZone.ElementAt(random.Next(0, numberOfNodesInZone)); //choose another one
                    }
					Node n2 = newZone.nodesInZone.ElementAt(random.Next(0, numberOfNodesInZone)); //randomly select node2
                    while (n2.isFullyConnected() || n2.id == n1.id || n2.alreadyConnected(n1)) //make sure it is not fully connected, not same as node1, not already connected with node1
                    {
						n2 = newZone.nodesInZone.ElementAt(random.Next(0, numberOfNodesInZone));
                    }
                    n1.connect(n2);
                    Logger.log("Nodes " + n1.id + " " + n2.id + " are connected");
                }


                List<Node> listOfNotFullNodes = new List<Node>(); //get list of not fully connected nodes in the zone to consider connections

                for (int j = 0; j < newZone.nodesInZone.Count; j++)
                {
                    Node nd = newZone.nodesInZone.ElementAt(j);
                    if (!nd.isFullyConnected()) listOfNotFullNodes.Add(nd);
                }
                Logger.log("Creating list of not full nodes, number of not full nodes " + listOfNotFullNodes.Count);

                //Connect last node to the zone, either a bridge or the exit node
                int min = 1;
                int max;
                if (i == level)
                { // last zone
                  //connect exit node

                    lastId = "" + (numberOfNodesInZone + 1);
                    nodeId = preId + lastId; //create id of the exit node
                    exitNode = new Node(nodeId, i); //create exit node
                    Logger.log("Last zone is finished, exit node id is set to " + nodeId);
                    max = 4; //max number of connections that node can have

                    if (listOfNotFullNodes.Count < 4) max = listOfNotFullNodes.Count; //can make at most listOfNotFullNodes.Count number of connections
					numberOfNodesToConnect = random.Next(min, max + 1); //randomly decide number of nodes to connect

                    for (int j = 0; j < numberOfNodesToConnect; j++) //connect exit node to that number of nodes
                    {
                        exitNode.connect(listOfNotFullNodes.ElementAt(j)); //can be randomized
                        Logger.log("Connected to node " + listOfNotFullNodes.ElementAt(j).id);
                    }

                }
                else
                { //connect to end bridge
                  //a bridge can be connected to at minimum 1 and at maximum 3 nodes
                    lastId = "" + (numberOfNodesInZone + 1);
                    nodeId = preId + lastId; //create bridge id
                    Bridge endBridge = new Bridge(nodeId, i); //create the bridge
                    bridges.Add(endBridge); //add it to bridges list to access it in the next loop iteration
                    Logger.log("A new bridge is created with id " + nodeId);

                    max = 3; //since a bridge should already have at least 1 connection to the other zone
                    //max number of connections it can make can not be more than 3

                    if (listOfNotFullNodes.Count < 3) max = listOfNotFullNodes.Count;  //can make at most listOfNotFullNodes.Count number of connections
					numberOfNodesToConnect = random.Next(min, max + 1); //decide number of nodes to connect

                    for (int j = 0; j < numberOfNodesToConnect; j++) //connect it to that number of nodes
                    {
                        endBridge.connectToNodeOfSameZone(listOfNotFullNodes.ElementAt(j)); //not randomized
                        Logger.log("Connected to node " + listOfNotFullNodes.ElementAt(j).id);
                    }
                    //end bridge is also connected

                }

                //Ensure the average connectivity
                if (i == 1)
                {
                    Node n1, n2;
                    connected = startNode.neighbors.Count;
                    for (int j = 0; j < numberOfNodesInZone; j++)
                    {
                        connected += newZone.nodesInZone.ElementAt(j).neighbors.Count;
                    }
                    while (Convert.ToDouble(connected / (numberOfNodesInZone + 1)) > 3)
                    {
						n1 = newZone.nodesInZone.ElementAt(random.Next(0, numberOfNodesInZone));
                        while (n1.neighbors.Count <= 1)
                        {
							n1 = newZone.nodesInZone.ElementAt(random.Next(0, numberOfNodesInZone));
                        }
						n2 = newZone.nodesInZone.ElementAt(random.Next(0, numberOfNodesInZone));
                        while (n2.neighbors.Count <= 1 || n2.id == n1.id || !n2.alreadyConnected(n1))
                        {
							n2 = newZone.nodesInZone.ElementAt(random.Next(0, numberOfNodesInZone));
                        }
                        n1.disconnect(n2);
                        Logger.log("Nodes " + n1.id + " " + n2.id + " are disconnected");
                        connected -= 2;
                    }
                }
                else if (i == level)
                {
                    Node n1, n2;
                    connected = exitNode.neighbors.Count;
                    connected += bridges.ElementAt(i - 2).neighbors.Count;
                    for (int j = 0; j < numberOfNodesInZone; j++)
                    {
                        connected += newZone.nodesInZone.ElementAt(j).neighbors.Count;
                    }
                    while (Convert.ToDouble(connected / (numberOfNodesInZone + 2)) > 3)
                    {
						n1 = newZone.nodesInZone.ElementAt(random.Next(0, numberOfNodesInZone));
                        while (n1.neighbors.Count <= 1)
                        {
							n1 = newZone.nodesInZone.ElementAt(random.Next(0, numberOfNodesInZone));
                        }
						n2 = newZone.nodesInZone.ElementAt(random.Next(0, numberOfNodesInZone));
                        while (n2.neighbors.Count <= 1 || n2.id == n1.id || !n2.alreadyConnected(n1))
                        {
							n2 = newZone.nodesInZone.ElementAt(random.Next(0, numberOfNodesInZone));
                        }
                        n1.disconnect(n2);
                        Logger.log("Nodes " + n1.id + " " + n2.id + " are disconnected");
                        connected -= 2;
                    }
                }
                else
                {
                    Node n1, n2;
                    connected = bridges.ElementAt(i - 2).neighbors.Count;
                    for (int j = 0; j < numberOfNodesInZone; j++)
                    {
                        connected += newZone.nodesInZone.ElementAt(j).neighbors.Count;
                    }
                    while (Convert.ToDouble(connected / (numberOfNodesInZone + 1)) > 3)
                    {
						n1 = newZone.nodesInZone.ElementAt(random.Next(0, numberOfNodesInZone));
                        while (n1.neighbors.Count <= 1)
                        {
							n1 = newZone.nodesInZone.ElementAt(random.Next(0, numberOfNodesInZone));
                        }
						n2 = newZone.nodesInZone.ElementAt(random.Next(0, numberOfNodesInZone));
                        while (n2.neighbors.Count <= 1 || n2.id == n1.id || !n2.alreadyConnected(n1))
                        {
							n2 = newZone.nodesInZone.ElementAt(random.Next(0, numberOfNodesInZone));
                        }
                        n1.disconnect(n2);
                        Logger.log("Nodes " + n1.id + " " + n2.id + " are disconnected");
                        connected -= 2;
                    }
                }


                //pass to next zone
                Logger.log("Passing to next zone");

            }

           
            //Add startnode, bridges and endnode
			foreach(Zone z in zones){
				if(z.id==1){ //first zone, add start node
					z.nodesInZone.Add(startNode);
				}else if(z.id == level){ //last zone, add end node
					z.nodesInZone.Add(exitNode);
				}

					if (z.id != level) //in middle zones, add their bridges
					{
						z.nodesInZone.Add(bridges.ElementAt(z.id - 1));
					}
				

			}


        }
        /**
         * Returns true if all nodes in toReachNodes are reachable from
         * the parameter mainNode
         */

        public bool allReachable(List<Node> toReachNodes, Node mainNode)
        {
            for (int i = 0; i < toReachNodes.Count; i++) //for each node in toReachNodes
            {
                if (!predicates.isReachable(toReachNodes.ElementAt(i), mainNode)) return false; //return false if not reachable
            }
            return true;
        }
        /* NOT USED? */
        //return average connectivity for Dungeon
		/*public float checkAvg(List<Node> allNodes)
        {
            float avg = 0;
            int denominator = allNodes.Count;

            foreach (Node nd in allNodes)
            {

            }
            return avg /= denominator;
        }*/
        /* ADDED */
        //return connectivity degree of Dungeon
		public double connectivityDegree()
        {
            int connected = 0;
            int numOfNodes = 0;
            foreach (Zone z in zones) //for each zone
            {
                foreach (Node n in z.nodesInZone) //for each node in the zone
                {
                    connected += n.neighbors.Count; //get their connections
                }
                numOfNodes += z.nodesInZone.Count; //get number of nodes
            }
            return connected / numOfNodes; // return total connections / total number of nodes
        }

        /* Return a shortest path between node u and node v */
        public List<Node> shortestpath(Node u, Node v)
        {
            List<Node> queue = new List<Node>(); //list of nodes to visit
            Dictionary<string, uint> nodeDist = new Dictionary<string, uint>(); //node id's and their distances 
                                                                                //nodeDist dictionary contains all reachable nodes from node u
            List<Node> reachableNodesFromU = predicates.reachableNodes(u); //get reachable nodes from the node u



			foreach (Node nd in reachableNodesFromU)//for each reachable node
            {
				nodeDist.Add(nd.id, Int32.MaxValue); //make its distance int max
                nd.visited = false; //make it not visited
                nd.pred = null; //and its predecessor null
            }

            //source node u is the first to be visited, change distance to 0, make it visited, add it to queue
            u.visited = true;
			nodeDist[u.id] = 0;
            queue.Add(u);

            uint tempDistance = 0; //temporary variable to store current distance to start point
            while (queue.Count != 0)
            { //while queue is not empty
                Node nd = queue.First(); //get first node at the queue
                queue.RemoveAt(0); //delete queue's first element
				tempDistance = nodeDist[nd.id]; //get the distance value

                
				foreach (Node tempNode in nd.neighbors) //for each neighbour of the nd  
                {
					if (!tempNode.visited) //if the neighbour node is not visited,
                    {
						tempNode.visited = true;//make it visited
						nodeDist[tempNode.id] = tempDistance + 1; //make its distance = distance of nd+1
						tempNode.pred = nd; //make its predecessor = nd
						queue.Add(tempNode); //add it into queue to explore
                    }
                    if (tempNode == v) break; //not sure if 'tempNode == v' is a valid compare
					//if the node is node v, stop BFS
                }

            }
            //now each reachable nodes have predecessor node information
            //can find the shortest path starting from Node v
            List<Node> path = new List<Node>(); //stores the path
                                                //path push back Node v
            Node current = v;// temp node

            path.Add(current); //add current node to path

            while (current.pred != null)
            {
                path.Add(current.pred); //path push back current.pred
                current = current.pred; //current = current.pred;
            }
            path.Reverse(); //starts from start node
            
            return path;
            //now the list path has the shortest path from v to u
            //return it in reverse order
        }


       
        /* To disconnect a bridge from the rest of the zone the bridge is in. */
        public void disconnect(Bridge b)
        {
            Logger.log("Disconnecting the bridge " + b.id + " from its zone.");

            b.disconnectFromSameZone();  //disconnet bridge from same zone
            startNode = b; //make bridge the startnode of the dungeon
            
        }

        /** ADDED */
        /* To calculate the level of the given node. */
        public uint level(Node d)
        {
            //get shortest path from starting node to node d
            //check list of nodes, increment the level with the number of bridge nodes
            uint nodeLevel = 0;
            List<Node> pathFromStartNode = shortestpath(startNode, d);
            foreach (Node nd in pathFromStartNode)
            {
                if (predicates.isBridge(startNode, exitNode, nd)) nodeLevel++;
            }
            return nodeLevel;
            //throw new NotImplementedException(); 
        }

        /**
         * NOT USED?
         * Returns true if there exist not connected nodes in the zone
         */
        /*
		public bool notConnectedGraph(List<Node> nodesInZone) 
        {
            for (int i = 0; i < nodesInZone.Count; i++)
            {
                Node n = nodesInZone.ElementAt(i);
                if (n.notConnected()) return true;
            }
            return false;
        }*/
    }

    public class Node
    {
        public String id;
        public List<Node> neighbors;
        public List<Pack> packs;
        public List<Item> items;
        public int level;
        public bool visited;
        public Node pred;

        public Node() { }
        public Node(String id, int level)
        {
            neighbors = new List<Node>();
            packs = new List<Pack>();
            items = new List<Item>();
            this.id = id;
            this.level = level;
            this.visited = false;
            this.pred = null;
        }

		/*NOT USED
		 public void setId(string id) 
        {
            this.id = id;
        }*/
		public bool isFullyConnected() //returns true if the node has maximum amount of connections(4)
        {
            return this.neighbors.Count == 4;
        }

        public bool notConnected() //returns true if the node does not have any neighbors
        {
            return this.neighbors.Count == 0;
        }
        /* To connect this node to another node. */
        public void connect(Node nd)
        {
            neighbors.Add(nd); nd.neighbors.Add(this); //add nodes to their respective neighbors list
        }

        /* To disconnect this node from the given node. */
        public void disconnect(Node nd)
        {
            neighbors.Remove(nd); nd.neighbors.Remove(this); //remove nodes from their neighbors list
        }

        public bool alreadyConnected(Node nd) //returns true if the nodes are already connected
        {
            return this.neighbors.Contains(nd); //meaning if it already contains that node in neighbors list
        }

        /**
         * Returns number of monsters in that node currently
         */

        public int currentNumberOfMonsters()
        {
            int numberOfMonsters = 0;
            foreach (Pack p in this.packs) //for each pack in the node
            {
                numberOfMonsters += p.members.Count; //add their number of monsters
            }
            return numberOfMonsters; //return total number of monsters in the node
        }

        /**
         * NOT USED
         * If the node contains a pack and a player
         * then the node is contested
         */
        /*
        public Boolean contested(Player player)
        {
            if (packs.Count > 0 && player.HP != 0)
            {
                return true;
            }
            else
            {
                return false;
            }
        }*/
		
        /**
         * Returns the sum of hp values of packs in that node
         */

        public int getNodeHPValue()
        {
            int nodeHPValue = 0;
            foreach (Pack p in this.packs) //for each pack
            {
                nodeHPValue += p.getPackHPValue(); //add pack's hp value
            }
            return nodeHPValue;
        }

        /* Execute a fight between the player and the packs in this node.
         * Such a fight can take multiple rounds as describe in the Project Document.
         * A fight terminates when either the node has no more monster-pack, or when
         * the player's HP is reduced to 0. 
         */
        /*
            Command List:
            1- Move to next node,flee
            2- Use Magic Crystal
            3- Use Healing Potion
            4- Attack
            */
        public int fight(Player player, int state)
        {
            int command = -1;
            if (state == 0)
            {
				//ensure player is not accelerated
				player.accelerated = false;
                Logger.log("Press 1 to flee");
                if (player.containsMagicCrystal())//if player bag contains item type of magic crystal
                    Logger.log("Press 2 to use Magic Crystal");
                if (player.containsHealingPotion()) //only show option if player has the potion
                    Logger.log("Press 3 to use Healing Potion");
                Logger.log("Press 4 to attack");
                command = player.getNextCommand().commandId;
                if (command == 1)
                {
                    //if presses 1 try to flee
                    //if possible game.state = 5
                    //if not game.state = 0
                    if (player.flee())
                    {
                        Logger.log("Player fleed, not contested anymore");
                        return 5; //next state 5
                    }
                    else
                    {
                        Logger.log("Player could not flee, still contested");
                        return 0;
                    }
                }
                else if (command == 2) //player wants to use magic crystal
                {
                    if (!player.containsMagicCrystal()) //if player has no magic crystal
                    {
                        Logger.log("Player has no magic crystal, press a valid command");
                        return 0;

                    }
                    //player uses magic crystal
                    foreach (Item i in player.bag)
                    {
                        if (i.GetType() == typeof(Crystal)) //find magic crystal in player's bag
                        {
                            player.use(i); 
                            Logger.log("Player used crystal");
                            break; //break the for loop
                        }
                        Logger.log("Could not be able to execute this");

                    }
                    return 2; //pass to 2nd state

                }
                else if (command == 3)//player wants to use healing potion
                {
                    if (!player.containsHealingPotion()) //if player has no healing potion
                    {
                        Logger.log("Player has no healing potion, press a valid command");
                        return 0;

                    }
                    foreach (Item i in player.bag) //else find healing potion in player's bag
                    {
                        if (i.GetType() == typeof(HealingPotion))
                        {
                            player.use(i); //use that potion
                            Logger.log("Player used potion");
                            break; //break the for loop
                        }
                        

                    }
                    //player uses healing potion
                    return 1;

                }
                else if (command == 4) //player chooses to attack
                {
                    //player attacks
                    Logger.log("Player is attacking");
                    //player attacks only one pack in the node
                    bool removePack = player.AttackBool(player.location.packs.First().members.First()); 
					//attackBool function returns true if that pack is killed and should be removed from the node
                    if (removePack)
                    {
                        player.location.packs.Remove(player.location.packs.First()); //if every monster died in the pack
                                                                                      //it gets removed from the node
                    }
                    if (player.location.packs.Count > 0) //if the node is still constested
                    {
                        return 3; //go to state 3
                    }
                    else                  
                    { //if the node is not contested anymore
                        return 6; //go to state 6
                    }
                    


                }


            }
            else if (state == 1) //if combat is at state 1
            {
				//player is not accelerated and
				//there is no other option than player attack
                Logger.log("Player is not accelerated, player is attacking a monster in a pack");
                bool removePack = player.AttackBool(player.location.packs.First().members.FirstOrDefault()); //player attacks one monster in one pack

                //call attack function and check if attacked pack should be removed
                if (removePack)
                {
                    player.location.packs.Remove(player.location.packs.First());
                }
				if (player.location.packs.Count > 0)//still contested
                {
                    return 3; //go to state 3
                }
                else
                {
                    return 6; //node is not contested anymore
                }


            }
            else if (state == 2) //if combat is at state 2
            {
				//player is accelerated and
                //there is no other option than player attack
                Logger.log("Player is accelerated, player is attacking all monsters in a pack");
                bool removePack = player.AttackBool(player.location.packs.First().members.FirstOrDefault()); //accelerated check is inside the function
                                                                                                             //call attack function and check if attacked pack should be removed
                if (removePack) //if all monsters in the pack die
                {
                    player.location.packs.Remove(player.location.packs.First()); //remove that pack from the location
                }
                if (player.location.packs.Count > 0)
                {
                    return 3; //still contested
                }
                else
                {
                    return 6; //not contested anymore
                }
                

            }
            else if (state == 3) //if the combat is at state 3
            {
                Logger.log("Pack flees or attacks");
                Pack pack = player.location.packs.First(); 
				if (pack.getAction() == 1) //pack attacks
                {
                    Logger.log("Pack attacks");
                    pack.Attack(player); //pack attacks to player
                    return 0;
                }
                else if (pack.getAction() == 2)
                { //pack flees
                    if (pack.flee()) //if pack can flee
                    {
                        Logger.log("Pack flees");
                        if (player.location.packs.Count > 0) //if the node is still contested
                        {
                            return 4; //go to state 4
                        }
                        else
                        {
                            return 6; //node is not contested anymore
                        }
                    }
                    else
                    { //if pack can not flee it attacks
                        Logger.log("Pack tried to flee, not possible. Pack attacks");
                        pack.Attack(player);
                        return 0;
                    }
                }
                
                //if flee probability> attack
                //pack flees
                //if still contested
                //game.state= 4
                //if not contested
                //game.state = 6
                //else pack attacks
                //game.state=0

            }
            else if (state == 4) //if combat state is 4
            {
                Logger.log("Pack attacks");
                player.location.packs.First().Attack(player); //first pack in the node attacks player
				//TO-DO randomly decide the pack
                return 0;
                //if player is still alive checked in main while

            }
            /*else if (state == 5) //if combat state is 5
            {
                Logger.log("Player successfully fleed, not contested anymore");
                return -1;
                //return -1; //exit
            }
            else if (state == 6)
            {
                Logger.log("Node is not contested anymore");
                return -1;
                //return -1; //exit

            }*/
            else
            { //it does not call the function with state 5 or 6
				//since the node is not contested anymore, it does not enter while loop again
                //this part is not executed only put for guarantee.
				Logger.log("Combat ends");
                
            }
			return -1;

            
            
        }
    }

    public class Bridge : Node
    {
        List<Node> fromNodes = new List<Node>();
        List<Node> toNodes = new List<Node>();
        public Bridge(String id, int level) : base(id, level) { }

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
            toNodes.Add(nd); //add this node to toNodes list of the bridge
        }

        /**
         * Same zone signifies nodes in the fromNodes list of a bridge,
         * it disconnects the bridge from same zone
         */

        public void disconnectFromSameZone()
        {
            //for each node in fromNodes list, remove the connection between the bridge (call base.disconnect(Node n))
            foreach (Node nd in this.fromNodes)
            {
                base.disconnect(nd); //call node disconnect
            }

        }
    }

    public class Zone
    {
		public List<Node> nodesInZone;//it stores every node in the zone
        public uint capacity; //number of monsters that each node can have in this node
        public int id; //level of the zone

        /**
         * Creates a zone with specified level and the capacity
         */

        public Zone(int level, uint M)
        {
			this.id = level; //zone id gives its level (Zone 1-> first level)
            this.capacity = (uint)(M * (level + 1)); //its capacity is calculated regarding the project document
            this.nodesInZone = new List<Node>(); //it stores every node in the zone

        }

        /**
         * Add Node n to the zone's nodesInZone list
         */

        public void addNodesToZone(Node n)
        {
            this.nodesInZone.Add(n); //nodesInZone stores every node in this zone
        }

        /**
         * Returns the zone's total hp value by adding up hp values of all nodes in that zone
         */

        public int getZoneHPValue()
        {
            int zoneHPValue = 0;
            foreach (Node n in this.nodesInZone) //for each zone in this zone
            {
                zoneHPValue += n.getNodeHPValue(); //add their hp values
            }
            return zoneHPValue;
        }
    }
}
