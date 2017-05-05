﻿//
//          Copyright Seth Hendrick 2016-2017.
// Distributed under the Boost Software License, Version 1.0.
//    (See accompanying file LICENSE_1_0.txt or copy at
//          http://www.boost.org/LICENSE_1_0.txt)
//

using System;
using ChaskisCore;
using Moq;
using NUnit.Framework;

namespace Tests
{
    [TestFixture]
    public class PingHandlerTest
    {
        // -------- Fields --------

        /// <summary>
        /// Unit under test.
        /// </summary>
        private PingHandler uut;

        /// <summary>
        /// Irc Config to use.
        /// </summary>
        private IrcConfig ircConfig;

        /// <summary>
        /// The mock IRC writer to use.
        /// </summary>
        private Mock<IIrcWriter> ircWriter;

        /// <summary>
        /// The user that joined.
        /// </summary>
        private const string RemoteUser = "remoteuser";

        // -------- Setup / Teardown --------

        [SetUp]
        public void TestSetup()
        {
            this.ircConfig = TestHelpers.GetTestIrcConfig();
            this.ircWriter = new Mock<IIrcWriter>( MockBehavior.Strict );

            this.uut = new PingHandler();
        }

        // -------- Tests --------

        /// <summary>
        /// Ensures that the class is created correctly.
        /// </summary>
        [Test]
        public void ConstructionTest()
        {
            // Keep Handling should be true by default.
            Assert.IsTrue( this.uut.KeepHandling );
        }

        /// <summary>
        /// Ensures everything that needs to throw argument null
        /// exceptions does.
        /// </summary>
        [Test]
        public void ArgumentNullTest()
        {
            Assert.Throws<ArgumentNullException>( () =>
                this.uut.HandleEvent( null, new IrcConfig(), this.ircWriter.Object )
            );

            Assert.Throws<ArgumentNullException>( () =>
                this.uut.HandleEvent( string.Empty, new IrcConfig(), this.ircWriter.Object )
            );

            Assert.Throws<ArgumentNullException>( () =>
                this.uut.HandleEvent( "hello world", null, this.ircWriter.Object )
            );

            Assert.Throws<ArgumentNullException>( () =>
                this.uut.HandleEvent( "hello world", new IrcConfig(), null )
            );
        }

        /// <summary>
        /// Ensures the bot PONGS the ping correctly.
        /// </summary>
        [Test]
        public void PongTest()
        {
            const string response = "12345"; // What we must pong back.

            // Expect us to send the response.
            this.ircWriter.Setup( i => i.SendPong( It.Is<string>( s => s == response ) ) );

            this.uut.HandleEvent(
                TestHelpers.ConstructPingString( response ),
                this.ircConfig,
                this.ircWriter.Object
            );
        }

        /// <summary>
        /// Ensures that if a PRIMSG appears, the PING
        /// event isn't fired.
        /// </summary>
        [Test]
        public void MessageCommandAppears()
        {
            string ircString =
                TestHelpers.ConstructIrcString(
                    RemoteUser,
                    MessageHandler.IrcCommand,
                    this.ircConfig.Channel,
                    "A message"
                );

            this.uut.HandleEvent( ircString, this.ircConfig, this.ircWriter.Object );
        }

        /// <summary>
        /// Ensures that if a PART appears, the PING
        /// event isn't fired.
        /// </summary>
        [Test]
        public void PartCommandAppears()
        {
            string ircString =
                TestHelpers.ConstructIrcString(
                    RemoteUser,
                    PartHandler.IrcCommand,
                    this.ircConfig.Channel,
                    string.Empty
                );

            this.uut.HandleEvent( ircString, this.ircConfig, this.ircWriter.Object );
        }

        /// <summary>
        /// Ensures that if a JOIN appears, the PING
        /// event isn't fired.
        /// </summary>
        [Test]
        public void JoinCommandAppears()
        {
            string ircString =
                TestHelpers.ConstructIrcString(
                    RemoteUser,
                    JoinHandler.IrcCommand,
                    this.ircConfig.Channel,
                    string.Empty
                );

            this.uut.HandleEvent( ircString, this.ircConfig, this.ircWriter.Object );
        }
    }
}