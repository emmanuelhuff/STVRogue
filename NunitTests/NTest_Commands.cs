﻿using System;
using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;
using STVRogue.Utils;

namespace STVRogue.GameLogic
{
    [TestFixture()]
    public class NUnit_Commands
    {
		Command command = new Command(1);

        [Test]
        public void NTest_create_commandclass()
        {
			Assert.AreEqual(command.commandId,1);
        }      
		[Test]
		public void NTest_check_stringreturn(){
			Assert.AreEqual(command.ToString(), "no-action");
		}
    }
}
