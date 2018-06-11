using System;
namespace STVRogue.GameLogic
{
    public class GamePlay
    {
        public GamePlay()
        {
			//GamePlay(file) load a saved game play from a file
        }

		public void reset(){
			//reset the recorded game play back to turn 0.
		}

		public void replayTurn(){
			//replay the current turn in the recorded game play
            //game play to the next turn
		}

		public void getState(){
			//to get an instace of Game representing the game's state in the game play's current turn
		}

		public Boolean replay(){
			//replay(Specification S)
			//replay the whole recoreded game play and test the given specificaiton S

			return true;
		}
    }

}
