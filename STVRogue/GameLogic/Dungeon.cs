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
        public Dungeon(uint level, uint nodeCapacityMultiplier)
        {
            Logger.log("Creating a dungeon of difficulty level " + level + ", node capacity multiplier " + nodeCapacityMultiplier + ".");
            difficultyLevel = level;
            bridges = new List<Bridge>(); //List of bridges initialized
            predicates = new Predicates();
            zones = new List<Zone>();
            M = nodeCapacityMultiplier;
            int numberOfNodesInZone = 0;

            //every node is named with its zone number followed by its number in a zone (nodeId = preId+lastId)
            string preId, lastId, nodeId = "";
            startNode = new Node("" + 10, 1); //start node id is set
            Logger.log("Set startNode Id: 10");
            for (int i = 1; i < level + 1; i++) //i signifies zone level
            {
                Zone newZone = new Zone(i, nodeCapacityMultiplier);
                zones.Add(newZone);
                Logger.log("Creating level " + i);
                //List<Node> nodesInZone = new List<Node>(); // stores nodes in the level
                preId = "" + i; // preId is zone level
                numberOfNodesInZone = RandomGenerator.rnd.Next(2, 5); //randomly decide between 2-4 nodes in a zone
                                                                      //if you change number of nodes can be in a zone, be careful with the dependent statements below
                Logger.log("Number of nodes in zone " + i + " is " + numberOfNodesInZone);

                for (int j = 1; j <= numberOfNodesInZone; j++)
                { //create and add nodes to the list nodesInZone
                    lastId = "" + j;
                    nodeId = preId + lastId; //merge preId and lastId to create nodeId
                    Node newNode = new Node(nodeId, i); //create node
                    Logger.log("Created node with id " + nodeId);
                    newZone.nodesInZone.Add(newNode); //add node to the list
                }

                //nodesInZone stores every node in this zone
                int numberOfNodesToConnect; // temp variable to decide how many nodes to connect for startNode or for bridges
                Node zoneFirstNode; //holds start node or starting bridge for the zone 
                                    //(starting bridge for a zone is the bridge which is connecting it to the previous zone)

                if (i == 1)
                { // connect start node to some nodes in the zone
                    zoneFirstNode = startNode;
                    numberOfNodesToConnect = RandomGenerator.rnd.Next(1, numberOfNodesInZone + 1); //randomly decide btw 1-4 nodes
                                                                                                   //rnd operation is exclusive for the max number, numberofNodesInZone can be 4 at most, thus it is safe this way
                    Logger.log("Connecting startNode to " + numberOfNodesToConnect + " nodes in the zone ");
                    for (int j = 0; j < numberOfNodesToConnect; j++)
                    { //connect them with the start node
                        int nodeIndex = RandomGenerator.rnd.Next(0, numberOfNodesInZone); //randomly get node index to connect
                        while (startNode.alreadyConnected(newZone.nodesInZone.ElementAt(nodeIndex)))
                        { //if the chosen node is already connected
                            nodeIndex = RandomGenerator.rnd.Next(0, numberOfNodesInZone); //choose a new one
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

                    if (numberOfNodesInZone < maxConnect)
                        maxConnect = numberOfNodesInZone;
                    numberOfNodesToConnect = RandomGenerator.rnd.Next(1, maxConnect + 1); //Decide how many connections it should make
                    Logger.log("Connecting bridge " + startBridge.id + " to " + numberOfNodesToConnect + " nodes in the next zone ");
                    for (int j = 0; j < numberOfNodesToConnect; j++)
                    { //connect them with the bridge node
                        int nodeIndex = RandomGenerator.rnd.Next(0, numberOfNodesInZone); //randomly decide the node index
                        while (startBridge.alreadyConnected(newZone.nodesInZone.ElementAt(nodeIndex)))
                        {
                            nodeIndex = RandomGenerator.rnd.Next(0, numberOfNodesInZone);
                        }
                        startBridge.connectToNodeOfNextZone(newZone.nodesInZone.ElementAt(nodeIndex)); //connect bridge with the next zone
                        Logger.log("Connected to node " + newZone.nodesInZone.ElementAt(j).id);
                    }




                }
                Logger.log("Connecting nodes in the same zone");

                //connect nodes in the same zone
                //TO-DO improve the algorithm
                //Currently it exits the algorithm once every node is accessible from the starting node of the zone            
                while (!allReachable(newZone.nodesInZone, zoneFirstNode))
                {

                    Node n1 = newZone.nodesInZone.ElementAt(RandomGenerator.rnd.Next(0, numberOfNodesInZone)); //randomly choose node1
                    while (n1.isFullyConnected()) //if it is fully connected node
                    {
                        n1 = newZone.nodesInZone.ElementAt(RandomGenerator.rnd.Next(0, numberOfNodesInZone)); //choose another one
                    }
                    Node n2 = newZone.nodesInZone.ElementAt(RandomGenerator.rnd.Next(0, numberOfNodesInZone)); //randomly select node2
                    while (n2.isFullyConnected() || n2.id == n1.id || n2.alreadyConnected(n1)) //make sure it is not fully connected, not same as node1, not already connected with node1
                    {
                        n2 = newZone.nodesInZone.ElementAt(RandomGenerator.rnd.Next(0, numberOfNodesInZone));
                    }
                    n1.connect(n2);
                    Logger.log("Nodes " + n1.id + " " + n2.id + " are connected");
                }


                List<Node> listOfNotFullNodes = new List<Node>(); //get list of not fully connected nodes to consider connections
                for (int j = 0; j < newZone.nodesInZone.Count; j++)
                {
                    Node nd = newZone.nodesInZone.ElementAt(j);
                    if (!nd.isFullyConnected()) listOfNotFullNodes.Add(nd);
                }
                Logger.log("Creating list of not full nodes, number of not full nodes " + listOfNotFullNodes.Count);

                //will connect last node to the zone
                int min = 1;
                int max;
                if (i == level)
                { // last zone
                  //connect exit node

                    lastId = "" + (numberOfNodesInZone + 1);
                    nodeId = preId + lastId;
                    exitNode = new Node(nodeId, i); //create exit node's id
                    Logger.log("Last zone is finished, exit node id is set to " + nodeId);
                    max = 4;
                    if (listOfNotFullNodes.Count < 4) max = listOfNotFullNodes.Count; //can make at most listOfNotFullNodes.Count number of connections
                    numberOfNodesToConnect = RandomGenerator.rnd.Next(min, max + 1);
                    for (int j = 0; j < numberOfNodesToConnect; j++)
                    {
                        exitNode.connect(listOfNotFullNodes.ElementAt(j)); //not randomized
                        Logger.log("Connected to node " + listOfNotFullNodes.ElementAt(j).id);
                    }
                }
                else
                { //connect to end bridge
                  //a bridge can be connected to at minimum 1 and at maximum 3 nodes
                    lastId = "" + (numberOfNodesInZone + 1);
                    nodeId = preId + lastId;
                    Bridge endBridge = new Bridge(nodeId, i); //create the bridge
                    bridges.Add(endBridge); //add it to bridges list to access it in the next loop iteration
                    Logger.log("A new bridge is created with id " + nodeId);

                    max = 3;
                    if (listOfNotFullNodes.Count < 3) max = listOfNotFullNodes.Count;  //can make at most listOfNotFullNodes.Count number of connections
                    numberOfNodesToConnect = RandomGenerator.rnd.Next(min, max + 1);
                    for (int j = 0; j < numberOfNodesToConnect; j++)
                    {
                        endBridge.connectToNodeOfSameZone(listOfNotFullNodes.ElementAt(j)); //not randomized
                        Logger.log("Connected to node " + listOfNotFullNodes.ElementAt(j).id);
                    }
                    //end bridge is also connected

                }


                //pass to next zone
                Logger.log("Passing to next zone");

            }

        }
        public bool allReachable(List<Node> toReachNodes, Node mainNode)
        {
            for (int i = 0; i < toReachNodes.Count; i++)
            {
                if (!predicates.isReachable(toReachNodes.ElementAt(i), mainNode)) return false;
            }
            return true;
        }
        /* ADDED */
        //return average of connectivity for Dungeon
        public float checkAvg(List<Node> allNodes)
        {
            float avg = 0;
            int denominator = allNodes.Count;

            foreach (Node nd in allNodes)
            {

            }
            return avg /= denominator;
        }

        /* Return a shortest path between node u and node v */
        public List<Node> shortestpath(Node u, Node v)
        {
            List<Node> queue = new List<Node>(); //list of nodes to visit
            Dictionary<string, uint> nodeDist = new Dictionary<string, uint>(); //node id's and their distances 
                                                                                //nodeDist dictionary contains all reachable nodes from node u
            List<Node> reachableNodesFromU = predicates.reachableNodes(u);
            //make their distances int max
            foreach (Node nd in reachableNodesFromU)
            {
                nodeDist.Add(nd.id, Int32.MaxValue); //include math to use INT_MAX?, using System? for Int32.MaxValue
                nd.visited = false;
                nd.pred = null;
            }
            //source node u is the first to be visited, change distance to 0, make it visited, add it to queue
            u.visited = true;
            nodeDist.Add(u.id, 0);
            queue.Add(u);
            uint tempDistance = 0;

            while (queue.Count != 0)
            { //while queue is not empty
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
                foreach (Node tempNode in nd.neighbors)
                {
                    if (!tempNode.visited)
                    {
                        tempNode.visited = true;
                        nodeDist.Add(tempNode.id, tempDistance + 1);
                        tempNode.pred = nd;
                        queue.Add(tempNode);
                    }
                    if (tempNode == v) break; //not sure if 'tempNode == v' is a valid compare
                }

            }
            //now each reachable nodes have predecessor node information
            //can find the shortest path starting from Node v
            List<Node> path = new List<Node>(); //stores the path
                                                //path push back Node v
            Node current = v;// temp node

            path.Add(current); // *****DOES IT REALLY WORK like push_back?

            while (current.pred != null)
            {
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
        public void disconnect(Bridge b)
        {
            Logger.log("Disconnecting the bridge " + b.id + " from its zone.");

            b.disconnectFromSameZone();  /** ADDED */
            startNode = b;
            //throw new NotImplementedException();
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

        public bool notConnectedGraph(List<Node> nodesInZone)
        {
            for (int i = 0; i < nodesInZone.Count; i++)
            {
                Node n = nodesInZone.ElementAt(i);
                if (n.notConnected()) return true;
            }
            return false;
        }

    }

    public class Node
    {
        public String id;
        public List<Node> neighbors = new List<Node>();
        public List<Pack> packs = new List<Pack>();
        public List<Item> items = new List<Item>();
        public int level;
        public bool visited;
        public Node pred;

        public Node() { }
        public Node(String id, int level)
        {
            this.id = id;
            this.level = level;
            this.visited = false;
            this.pred = null;
        }

        public void setId(string id)
        {
            this.id = id;
        }
        public bool isFullyConnected()
        {
            return this.neighbors.Count == 4;
        }

        public bool notConnected()
        {
            return this.neighbors.Count == 0;
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

        public bool alreadyConnected(Node nd)
        {
            return this.neighbors.Contains(nd);
        }

        //ADDED
        public int currentNumberOfMonsters()
        {
            int numberOfMonsters = 0;
            foreach (Pack p in this.packs)
            {
                numberOfMonsters += p.members.Count;
            }
            return numberOfMonsters;
        }

        /* ADDED */
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
        }
        //ADDED
        public int getNodeHPValue()
        {
            int nodeHPValue = 0;
            foreach (Pack p in this.packs)
            {
                nodeHPValue += p.getPackHPValue();
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
                Logger.log("Press 1 to flee");
                if (player.containsMagicCrystal())//if player bag contains item type of magic crystal
                    Logger.log("Press 2 to use Magic Crystal");
                if (player.containsHealingPotion())
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
                else if (command == 2)
                {
                    if (!player.containsMagicCrystal())
                    {
                        Logger.log("Player has no magic crystal, press a valid command");
                        return 0;

                    }
                    //player uses magic crystal
                    foreach (Item i in player.bag)
                    {
                        if (i.GetType() == typeof(Crystal))
                        {
                            player.use(i);
                            Logger.log("Player used crystal");
                            break; //break the for loop
                        }
                        Logger.log("Could not be able to execute this");

                    }
                    return 2;

                }
                else if (command == 3)
                {
                    if (!player.containsHealingPotion())
                    {
                        Logger.log("Player has no healing potion, press a valid command");
                        return 0;

                    }
                    foreach (Item i in player.bag)
                    {
                        if (i.GetType() == typeof(HealingPotion))
                        {
                            player.use(i);
                            Logger.log("Player used potion");
                            break; //break the for loop
                        }
                        Logger.log("Could not be able to execute this");

                    }
                    //player uses healing potion
                    return 1;

                }
                else if (command == 4)
                {
                    //player attacks
                    Logger.log("Player attacked");
                    bool removePack = player.AttackBool(player.location.packs.First().members.First());
                    if (removePack)
                    {
                        player.location.packs.Remove(player.location.packs.First());
                    }
                    ///ASK:
                    /// if player attacks the last monster in a pack, pack dies
                    /// after this pack is removed, node can pass to the not contested situation if there is no other monster packs
                    return 3;


                }


            }
            else if (state == 1)
            {
                Logger.log("Player is not accelerated, player is attacking a monster in a pack");
                bool removePack = player.AttackBool(player.location.packs.First().members.FirstOrDefault()); //player attacks one monster in one pack

                //call attack function and check if attacked pack should be removed
                if (removePack)
                {
                    player.location.packs.Remove(player.location.packs.First());
                }
                if (player.location.packs.Count > 0)
                {
                    return 3; //still contested
                }
                else
                {
                    return 6;
                }
                //if still contested
                //game.state = 3
                //else game.state=6


            }
            else if (state == 2)
            {
                Logger.log("Player is accelerated, player is attacking all monsters in a pack");
                bool removePack = player.AttackBool(player.location.packs.First().members.FirstOrDefault()); //accelerated check is inside the function
                                                                                                             //call attack function and check if attacked pack should be removed
                if (removePack)
                {
                    player.location.packs.Remove(player.location.packs.First());
                }
                if (player.location.packs.Count > 0)
                {
                    return 3; //still contested
                }
                else
                {
                    return 6;
                }
                //if still contested
                //game.state = 3
                //else game.state=6

            }
            else if (state == 3)
            {
                Logger.log("Pack flees or attacks");
                Pack pack = player.location.packs.First();
                if (pack.getAction() == 1)
                {//pack attacks
                    Logger.log("Pack attacks");
                    pack.Attack(player); //TO-DO check attack method
                    return 0;
                }
                else if (pack.getAction() == 2)
                { //pack flees
                    if (pack.flee())
                    {//TO-DO check flee method
                        Logger.log("Pack flees");
                        if (player.location.packs.Count > 0)
                        {
                            return 4;
                        }
                        else
                        {
                            return 6;
                        }
                    }
                    else
                    {
                        Logger.log("Pack tried to flee, not possible. Pack attacks");
                        pack.Attack(player);
                        return 0;
                    }
                }
                else
                {
                    Logger.log("Not possible");
                    return -1;
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
            else if (state == 4)
            {
                Logger.log("Pack attacks");
                player.location.packs.First().Attack(player); //TO-DO randomly decide the pack
                return 0;
                //game.state = 0 //if player is still alive checked in main while

            }
            else if (state == 5)
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

            }
            else
            {
                return -1;
            }

            Logger.log("Control");
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
            toNodes.Add(nd);
        }

        /** ADDED */
        public void disconnectFromSameZone()
        {
            //for each node in fromNodes list, remove the connection between the bridge (call base.disconnect(Node n))
            foreach (Node nd in this.fromNodes)
            {
                base.disconnect(nd);
            }

        }
    }

    public class Zone
    {
        public List<Node> nodesInZone;
        public uint capacity;
        public int id;

        public Zone(int level, uint M)
        {
            this.id = level;
            this.capacity = (uint)(M * (level + 1));
            this.nodesInZone = new List<Node>();

        }
        public void addNodesToZone(Node n)
        {
            this.nodesInZone.Add(n);
        }

        public int getZoneHPValue()
        {
            int zoneHPValue = 0;
            foreach (Node n in this.nodesInZone)
            {
                zoneHPValue += n.getNodeHPValue();
            }
            return zoneHPValue;
        }
    }
}
