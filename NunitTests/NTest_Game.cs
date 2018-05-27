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
        Game game = new Game(5, 5, 50);
        [Test]
        public void NTest_check_ifValidGame()
        {
            Assert.IsTrue(game.predicates.isValidDungeon(game.dungeon.startNode, game.dungeon.exitNode, 5));
        }

        [Test]
        public void NTest_check_ifLastZone()
        {
            Zone testZone = new Zone(5, 10);
            //check if 5 == 5, we are last zone 
            Assert.AreEqual(game.dungeon.difficultyLevel, testZone.id);
        }

        [Test]
        public void NTest_not_LastZone()
        {
            //if not last zone, we have monsters to place still
            Zone testZone = new Zone(5, 10);
            //check if 5 == 5, we are last zone 
            Assert.AreEqual(game.dungeon.difficultyLevel, testZone.id);

            Assert.IsFalse(testZone.level, game.dungeon.difficultyLevel);

        }

        [Test]
        public void NTest_check_ifLastZone()
        {
            Zone testZone = new Zone(5, 10);
            //check if 5 == 5, we are last zone 
            Assert.AreEqual(game.dungeon.difficultyLevel, testZone.id);

            Assert.IsFalse(testZone.level, game.dungeon.difficultyLevel);

        }
    }
}
