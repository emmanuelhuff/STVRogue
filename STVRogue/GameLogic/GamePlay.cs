using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Runtime.Serialization;
using System.Xml;
using STVRogue.Utils;


namespace STVRogue.GameLogic
{

    public class GamePlay
    {
        //
        public Game game;
        private static DataContractSerializer serializer;
      
		public GamePlay(){
			
		}
        public GamePlay(string fileName)
        {
            //GamePlay(file) load a saved game play from a file
            DataContractSerializer s = new DataContractSerializer(typeof(Game));
            FileStream fs = File.Open(fileName, FileMode.Open);
            object s2 = s.ReadObject(fs);
            if (s2 == null)
                Console.WriteLine("  Deserialized object is null (Nothing in VB)");
            else
                Console.WriteLine("  Deserialized type: {0}", s2.GetType());

            this.game = (Game)s2;
            // We loaded the initial game, now we have to load all the player commands
            // that were executed in the previous game.  They were saved in the
            // previous game so
            this.game.player.replayInput = new Queue<Command>(this.game.player.saveInputForReplay);
            this.game.player.saveInputForReplay = new Queue<Command>(); //resets 
        }

        public void saveInitialGame(Game game)
        {
            serializer = new DataContractSerializer(game.GetType(), null,
                           0x7FFF /*maxItemsInObjectGraph*/,
                           false /*ignoreExtensionDataObject*/,
                           true /*preserveObjectReferences : This handles when multiple variables contain the same instance of an object */,
                           null /*dataContractSurrogate*/);
            game.initialGame = new MemoryStream();
            serializer.WriteObject(game.initialGame, game);
        }

        public void saveToFile(string fileName, Game game)
        {

            FileStream fs = File.Open(fileName, FileMode.Create);
            serializer.WriteObject(Console.OpenStandardOutput(), game);

            // Put this next somewhere to save off the initial state of the
            // game before any moves have been made
            //serializer.WriteObject(fs, game);

            //This actually writes out to the file
            //serializer.Seek(0, SeekOrigin.Begin);
            //serializer.CopyTo(fs);
            serializer.WriteObject(fs, game);
            fs.Close();
            //writer.Flush();
        }

        public void reset()
        {
            //reset the recorded game play back to turn 0.
            game.initialGame.Position = 0;
            //Deserialize the Record object back into a new record object. 
            Game tempGame = (Game)serializer.ReadObject(this.game.initialGame);
            tempGame.player.replayInput = new Queue<Command>(this.game.player.saveInputForReplay);
            tempGame.player.saveInputForReplay = new Queue<Command>();
            this.game = tempGame;
        }


		public void replayTurn()
        {
			//replay the current turn in the recorded game play         
			Zone curZone = new Zone(game.player.location.level, game.dungeon.M);
			game.update(game.player.getNextCommand(),game.turn,curZone);         
        }

        public Game getState()
        {
            //to get an instace of Game representing the game's state in the game play's current turn
            return this.game;
        }

        //**ADDED**//
		public bool hasNextTurn(){
            //find that there is next Turn
			if(game.player.replayInput.Count == 0){
				return false;
			}
			return true;
		}

        public Boolean replay(Specification S)
        {
			//replay the whole recoreded game play and test the given specificaiton S
			reset();
			while(true)
			{
				bool ok = S.test(getState());
				if(ok){
					if(hasNextTurn()){
						replayTurn();
					}
					else break;               
				}
				else{
					return false;
				}
			}         
            return true;
        }
    }
}