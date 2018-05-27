using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NUnit.Framework;

namespace STVRogue.GameLogic
{
    [TestFixture]
    public class NTest_Items
    {
		[Test]
		public void NTest_create_healingPorion()
		{
			Item c = new Crystal("ruby");
			Assert.Equals(c.id, "ruby");
		}
		[Test]
        public void NTest_create_crystal()
        {
            Item h = new HealingPotion("pot1");
            Assert.Equals(h.id, "pot1");
        }
        [Test]
        public void NTest_use_item()
        {
            Player P = new Player();
            Item x = new HealingPotion("pot1");
            P.use(x);
            Assert.True(x.used);
        }
        [Test]
        public void NTest_use_healingPotionWithHalfHP()
        {
            Player p = new Player();
            Item h = new HealingPotion("pot1");
            p.HP = 50;
            p.use(h);
            Assert.True(p.HP > 50);
            Assert.False(p.HP < 51);

        }
        [Test]
        public void NTest_use_crystal()
        {
			Dungeon dungeon = new Dungeon(5, 6);
            Player p = new Player();
            Item c = new Crystal("ruby");
            p.use(c);
			Assert.True(p.accelerated);
			if(p.location is Bridge)
			{
				Assert.True(p.location == dungeon.startNode);
			}
        }
    }
    
}
