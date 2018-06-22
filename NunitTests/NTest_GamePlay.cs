using System;
using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;
using STVRogue.Utils;

namespace STVRogue.GameLogic
{
    [TestFixture()]
    public class NTest_GamePlay
    {
        [Test()]
        public void test_HPplayer_never_negative()
        {
			List<GamePlay> plays = new List<GamePlay>();
			//I can't find the location?
			plays.Add(new GamePlay("test.xml"));
			foreach(GamePlay gp in plays){
				//testing two "always" properties
				Assert.IsTrue(gp.replay(new Always(Game => Game.player.HP >= 0)));
				Assert.IsTrue(gp.replay(new Always(Game => Game.player.HP >= 0)));
			}
        }

		[Test()]
        public void test_HPplayer_never_negative_Unless()
        {
            List<GamePlay> plays = new List<GamePlay>();
            //I can't find the location?
            plays.Add(new GamePlay("test.xml"));
            foreach (GamePlay gp in plays)
            {
				for (var m = 1; m <= 100; m++){
					Assert.IsTrue(gp.replay(new Unless(Game => Game.activePack.members.Count == m, Game => Game.activePack.members.Count < m)));
				}
            }
        }
    }
}
