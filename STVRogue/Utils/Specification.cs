using System;
using System.Collections.Generic;
using STVRogue.GameLogic;
using System.Linq;

namespace STVRogue.Utils
{
    public abstract class Specification
    {
		//state predicate
		private Predicate<Game> p;      
		public abstract bool test(Game G);
    }

	public class Always : Specification{
		private Predicate<Game> p;

		public Always(Predicate<Game> p){
			this.p = p;
		}
        
		public override bool test(Game G) => p(G);      

	}

	public class Unless : Specification
	{
		private Predicate<Game> p;
		private Predicate<Game> q;

		List<bool> history;
		bool verdict;

		public Unless(Predicate<Game> p, Predicate<Game> q)
        {
            this.p = p;
			this.q = q;
        }
        
		public override bool test(Game G)
		{
			if(history.Count >= 1){
				//check if p && !q holds on the previous state:
				bool previous = history.Last();
				//calculate wheter the previous and current state satisfy the unless property:
				verdict = !previous || (previous && (p(G)) || q(G));
			}else{
				
			}
			//push p && !q to the history:
			history.Add(p(G) && !q(G));
            //return the verdict
			return verdict;
		}
	}
}
