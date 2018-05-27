using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using STVRogue.GameLogic;

namespace STVRogue.Utils
{
    /* Providing some useful predicates and functions to extract information from various
     * game entities.
     */
    public class Predicates
    {
        public Boolean isPath(List<Node> path)
        {
            Node[] path_ = path.ToArray();
            if (path_.Length <= 1) return true;
            Node a = path_[0];
            for (int i = 1; i < path_.Length; i++)
            {
                if (!a.neighbors.Contains(path_[i])) return false;
                a = path_[i];
            }
            return true;
        }
        
        public List<Node> reachableNodes(Node x0)
        {
            List<Node> seen = new List<Node>();
            List<Node> todo = new List<Node>();
            todo.Add(x0);
            while (todo.Count != 0)
            {
                x0 = todo[0]; todo.RemoveAt(0);
                seen.Add(x0);
                //Console.WriteLine("++ marking " + x0.id + " as seen");
                foreach (Node y in x0.neighbors)
                {
                  //  Console.WriteLine("-- considering " + x0.id + " -> " + y.id);
                    if (!seen.Contains(y) && !todo.Contains(y))
                    {
                    //    Console.WriteLine("++ adding " + y.id + " to todo-list");
                        todo.Add(y);
                    }
                }
            }
            return seen;
        }

        public Boolean isReachable(Node u, Node v)
        {
			List<Node> reachables = reachableNodes(u);
			//Logger.log("Can be reached:");
			//foreach (Node n in reachables)
			//	Logger.log(n.id);
			//Logger.log("we want to reach " + v.id);
            return reachableNodes(u).Contains(v);
        }

        /* Check if a node is actually a bridge node. */
        public Boolean isBridge(Node startNode, Node exitNode, Node nd)
        {
            if (nd == startNode || nd == exitNode) return false;
            List<Node> around = nd.neighbors;
            // temporarily disconnect the bridge 
            foreach (Node a in around) a.neighbors.Remove(nd);
            Boolean isBridge = true;
            if (isReachable(startNode, exitNode)) isBridge = false;
            // restore the connections
            foreach (Node a in around) a.neighbors.Add(nd);
                     
            
            return isBridge;
        }

        /* Count the number of bridges between the given start and exit node. */
        public uint countNumberOfBridges(Node startNode, Node exitNode)
        {
            List<Node> nodes = reachableNodes(startNode);
            uint n = 0;
            foreach (Node nd in nodes)
				if (isBridge(startNode, exitNode, nd)){
					n++;
					Logger.log("counted as bridge " + nd.id);
				} 
            return n;
        }

        /* Check if a graph beween start and end nodes forms a valid dungeon of the
         * specified level.
         */
        public Boolean isValidDungeon(Node startNode, Node exitNode, uint level)
        {
			Logger.log("HERE");
            if (startNode is Bridge || exitNode is Bridge) return false;
			Logger.log("HERE");
            if (countNumberOfBridges(startNode, exitNode) != level) return false;
			Logger.log("will check nodes contain exit node");
            List<Node> nodes = reachableNodes(startNode);
			foreach(Node nd in nodes){
				Logger.log(nd.id);
			}
			Logger.log("start exit reachable " + isReachable(startNode, exitNode));
            if (!nodes.Contains(exitNode)) return false;
			Logger.log("2ill check connectivity");
            int totalConnectivityDegree = 0;
            foreach (Node nd in nodes)
            {
                foreach (Node nd2 in nd.neighbors)
                {
                    // check that each connection is bi-directional
                    if (!nd2.neighbors.Contains(nd)) return false;
                }
                // check the connectivity degree
                if (nd.neighbors.Count > 4) return false;
                totalConnectivityDegree += nd.neighbors.Count;
                // check bridge
                Boolean isBridge_ = isBridge(startNode, exitNode, nd);
                if (nd is Bridge && !isBridge_) return false;
                if (!(nd is Bridge) && isBridge_) return false;
            }
			Logger.log("before avrg connectivity");
            float avrgConnectivity = (float)totalConnectivityDegree / (float)nodes.Count;
            if (avrgConnectivity > 3) return false;
            return true;
        }
    }
}
