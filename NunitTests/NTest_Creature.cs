using System;
using NUnit.Framework;
namespace STVRogue.GameLogic
{
    [TestFixture]
    public class NTest_Creature
    {

        [Test]
        public void NTest_createCreature_byMonster()
        {
			Creature creature = new Monster("1");
			Assert.Equals(creature.AttackRating, 1);
        }
              
    }
}
