using System;
namespace STVRogue.Utils
{
    public class Test
    {
        public Test()
        {
        }

		public GameLogic.Game createGame(){
			return new GameLogic.Game(3, 4, 2);
		}

		public GameLogic.Dungeon createDungeon()
        {
			return new GameLogic.Dungeon(3, 4);   
        }




    }
}
