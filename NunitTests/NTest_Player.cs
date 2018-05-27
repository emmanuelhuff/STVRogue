using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using STVRogue.Utils;
using NUnit.Framework;

namespace STVRogue.GameLogic
{
    /* An example of a test class written using NUnit testing framework. 
     * This one is to unit-test the class Player. The test is incomplete though, 
     * as it only contains two test cases. 
     */
    [TestFixture]
    public class NTest_Player
    {
        
		[Test]
        public void NTest_createPlayer()
        {
            Player P = new Player();
			Assert.AreEqual(P.id, "player");
			Assert.AreEqual(P.AttackRating, 5);
			Assert.AreEqual(P.HPbase, 100);

        }

		[Test]
        public void NTest_bagContainsMagicCrystal_returnsTrue()
        {
            Player P = new Player();
			HealingPotion healingPotion = new HealingPotion("healing");
			Crystal crystal = new Crystal("crystal");
			P.bag.Add(healingPotion);
			P.bag.Add(crystal);
			Assert.IsTrue(P.containsMagicCrystal());

        }

		[Test]
        public void NTest_bagDoesNotContainMagicCrystal_returnsFalse()
        {
            Player P = new Player();
            HealingPotion healingPotion1 = new HealingPotion("healing1");
			HealingPotion healingPotion2 = new HealingPotion("healing2");
            P.bag.Add(healingPotion1);
			P.bag.Add(healingPotion2);
            Assert.IsFalse(P.containsMagicCrystal());

        }

		[Test]
        public void NTest_bagContainsHealingPotion_returnsTrue()
        {
            Player P = new Player();
            HealingPotion healingPotion = new HealingPotion("healing");
            Crystal crystal = new Crystal("crystal");
            P.bag.Add(healingPotion);
            P.bag.Add(crystal);
			Assert.IsTrue(P.containsHealingPotion());

        }

        [Test]
        public void NTest_bagDoesNotContainHealingPotion_returnsFalse()
        {
            Player P = new Player();
			Crystal crystal1 = new Crystal("crystal1");
			Crystal crystal2 = new Crystal("crystal2");
			P.bag.Add(crystal1);
			P.bag.Add(crystal2);
			Assert.IsFalse(P.containsHealingPotion());

        }

		[Test]
        public void NTest_use_onEmptyBag()
        {
            Player P = new Player();
            Assert.Throws<ArgumentException>(() => P.use(new Item()));
        }

        [Test]
        public void NTest_use_item_in_bag()
        {
            Player P = new Player();
            Item x = new HealingPotion("pot1");
            P.bag.Add(x);
            P.use(x);
            Assert.True(x.used);
            Assert.False(P.bag.Contains(x));
        }

		[Test]
        public void NTest_getHPValueOfBag_onEmptyBag_returnsZero()
        {
            Player P = new Player();
			Assert.Zero(P.getHPValueOfBag());
        }

		[Test]
        public void NTest_getHPValueOfBag_returnsValid()
        {
            Player P = new Player();
			HealingPotion healingPotion = new HealingPotion("hp");
			int originalHPValue = (int)healingPotion.HPvalue;
			P.bag.Add(healingPotion);
			Assert.AreEqual(originalHPValue, P.getHPValueOfBag());
        }




		[Test]
        public void NTest_attack_foeNotMonster_throwsException()
        {
            Player p1 = new Player();
			Player p2 = new Player();
			Assert.Throws<ArgumentException>(() => p1.Attack(p2));
        }

		[Test]
        public void NTest_attack_notAcceleratedFoeHPZero_creatureIsKilled()
        {
			Dungeon dungeon = new Dungeon(3, 4);
			Player testPlayer = new Player();
			Pack testPack = new Pack("testPack", 1,dungeon);
			testPack.location = dungeon.startNode;
            testPlayer.location = dungeon.startNode;
            Monster M = testPack.members.FirstOrDefault<Monster>();
            M.setHP(1);
			M.pack = testPack;
            testPlayer.Attack(M);
			Assert.IsTrue(testPack.members.Count==0);
			Assert.IsFalse(testPack.members.Count!=0);
            
            
        }
		[Test]
        public void NTest_attack_notAcceleratedFoeHPDiffThanZero_creatureIsNotKilled()
        {
			Player testPlayer = new Player();
			Dungeon dungeon = new Dungeon(3, 4);
			Pack testPack = new Pack("testPack", 1,dungeon);
			testPack.location = dungeon.startNode;
            testPlayer.location = dungeon.startNode;
			Monster testMonster = testPack.members.FirstOrDefault<Monster>();
            testMonster.setHP(6);
			testMonster.pack = testPack;
			testPlayer.Attack(testMonster);         
			Assert.IsTrue(testPack.members.Contains(testMonster));
			Assert.IsFalse(!testPack.members.Contains(testMonster));

        }
		[Test]
        public void NTest_attack_acceleratedTargetHPZero_targetIsKilled()
        {
			Player testPlayer = new Player();
			Dungeon dungeon = new Dungeon(3, 4);
			testPlayer.accelerated = true;
			Pack testPack = new Pack("testPack", 3,dungeon);
			testPack.location = dungeon.startNode;
            testPlayer.location = dungeon.startNode;
			foreach(Monster m in testPack.members){
				Logger.log(m.id);
			}
			Monster testMonster = testPack.members.FirstOrDefault<Monster>(); //first monster of the pack
			Logger.log("Test monster is " + testMonster.id+" with HP "+ testMonster.HP);
			testMonster.pack = testPack;
			Monster targetMonster = testPack.members.LastOrDefault<Monster>(); //last monster of the pack
			targetMonster.setHP(1); //should be killed
			Logger.log("Target monster is " + targetMonster.id+" with HP "+targetMonster.HP);
			targetMonster.pack = testPack;
			testPlayer.Attack(testMonster); //player attacks first monster
			Assert.IsTrue(!testPack.members.Contains(targetMonster)); //returns true if last monster is killed
			Assert.IsFalse(testPack.members.Contains(targetMonster));

        }
		[Test]
        public void NTest_attack_acceleratedTargetHPNotZero_targetIsNotKilled()
        {
			Dungeon dungeon = new Dungeon(3, 4);
			Player testPlayer = new Player();
            testPlayer.accelerated = true;
			Pack testPack = new Pack("testPack", 3,dungeon);
			testPack.location = dungeon.startNode;
			testPlayer.location = dungeon.startNode;
            foreach (Monster m in testPack.members)
            {
                Logger.log(m.id);
            }
            Monster testMonster = testPack.members.FirstOrDefault<Monster>(); //first monster of the pack
            Logger.log("Test monster is " + testMonster.id + " with HP " + testMonster.HP);
            testMonster.pack = testPack;

            Monster targetMonster = testPack.members.LastOrDefault<Monster>(); //last monster of the pack
			targetMonster.setHP((int)(testPlayer.AttackRating + 10)); //target monster should not be killed
            Logger.log("Target monster is " + targetMonster.id + " with HP " + targetMonster.HP);
			targetMonster.pack = testPack;
            
            testPlayer.Attack(testMonster); //player attacks first monster
            Assert.IsTrue(testPack.members.Contains(targetMonster)); //returns true if last monster is not killed
            Assert.IsFalse(!testPack.members.Contains(targetMonster));
        }

