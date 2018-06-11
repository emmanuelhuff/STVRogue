using System;
using STVRogue.GameLogic;

namespace STVRogue.Utils
{
    public class Specification
    {
        public Specification()
        {
			
        }
		public bool test(Game G){
			return G.player.HP >= 0;
		}
    }
}
