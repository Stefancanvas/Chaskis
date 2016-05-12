﻿
//          Copyright Seth Hendrick 2016.
// Distributed under the Boost Software License, Version 1.0.
//    (See accompanying file ../../../../LICENSE_1_0.txt or copy at
//          http://www.boost.org/LICENSE_1_0.txt)

using System;
using System.IO;
using Chaskis.Plugins.CowSayBot;
using NUnit.Framework;

namespace Tests.Plugins.CowSayBot
{
    [TestFixture]
    public class CowSayBotConfigTest
    {
        /// <summary>
        /// Path to the fake cowsay executable.
        /// </summary>
        private static readonly string FakeExe = Path.Combine(
            TestHelpers.TestFilesDir, "CowSayBot", "cowsay"
        );

        /// <summary>
        /// Ensures a vaild CowSayBotConfig works.
        /// </summary>
        [Test]
        public void ValidateSuccessTest()
        {
            CowSayBotConfig uut = new CowSayBotConfig();
            uut.ListenRegex = @"^!{%saycmd%} (?<msg>.+)";
            uut.ExeCommand = FakeExe;
            uut.CowFileInfoList.Add( new CowFileInfo( "name", "command" ) );

            Assert.DoesNotThrow( () => uut.Validate() );
        }

        /// <summary>
        /// Ensures an invaild CowSayBotConfig with no
        /// listen regex fails.
        /// </summary>
        [Test]
        public void ValidateBadListenRegexTest()
        {
            CowSayBotConfig uut = new CowSayBotConfig();
            uut.ListenRegex = null;
            uut.ExeCommand = FakeExe;
            uut.CowFileInfoList.Add( new CowFileInfo( "name", "command" ) );

            Assert.Throws<InvalidOperationException>( () => uut.Validate() );

            uut.ListenRegex = string.Empty;
            Assert.Throws<InvalidOperationException>( () => uut.Validate() );

            uut.ListenRegex = "       ";
            Assert.Throws<InvalidOperationException>( () => uut.Validate() );
        }

        /// <summary>
        /// Ensures an invaild CowSayBotConfig with no
        /// exe command fails.
        /// </summary>
        [Test]
        public void ValidateBadExeCommandTest()
        {
            CowSayBotConfig uut = new CowSayBotConfig();
            uut.ListenRegex = @"^!{%saycmd%} (?<msg>.+)";
            uut.ExeCommand = null;
            uut.CowFileInfoList.Add( new CowFileInfo( "name", "command" ) );

            Assert.Throws<InvalidOperationException>( () => uut.Validate() );

            uut.ExeCommand = string.Empty;
            Assert.Throws<InvalidOperationException>( () => uut.Validate() );

            uut.ExeCommand = "       ";
            Assert.Throws<InvalidOperationException>( () => uut.Validate() );

            uut.ExeCommand = "doesNotExist";
            Assert.Throws<InvalidOperationException>( () => uut.Validate() );
        }

        /// <summary>
        /// Ensures an invaild CowFileInfoList with no
        /// or invalid entries command fails.
        /// </summary>
        [Test]
        public void ValidateBadCowFileInfoListTest()
        {
            CowSayBotConfig uut = new CowSayBotConfig();
            uut.ListenRegex = @"^!{%saycmd%} (?<msg>.+)";
            uut.ExeCommand = FakeExe;
            uut.CowFileInfoList.Clear();

            Assert.Throws<InvalidOperationException>( () => uut.Validate() );

            uut.CowFileInfoList.Add( new CowFileInfo( null, null ) );
            Assert.Throws<InvalidOperationException>( () => uut.Validate() );
        }

        /// <summary>
        /// Tests the clone method.
        /// </summary>
        [Test]
        public void CloneTest()
        {
            CowSayBotConfig uut1 = new CowSayBotConfig();
            uut1.ListenRegex = @"^!{%saycmd%} (?<msg>.+)";
            uut1.ExeCommand = FakeExe;
            uut1.CowFileInfoList.Add( new CowFileInfo( "name", "command" ) );

            CowSayBotConfig uut2 = uut1.Clone();

            Assert.AreNotSame( uut1.CowFileInfoList, uut2.CowFileInfoList );
            Assert.AreEqual( uut1.CowFileInfoList.Count, uut2.CowFileInfoList.Count );

            Assert.AreEqual( uut1.ListenRegex, uut2.ListenRegex );
            Assert.AreEqual( uut1.ExeCommand, uut2.ExeCommand );
        }
    }
}