        //TEST getNextCommand
		[Test]
        public void NTest_getNextCommand_unknown()
        {
			Player player = new Player();
			Console.Write("Press any key 5 to 9");
			Assert.AreSame(new Command(-1), player.getNextCommand());
        }
		[Test]
        public void NTest_getNextCommand_known()
        {
			Player player = new Player();
            Console.Write("Press key 1");
            Assert.AreSame(new Command(1), player.getNextCommand());
        }
        //TEST flee
		[Test]
		public void NTest_flee_playerFlees(){
			Player player = new Player();
			Node n1 = new Node("1", 1);
            Node n2 = new Node("2", 1);
            n1.neighbors.Add(n2);
            n2.neighbors.Add(n1);
			player.location = n1;
			Assert.IsTrue(player.flee());         
			Assert.AreSame(player.location, n2);         
		}

		[Test]
        public void NTest_flee_playerCanNotFlee()
        {
			Dungeon dungeon = new Dungeon(3, 4);
			Player player = new Player();
            Node n1 = new Node("1", 1);
            Node n2 = new Node("2", 1);
            n1.neighbors.Add(n2);
            n2.neighbors.Add(n1);
            player.location = n1;
			Pack pack = new Pack("1", 2,dungeon);
			n2.packs.Add(pack);
			Assert.IsFalse(player.flee()); 
            Assert.AreSame(player.location, n1);

        }
        //TEST move
		[Test]
        public void NTest_move_playerMoves()
        {
			Dungeon dungeon = new Dungeon(3, 4);
			Player player = new Player();
			player.location = dungeon.startNode;
			player.move();
			Assert.AreSame(dungeon.startNode, player.location);
        }
        //TEST collectItems
		[Test]
        public void NTest_collectItems_ReturnsValid()
        {
            Player P = new Player();
			Node testNode = new Node("1", 1);
			HealingPotion healingPotion = new HealingPotion("hp");
			Crystal crystal = new Crystal("cr");

			testNode.items.Add(healingPotion);
			testNode.items.Add(crystal);
			List<Item> testList = testNode.items.ToList<Item>();
			P.location = testNode;

			P.collectItems();
			CollectionAssert.AreEquivalent(testList, P.bag.ToList<Item>());

        }
        //TEST attackbool
		[Test]
        public void NTest_attackBool_throwsException()
        {
			Player p1 = new Player();
            Player p2 = new Player();
            Assert.Throws<ArgumentException>(() => p1.Attack(p2));

        }
		[Test]
        public void NTest_attackBool_packIsBeated()
        {
			Dungeon dungeon = new Dungeon(3, 4);
			Player player = new Player();
			Pack pack = new Pack("1",1,dungeon);
			player.location = dungeon.startNode;
            pack.location = dungeon.startNode;
			uint playerAttackRating = player.AttackRating;
			int monsterHP = pack.members.First().HP;
			pack.members.First().setHP((int)Math.Max(1, playerAttackRating - 5));

			Assert.IsTrue(player.AttackBool(pack.members.First()));


        }
		[Test]
        public void NTest_attackBool_packIsAlive()
        {
			Dungeon dungeon = new Dungeon(3, 4);
			Player player = new Player();
			Pack pack = new Pack("1", 1,dungeon);
			player.location = dungeon.startNode;
			pack.location = dungeon.startNode;
            uint playerAttackRating = player.AttackRating;
            int monsterHP = pack.members.First().HP;
			pack.members.First().setHP((int)(playerAttackRating + 5));
			Assert.IsFalse(player.AttackBool(pack.members.First()));
        }
        

    }
}
