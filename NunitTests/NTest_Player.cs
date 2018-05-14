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
        public void NTest_attack_foeNotMonster_throwsException()
        {
            Player p1 = new Player();
			Player p2 = new Player();
			Assert.Throws<ArgumentException>(() => p1.Attack(p2));
        }

		[Test]
        public void NTest_attack_notAcceleratedFoeHPZero_creatureIsKilled()
        {
			Player testPlayer = new Player();
            Pack testPack = new Pack("testPack", 1);
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
			Pack testPack = new Pack("testPack", 1);
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
			testPlayer.accelerated = true;
			Pack testPack = new Pack("testPack", 3);
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
			Player testPlayer = new Player();
            testPlayer.accelerated = true;
            Pack testPack = new Pack("testPack", 3);
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
    }
}
