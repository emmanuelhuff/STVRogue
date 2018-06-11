using System;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using STVRogue.Utils;

namespace STVRogue.GameLogic
{
	[Serializable]
    public class GamePlay
    {
		BinaryFormatter formatter = new BinaryFormatter();

        public GamePlay()
        {
			
		}

		/* GamePlay(filename) load a saved game play from a file
            Use Binary deserialize
            Get Filename */
		public GamePlay(String s){
			Stream streamRead = new FileStream(s, FileMode.Open, FileAccess.Read, FileShare.None);
			GamePlay gamePlay = (GamePlay)formatter.Deserialize(streamRead);
			streamRead.Close();
		}
              
        //** ADDED **//
		/* save gameplay instance to file
                Use Binary Serialize
                needs : currentState */
		public void save(){
			GamePlay record = this;         
			Stream streamWrite = new FileStream("GamePlay.bin", FileMode.Create, FileAccess.Write,
												FileShare.None);
			formatter.Serialize(streamWrite, record);
			streamWrite.Close();
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

		public Boolean replay(Specification S){
			//replay the whole recoreded game play and test the given specificaiton S

			return true;
		}
    }

}
