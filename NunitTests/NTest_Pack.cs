using System;
using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;
namespace STVRogue.GameLogic
{
    public class NTest_Pack
    {
		[Test]
        public void NTest_createPack()
        {
			Pack pack = new Pack("1", 4);
			int packStartingHP = 0;
			Assert.AreEqual(pack.members.Count, 4);
			foreach(Monster m in pack.members){
				packStartingHP += m.HP;
				Assert.AreSame(m.pack, pack);
			}
			Assert.AreEqual(packStartingHP, pack.startingHP);

		}
		[Test]
        public void NTest_attack_playerDies()
        {
			Player player = new Player();
			player.HP = 1;
			Pack pack = new Pack("1", 4);
			pack.Attack(player);
			Assert.AreEqual(player.HP, 0);
        }
		[Test]
        public void NTest_attack_playerAlive()
        {
			Player player = new Player();
            Pack pack = new Pack("1", 1);
			uint monsterAttackRating = pack.members.First().AttackRating;
			player.HP = (int)(monsterAttackRating + 5);
            pack.Attack(player);
			Assert.AreNotEqual(player.HP, 0);

        }
		[Test]
        public void NTest_move_throwsException()
        {
			Pack pack = new Pack("1", 2);
			Node node1 = new Node("1", 1);
			Node node2 = new Node("2", 1);
			pack.location = node1;
			node1.packs.Add(pack);
			Assert.Throws<ArgumentException>(() => pack.move(node2));
        }
		[Test]
        public void NTest_move_nodeIsAlreadyFullRejected()
        {
			Dungeon dungeon = new Dungeon(3, 3); //can use Utils.Test
            //capacity of a node in first zone = 6
			Pack pack1 = new Pack("1", 4);
			Pack pack2 = new Pack("2", 3);
			List<Node> firstZone = dungeon.zones.First().nodesInZone;
			if(firstZone.Count>1){
				pack1.location = firstZone.ElementAt(1);
				firstZone.ElementAt(1).packs.Add(pack1);
				pack2.location = firstZone.ElementAt(2);
				firstZone.ElementAt(2).packs.Add(pack2);
				firstZone.ElementAt(1).neighbors.Add(firstZone.ElementAt(2));
				firstZone.ElementAt(2).neighbors.Add(firstZone.ElementAt(1));
			}
			pack1.move(firstZone.ElementAt(2));
			//rejected
			Assert.AreSame(firstZone.ElementAt(1),pack1.location);
			
        }
		[Test]
        public void NTest_move_packMoves()
        {
			Dungeon dungeon = new Dungeon(3, 3); //can use Utils.Test
            //capacity of a node in first zone = 6
            Pack pack1 = new Pack("1", 4);
            List<Node> firstZone = dungeon.zones.First().nodesInZone;
            if(firstZone.Count>1){
                pack1.location = firstZone.ElementAt(1);
                firstZone.ElementAt(1).packs.Add(pack1);
                firstZone.ElementAt(1).neighbors.Add(firstZone.ElementAt(2));
                firstZone.ElementAt(2).neighbors.Add(firstZone.ElementAt(1));
            }
            pack1.move(firstZone.ElementAt(2));
            //rejected
            Assert.AreSame(firstZone.ElementAt(2),pack1.location);
            

        }
		[Test]
        public void NTest_moveTowards_validMove()
        {
			Dungeon dungeon = new Dungeon(3, 3);
			Pack pack1 = new Pack("1", 4);
			pack1.location = dungeon.startNode;
			dungeon.startNode.packs.Add(pack1);
			Node nextNode = (dungeon.shortestpath(dungeon.startNode, dungeon.exitNode))[0];
			pack1.moveTowards(dungeon.exitNode);
			Assert.AreSame(nextNode, pack1.location);

        }
		/*[Test]
        public void NTest_getMonster_noMonsterException()
        {

        }
		[Test]
        public void NTest_getMonster_returnsMonster()
        {

        }*/
		[Test]
        public void NTest_getAction_packFlees()
        {

        }
		[Test]
        public void NTest_getAction_packAttacks()
        {

        }

		[Test]
        public void NTest_getPackHPValue_validHP()
        {
			Pack pack = new Pack("1", 3);
			int packHP = 0;
			foreach(Monster m in pack.members){
				packHP += m.HP;
			}
			Assert.AreEqual(packHP, pack.getPackHPValue());

        }

		[Test]
        public void NTest_flee_returnsTruePackFlees()
        {
			Dungeon dungeon = new Dungeon(3, 3);
            Pack pack1 = new Pack("1", 4);
            pack1.location = dungeon.startNode;
            dungeon.startNode.packs.Add(pack1);
			//there is no monster packs in the dungeon so there is no capacity problems
			Assert.IsTrue(pack1.flee());
			Assert.AreNotSame(pack1.location, dungeon.startNode);
			Assert.Contains(pack1.location, dungeon.startNode.neighbors);

        }
		[Test]
        public void NTest_flee_returnsFalsePackCantFlee()
        {
			Dungeon dungeon = new Dungeon(3, 3);
            Pack pack1 = new Pack("1", 4);
            pack1.location = dungeon.startNode;
            dungeon.startNode.packs.Add(pack1);
			int packId = 2;
			foreach(Node n in dungeon.startNode.neighbors){
				Pack pack = new Pack("" + packId, 4);
				pack.location = n;
				n.packs.Add(pack);
			}
            Assert.IsFalse(pack1.flee());
            Assert.AreSame(pack1.location, dungeon.startNode);
        }
        

    }
}
