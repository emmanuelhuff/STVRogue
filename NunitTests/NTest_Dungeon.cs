using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace STVRogue.GameLogic
{
    [TestFixture]
    public class NTest_Dungeon
	{
		Dungeon dungeon = new Dungeon(5, 6);
        [Test]
        public void NTest_check_ifValidDungeon()
        {
            Assert.IsTrue(dungeon.predicates.isValidDungeon(dungeon.startNode, dungeon.exitNode, 5));
        }
        [Test]
        public void NTest_check_allReachable()
        {
            List<Node> nodes = new List<Node>();
            Node n1, n2;
            Node startNode = new Node("" + 10, 1);
            nodes.Add(startNode);
            for (int i = 1; i <= 10; i++)
            {
                Node newNode = new Node("" + (10 + i), 1);
                nodes.Add(newNode);
            }
            for (int i = 1; i <= 3; i++)
            {
                startNode.connect(nodes.ElementAt(i));
            }
            for (int i = 3; i < 9; i++)
            {
                n1 = nodes.ElementAt(i+1);
                n2 = nodes.ElementAt(i+2);
                nodes.ElementAt(i).connect(n1);
                nodes.ElementAt(i).connect(n2);
            }
            Assert.IsTrue(dungeon.allReachable(nodes, startNode));
        }
        [Test]
        public void NTest_check_moreAllReachable()
        {
            Assert.IsTrue(dungeon.allReachable(dungeon.zones.ElementAt(2).nodesInZone, dungeon.startNode));
            Assert.IsTrue(dungeon.allReachable(dungeon.zones.ElementAt(4).nodesInZone, dungeon.bridges.ElementAt(3)));
        }
		[Test(Description = "Test function shortestpath() that the result is equal to expectation")]
		public void NTest_shortestpath()
		{
            List<Node> nodes = new List<Node>();
            Node n1, n2;
            Node startNode = new Node("" + 0, 1);
            nodes.Add(startNode);
            for (int i = 1; i <= 10; i++)
            {
                Node newNode = new Node("" + (i), 1);
                nodes.Add(newNode);
            }
            for (int i = 1; i <= 3; i++)
            {
                startNode.connect(nodes.ElementAt(i));
            }
            for (int i = 3; i < 9; i++)
            {
                n1 = nodes.ElementAt(i + 1);
                n2 = nodes.ElementAt(i + 2);
                nodes.ElementAt(i).connect(n1);
                nodes.ElementAt(i).connect(n2);
            }
            List<Node> listToCheck = new List<Node>();
            List<Node> pathToCheck = new List<Node>();
            pathToCheck.Add(nodes.ElementAt(3));
            listToCheck.Add(nodes.ElementAt(3));
            for (int i = 4; i <=10; i++)
            {
                listToCheck.Add(nodes.ElementAt(i));
                i++;
            }
            Assert.AreSame(listToCheck, dungeon.shortestpath(nodes.ElementAt(3), nodes.ElementAt(10)));
        }
        [Test]
        public void NTest_check_fullyConnected_notConnected()
        {
            Node n1 = new Node("" + 0, 1);
            Node n2 = new Node("" + 1, 1);
            Node n3 = new Node("" + 2, 1);
            Node n4 = new Node("" + 3, 1);
            Node n5 = new Node("" + 4, 1);
            n1.connect(n2);
            n1.connect(n3);
            n1.connect(n4);
            n1.connect(n5);
            Assert.IsTrue(n1.isFullyConnected());
        }

		[Test(Description ="Test function level() that the result is equal to expectation")]
        public void NTest_located_Level()
		{
			Node n1 = new Node("1",1);
			Assert.Equals(dungeon.level(n1), 1);
        }
              
		[Test(Description ="Test function disconnect() that Disconnection is fully completed")]
		public void NTest_complete_disconnection()
		{
			//TO-DO: Check startNode is b
		}
        
		[Test(Description = "")]
        public void NTest_check_averageConnectivity()
        {
            Assert.True(dungeon.connectivityDegree() < 3);
        }
    }
}
