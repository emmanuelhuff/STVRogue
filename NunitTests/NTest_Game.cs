﻿using NUnit.Framework;
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
        public RandomGenerator random;
        public Dungeon dungeon;

        Game game = new Game(5, 5, 50);

        [Test]
        public void NTest_check_ifValidGame()
        {
            Assert.IsTrue(game.dungeon.predicates.isValidDungeon(game.dungeon.startNode, game.dungeon.exitNode, 5));
        }

        [Test]
        public void NTest_check_ifLastZone()
        {
            Zone testZone = new Zone(5, 10);
            //check if 5 == 5, we are last zone 
            Assert.AreEqual(game.dungeon.difficultyLevel, testZone.id);

            foreach (Zone z in game.dungeon.zones)
            {
                if (game.dungeon.difficultyLevel != testZone.id)
                {

                }
            }


        }

        [Test]
        public void NTest_not_LastZone()
        {
            //if not last zone, we have monsters to place still
            Zone testZone = new Zone(1, 10);
            //check if 1 == 5, our code is not working
            Assert.AreNotEqual(game.dungeon.difficultyLevel, testZone.id);
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
            foreach (Zone z in this.dungeon.zones) //for each zone
            {
                totalMonsterHP += z.getZoneHPValue(); //adds up zone HP values
            }
            Assert.IsTrue(totalMonsterHP == game.getHPM());
        }
    }
}