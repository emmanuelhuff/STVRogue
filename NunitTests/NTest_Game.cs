using NUnit.Framework;
using STVRogue.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace STVRogue.GameLogic
{
    [TestFixture]
    public class NTest_Game
    {

		//public Random random = RandomGenerator.rnd;
		public Random random = RandomGenerator.initializeWithSeed(5);
        public Dungeon dungeon;

        Game game = new Game(5, 5, 50);

        [Test]
        public void NTest_check_ifValidGame()
        {
            Assert.IsTrue(game.dungeon.predicates.isValidDungeon(game.dungeon.startNode, game.dungeon.exitNode, 5));
        }

        [Test]
        public void NTest_checksZones()
        {
            //if not last zone, we have monsters to place still
            Zone testZone = new Zone(2, 10);
            //check if 2 != 5, we are not in last zone
            Assert.AreNotEqual(game.dungeon.difficultyLevel + 1, testZone.id,"test1");

            int total_monsterCount = 0;
            foreach (Zone z in game.dungeon.zones)
            {
                int monsterCount = 0;
                //if (game.dungeon.difficultyLevel + 1 != testZone.id)
                {
                    foreach(Node n in z.nodesInZone)
                    {
                        foreach (Pack p in n.packs)
                        {
                            foreach (Monster m in p.members)
                            {
                                monsterCount += 1;
                                total_monsterCount += 1;
								Logger.log("Monster in " + n.id);
                            }
                        }
                    }
                }
				Logger.log("in zone " + z.id);
				Logger.log("proportion is " + game.getProportion(50, z.id, 5));
				Logger.log(""+monsterCount);
				Logger.log(""+z.monstersInZone);
				Assert.AreEqual(monsterCount ,z.monstersInZone,"test2");
            }
            Assert.AreEqual(total_monsterCount, 50, "test3");
        }

        [Test]
        public void NTest_check_getProportion()
        {
            int test = game.getProportion(10, 5, 3);
            Assert.AreEqual(test, 5); // 100 % 20 == 5
        }

        [Test]
        public void NTest_check_getItemsHP()
        {
            int itemsHPTOTAL = 0;
            foreach (Item i in game.itemsToSeed) //for each item in the list
            {
                if (i.GetType() == typeof(HealingPotion)) //if item is a healing potion
                {
                    itemsHPTOTAL += (int)((HealingPotion)i).HPvalue; //Add its HP value
                }
            }
            Assert.IsTrue(itemsHPTOTAL == game.getItemsHP());

        }

        [Test]
        public void NTest_check_getHPM()
        {
            int totalMonsterHP = 0;
            foreach (Zone z in game.dungeon.zones) //for each zone
            {
                totalMonsterHP += z.getZoneHPValue(); //adds up zone HP values
            }
            Assert.IsTrue(totalMonsterHP == game.getHPM());
        }
    }
}