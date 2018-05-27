using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using STVRogue.Utils;
using System.Text;
using System.Threading.Tasks;

namespace STVRogue.GameLogic
{

    // overrides the getNextCommand player class
    // instead of creating a player, this test creates a player
    // now we can test the input 

    public class TestPlayer : Player   
    {
        public List <Command> nextCommand = new List<Command>;        
        new public Command getNextCommand()
        {
            Command = first().
            return nextCommand.get(0);
        }
    }   
    [TestFixture]
    public class NTest_Dungeon
	{
		public Random random = RandomGenerator.initializeWithSeed(5);
		Dungeon dungeon = new Dungeon(5, 6);
        [Test]
        public void NTest_check_isValidDungeon()
        {
            Assert.IsTrue(dungeon.predicates.isValidDungeon(dungeon.startNode, dungeon.exitNode, dungeon.difficultyLevel));
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
                Node newNode = new Node("" + i, 1);
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
         
            Assert.AreEqual(listToCheck, dungeon.shortestpath(nodes.ElementAt(3), nodes.ElementAt(10)));
        }
        [Test]
        public void NTest_Fight()
        {
            TestPlayer testPlayer = new TestPlayer();
            testPlayer.nextCommand = new Command(4);  //player fights          

            Dungeon dungeon = new Dungeon(3, 4);
            Pack pack = new Pack("1", 2, dungeon);
            Node node1 = new Node("1", 1);
            //Node node2 = new Node("2", 1);
            pack.location = node1;
            node1.packs.Add(pack);

            testPlayer.location = node1;
            node1.fight(testPlayer, 1);
            //Assert.IfFalse(node1.contested());
            node1.fight(testPlayer, 2);
            node1.fight(testPlayer, 3);
            node1.fight(testPlayer, 4);
            node1.fight(testPlayer, 5);
            node1.fight(testPlayer, 6);
        }      
        [Test]
        public void NTest_check_fullyConnected_notConnected()
        {
            Node n1 = new Node("" + 0, 1);
            Node n2 = new Node("" + 1, 1);
            Node n3 = new Node("" + 2, 1);
            Node n4 = new Node("" + 3, 1);
            Node n5 = new Node("" + 4, 1);
            Node n6 = new Node("" + 5, 1);
            n1.connect(n2);
            n1.connect(n3);
            n1.connect(n4);
            n1.connect(n5);
            Assert.IsTrue(n1.isFullyConnected());
            Assert.IsTrue(n6.notConnected());
        }
        [Test]
        public void NTest_check_connectedList()
        {
            Node n1 = new Node("" + 0, 1);
            Node n2 = new Node("" + 1, 1);
            Node n3 = new Node("" + 2, 1);
            n1.connect(n2);
            n1.connect(n3);
            List<Node> nodeList = new List<Node>();
            nodeList.Add(n2);
            nodeList.Add(n3);
            Assert.AreEqual(n1.neighbors, nodeList);
        }
        [Test]
        public void NTest_check_disconnectedNodes()
        {
            Node n1 = new Node("" + 0, 1);
            Node n2 = new Node("" + 1, 1);
            Node n3 = new Node("" + 2, 1);
            Node n4 = new Node("" + 3, 1);
            n1.connect(n2);
            n1.connect(n3);
            n1.connect(n4);
            List<Node> nodeList = new List<Node>();
            nodeList.Add(n2);
            nodeList.Add(n3);
            nodeList.Add(n4);
            Assert.AreEqual(n1.neighbors, nodeList);
            Assert.IsTrue(dungeon.predicates.isReachable(n1, n4));
            n1.disconnect(n4);
            nodeList.Remove(n4);
            Assert.AreEqual(n1.neighbors, nodeList);
            Assert.IsFalse(dungeon.predicates.isReachable(n1, n4));         
        }
        [Test]
        public void NTest_check_numberOfBridges()
        {
            Assert.AreEqual(dungeon.bridges.Count, 5);
        }

