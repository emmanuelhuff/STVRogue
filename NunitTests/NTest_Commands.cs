using System;
using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;

namespace STVRogue.GameLogic
{
    [TestFixture()]
    public class NUnit_Commands
    {
		Command command = new Command(1);

        [Test]
        public void NTest_create_commandclass()
        {
			Assert.Equals(command.commandId,1);
        }

		[Test]
		public void NTest_check_stringreturn(){
			Assert.Equals(command.ToString(), "no-action");
		}
        
		[Test]
		public void NTest_check_userCommand(){
			Console.Write("Press 2");
			Assert.Equals(command.getUserCommand().commandId, 2);
		}
    }
}
