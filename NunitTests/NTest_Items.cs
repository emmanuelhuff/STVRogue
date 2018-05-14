using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace STVRogue.GameLogic
{
    [TextFixture]
    public class NTest_Items
    {
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
            Player p = new Player();
            Item c = new Crystal("ruby");
            p.use(c);
            Assert.True(p.accelerated);
            if (p.location is Bridge) ;
        }
    }
    
}
