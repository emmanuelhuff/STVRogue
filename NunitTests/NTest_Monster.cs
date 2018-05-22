using System;
using NUnit.Framework;
namespace STVRogue.GameLogic
{
	[TestFixture]
    public class NTest_Monster
    {
       
		[Test]
		public void NTest_createMonster(){
			Monster monster = new Monster("1");
			Assert.LessOrEqual(monster.HP, 6);
			Assert.GreaterOrEqual(monster.HP, 0);
		}

		[Test]
        public void NTest_setHP_changesHP()
        {
            Monster monster = new Monster("1");
			int oldHP = monster.HP;
			monster.setHP(oldHP + 5);
			Assert.AreEqual(oldHP + 5, monster.HP);
        }

    }
}
