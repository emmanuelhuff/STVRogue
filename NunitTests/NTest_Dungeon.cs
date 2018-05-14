using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace STVRogue.GameLogic
{
    [TestFixture()]
    public class NTest_Dungeon
	{

		[Test(Description = "Test function shortestpath() that the result is equal to expectation")]
		public void NTest_shortestpath()
		{
			Dungeon dungeon = new Dungeon(2, 12);
			List<Node> samplePath = new List<Node>();

			//Assert.Equals(dungeon.shortestpath(dungeon.startNode, dungeon.exitNode),);
		}

		[Test(Description ="Test function level() that the result is equal to expectation")]
        public void NTest_located_Level()
        {
			Dungeon dungeon = new Dungeon(2, 12);
			Node n1 = new Node("1");
			Assert.Equals(dungeon.level(n1), 1);
        }
              
		[Test(Description ="Test function disconnect() that Disconnection is fully completed")]
		public void NTest_complete_Disconnection()
		{
			Dungeon dungeon = new Dungeon(2, 12);
			//TO-DO: Check startNode is b
		}
        
		[Test(Description = "")]
        public void NTest_check_averageConnectivity()
        {
            Dungeon dungeon = new Dungeon(2, 12);
			//TO-DO: Check Average Connectivity is under 3.0
        }
    }
}
