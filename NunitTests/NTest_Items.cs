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
        public void NTest_create_Item()
        {
			Item c = new Item();
			Assert.IsFalse(c.used);
        }
		[Test]
		public void NTest_create_healingPorion()
		{
			Item c = new Crystal("ruby");
			Assert.AreEqual(c.id, "ruby");
		}
		[Test]
        public void NTest_create_crystal()
        {
            Item h = new HealingPotion("pot1");
			Assert.AreEqual(h.id, "pot1");
        }
        [Test]
        public void NTest_use_item()
        {
            Player p = new Player();
            Item x = new HealingPotion("pot1");
			p.bag.Add(x);
            p.use(x);
            Assert.True(x.used);
        }
        [Test]
        public void NTest_use_healingPotionWithHalfHP()
        {
            Player p = new Player();
            Item h = new HealingPotion("pot1");
            p.HP = 50;
			p.bag.Add(h);
            p.use(h);
            Assert.True(p.HP > 50);
            Assert.False(p.HP < 51);

        }
        [Test]
        public void NTest_use_crystal()
        {
			Dungeon dungeon = new Dungeon(5, 6);
            Player p = new Player();
			p.dungeon = dungeon;

			List<Node> nodes = dungeon.shortestpath(dungeon.startNode, dungeon.exitNode);
			p.location = nodes[1];
			foreach(Node n in nodes){
				if(n is Bridge){
					p.location = n;
				}
			}

            Item c = new Crystal("ruby");
			p.bag.Add(c);
            p.use(c);
			Assert.IsTrue(p.accelerated);         
			Assert.AreSame(p.location, dungeon.startNode);

        }
    }
    
}