		[Test(Description ="Test function level() that the result is equal to expectation")]
        public void NTest_located_Level()
		{
			List<Zone> zones = dungeon.zones;
			Node n1 = zones.ElementAt(1).nodesInZone.First();
			Assert.AreEqual(dungeon.level(n1), 1);
			Node n1 = new Node("1",1);
			Assert.AreEqual(n1.level, 1);
        }     
		[Test]
		public void NTest_check_alreadyConnected()
		{
            Node n1 = new Node("" + 1, 1);
            Node n2 = new Node("" + 2, 1);
            n1.connect(n2);
            Assert.IsTrue(n1.alreadyConnected(n2));
		}
        [Test]
        public void NTest_check_numberOfMonsters()
        {
            Pack p1 = new Pack("1", 3, dungeon);
            Pack p2 = new Pack("2", 5, dungeon);
            Node n1 = new Node("1", 1);
            n1.packs.Add(p1);
            Assert.AreEqual(n1.currentNumberOfMonsters(), 3);
            n1.packs.Add(p2);
            Assert.AreEqual(n1.currentNumberOfMonsters(), 8);
            n1.packs.Remove(p1);
            Assert.AreEqual(n1.currentNumberOfMonsters(), 5);
        }
        [Test]
        public void NTest_check_nodeHP()
        {
            Pack p1 = new Pack("1", 3, dungeon);
            Pack p2 = new Pack("2", 5, dungeon);
            Node n1 = new Node("1", 1);
            int p1HP = p1.getPackHPValue();
            int p2HP = p2.getPackHPValue();
            n1.packs.Add(p1);
            Assert.AreEqual(n1.getNodeHPValue(), p1HP);
            n1.packs.Add(p2);
            Assert.AreEqual(n1.getNodeHPValue(), p1HP+p2HP);
            n1.packs.Remove(p1);
            Assert.AreEqual(n1.getNodeHPValue(), p2HP);
        }
        [Test]
        public void NTest_check_bridgeConnectToSameZone()
        {
            List<Node> bridgeNodes = new List<Node>();
            Node n1 = new Node("1", 1);
            Node n2 = new Node("2", 1);
            Bridge b1 = new Bridge("b", 1);
            b1.connectToNodeOfSameZone(n1);
            b1.connectToNodeOfSameZone(n2);
            bridgeNodes.Add(n1);
            bridgeNodes.Add(n2);
            Assert.AreEqual(b1.neighbors, bridgeNodes);         
        }
        [Test]
        public void NTest_check_bridgeConnectToNextZone()
        {
            List<Node> bridgeNodes = new List<Node>();
            Node n3 = new Node("3", 2);
            Node n4 = new Node("4", 2);
            Bridge b1 = new Bridge("b", 1);
            b1.connectToNodeOfNextZone(n3);
            b1.connectToNodeOfNextZone(n4);
            bridgeNodes.Add(n3);
            bridgeNodes.Add(n4);
            Assert.AreEqual(b1.neighbors, bridgeNodes);
        }
        [Test]
        public void NTest_check_bridgeDisconnectOfSameZone()
        {
            List<Node> bridgeNodes = new List<Node>();
            Node n1 = new Node("1", 1);
            Node n2 = new Node("2", 1);
            Bridge b1 = new Bridge("b", 1);
            b1.connectToNodeOfSameZone(n1);
            b1.connectToNodeOfSameZone(n2);
            bridgeNodes.Add(n1);
            bridgeNodes.Add(n2);
            Assert.AreEqual(b1.neighbors, bridgeNodes);
            b1.disconnectFromSameZone();
            Assert.IsEmpty(b1.neighbors);
        }
        [Test]
        public void NTest_check_addNodesToZone()
        {
            Zone z = new Zone(1, 3);
            Node n1 = new Node("1", 1);
            Node n2 = new Node("2", 1);
            Node n3 = new Node("3", 2);
            List<Node> toCheck = new List<Node>();
            z.addNodesToZone(n1);
            z.addNodesToZone(n2);
            z.addNodesToZone(n3);
            Assert.AreEqual(z.nodesInZone.Count, 3);
            toCheck.Add(n1);
            toCheck.Add(n2);
            toCheck.Add(n3);
            Assert.AreEqual(z.nodesInZone, toCheck);
        }
        [Test]
        public void NTest_check_zoneHP()
        {
            Pack p1 = new Pack("1", 3, dungeon);
            Pack p2 = new Pack("2", 5, dungeon);
            Pack p3 = new Pack("3", 4, dungeon);
            Zone z = new Zone(1, 14);
            Node n1 = new Node("1", 1);
            Node n2 = new Node("2", 1);
            Node n3 = new Node("3", 1);
            int p1HP = p1.getPackHPValue();
            int p2HP = p2.getPackHPValue();
            int p3HP = p3.getPackHPValue();
            n1.packs.Add(p1);
            n1.packs.Add(p2);
            z.addNodesToZone(n1);
            Assert.AreEqual(z.getZoneHPValue(), p1HP + p2HP);
            n2.packs.Add(p2);
            n2.packs.Add(p3);
            z.addNodesToZone(n2);
            Assert.AreEqual(z.getZoneHPValue(), p1HP + (2 * p2HP) + p3HP);
            n3.packs.Add(p1);
            n3.packs.Add(p3);
            z.addNodesToZone(n3);
            Assert.AreEqual(z.getZoneHPValue(), (p1HP * 2) + (p2HP * 2) + (p3HP * 2));
            z.nodesInZone.Remove(n2);
            Assert.AreEqual(z.getZoneHPValue(), (p1HP * 2) + p2HP + p3HP);         
        }
        [Test]
        public void NTest_check_fight()
        {
            Player p = new Player();
            Pack p1 = new Pack("1", 3, dungeon);
            Node n1 = new Node("1", 1);
            Node n2 = new Node("2", 1);
            n1.connect(n2);
            n1.packs.Add(p1);
            p.location = n1;
            n1.fight(p, 4);
            Assert.IsTrue(p.HP < 100);
            int checkHP = p.HP;
            n1.fight(p, 3);
            if(p1.getAction() == 1)
            {
                Assert.IsTrue(p.HP < checkHP);
            }
            else if(p1.getAction() == 2)
            {
                Assert.AreEqual(p.location.packs.Count, 0);
            }
        }
		[Test]
        public void NTest_check_averageConnectivity()
        {
            Assert.True(dungeon.connectivityDegree() < 3);
        }
    }
}
