using System;
using NUnit.Framework;
namespace STVRogue.GameLogic
{
	[TestFixture]
    public class NTest_Monster
    {
		Dungeon dungeon = new Dungeon(3, 4);
       
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
		[Test]
        public void NTest_attack_playerDies()
        {
            Dungeon dungeon = new Dungeon(5, 6);
            Player player = new Player(dungeon);
            player.HP = 1;
			Monster monster = new Monster("1");
			monster.Attack(player);
            Assert.AreEqual(player.HP, 0);
        }      
        [Test]
        public void NTest_attack_playerAlive()
        {
            Dungeon dungeon = new Dungeon(5, 6);
            Player player = new Player(dungeon);
            Monster monster = new Monster("1");
            player.HP = 5;
			monster.Attack(player);
            Assert.AreNotEqual(player.HP, 0);         
        }
    }
}
