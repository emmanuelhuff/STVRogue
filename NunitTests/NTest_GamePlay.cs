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
			plays.Add(new GamePlay("test.xml"));//I can't find the location?
			foreach(GamePlay gp in plays){
				//testing two "always" properties
				Assert.IsTrue(gp.replay(new Always(Game => Game.player.HP >= 0)));
				Assert.IsTrue(gp.replay(new Always(Game => Game.player.HP >= 0)));
			}
        }
    }
}
