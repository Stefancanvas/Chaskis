﻿//
//          Copyright Seth Hendrick 2017-2018.
// Distributed under the Boost Software License, Version 1.0.
//    (See accompanying file LICENSE_1_0.txt or copy at
//          http://www.boost.org/LICENSE_1_0.txt)
//

using System;
using System.Collections.Generic;
using Chaskis.UnitTests.Common;
using Chaskis.Core;
using NUnit.Framework;

namespace Chaskis.UnitTests.CoreTests.Handlers.All
{
    [TestFixture]
    public class ChaskisEventTest
    {
        // ---------------- Fields ----------------

        private ChaskisEventFactory factory;

        private string pluginName;
        private string targetPluginName;

        private Dictionary<string, string> expectedArgs;
        private Dictionary<string, string> expectedPassthroughArgs;

        // ---------------- Setup / Teardown ----------------

        [OneTimeSetUp]
        public void FixtureSetup()
        {
            this.factory = TestHelpers.CreateEventFactory();
            this.pluginName = TestHelpers.FactoryPluginNames[0];
            this.targetPluginName = TestHelpers.FactoryPluginNames[1];

            this.expectedArgs = new Dictionary<string, string>();
            this.expectedArgs.Add( "arg1", "arg1Value" );
            this.expectedArgs.Add( "arg2", "arg2Value" );

            this.expectedPassthroughArgs = new Dictionary<string, string>();
            this.expectedPassthroughArgs.Add( "earg1", "earg 1 value" );
            this.expectedPassthroughArgs.Add( "earg2", "earg 2 value" );
        }

        // ---------------- Tests ----------------

        /// <summary>
        /// Ensures that if we try to create multiple
        /// ChaskisEventFactories, we get an exception.
        /// </summary>
        [Test]
        public void DoubleInstanceFailure()
        {
            Assert.Throws<InvalidOperationException>(
                () => ChaskisEventFactory.CreateInstance( TestHelpers.FactoryPluginNames )
            );
        }

        /// <summary>
        /// Ensures that a targeted plugin event results in the correct string.
        /// </summary>
        [Test]
        public void CreateTargetedPluginEvent()
        {
            ChaskisEvent e = this.factory.EventCreators[pluginName].CreateTargetedEvent(
                this.targetPluginName,
                this.expectedArgs,
                this.expectedPassthroughArgs
            );

            string xmlString = e.ToString();
            Assert.IsFalse( xmlString.Contains( Environment.NewLine ) );

            ChaskisEvent recreatedEvent = ChaskisEvent.FromXml( xmlString );
            this.CompareEvents( e, recreatedEvent );
        }

        /// <summary>
        /// Ensures that a targeted plugin event results in the correct string.
        /// </summary>
        [Test]
        public void CreateTargetedPluginEventNoArgs()
        {
            ChaskisEvent e = this.factory.EventCreators[pluginName].CreateTargetedEvent(
                this.targetPluginName,
                new Dictionary<string, string>()
            );

            string xmlString = e.ToString();
            Assert.IsFalse( xmlString.Contains( Environment.NewLine ) );

            ChaskisEvent recreatedEvent = ChaskisEvent.FromXml( xmlString );
            this.CompareEvents( e, recreatedEvent );
        }

        /// <summary>
        /// Ensures a BCAST event results in the correct string.
        /// </summary>
        [Test]
        public void CreateBCastEvent()
        {
            ChaskisEvent e = this.factory.EventCreators[pluginName].CreateBcastEvent(
                this.expectedArgs,
                this.expectedPassthroughArgs
            );

            string xmlString = e.ToString();
            Assert.IsFalse( xmlString.Contains( Environment.NewLine ) );

            ChaskisEvent recreatedEvent = ChaskisEvent.FromXml( xmlString );
            this.CompareEvents( e, recreatedEvent );
        }

        /// <summary>
        /// Ensures a BCAST event with no args results in the correct string.
        /// </summary>
        [Test]
        public void CreateBCastEventNoArgs()
        {
            ChaskisEvent e = this.factory.EventCreators[pluginName].CreateBcastEvent(
                new Dictionary<string, string>()
            );

            string xmlString = e.ToString();
            Assert.IsFalse( xmlString.Contains( Environment.NewLine ) );

            ChaskisEvent recreatedEvent = ChaskisEvent.FromXml( xmlString );
            this.CompareEvents( e, recreatedEvent );
        }

        private void CompareEvents( ChaskisEvent expected, ChaskisEvent actual )
        {
            Assert.AreEqual( expected.SourceType, actual.SourceType );
            Assert.AreEqual( expected.SourcePlugin, actual.SourcePlugin );
            Assert.AreEqual( expected.DestinationPlugin, actual.DestinationPlugin );
            if( expected.Args == null )
            {
                Assert.IsNull( actual.Args );
            }
            else
            {
                Assert.AreEqual( expected.Args.Count, actual.Args.Count );
                foreach( KeyValuePair<string, string> arg in expected.Args )
                {
                    Assert.AreEqual( arg.Value, actual.Args[arg.Key] );
                }
            }

            if( expected.PassThroughArgs == null )
            {
                // FromXml will not have a null PassThroughArgs Dictionary.
                Assert.AreEqual( 0, actual.PassThroughArgs.Count );
            }
            else
            {
                Assert.AreEqual( expected.PassThroughArgs.Count, actual.PassThroughArgs.Count );
                foreach( KeyValuePair<string, string> arg in expected.PassThroughArgs )
                {
                    Assert.AreEqual( arg.Value, actual.PassThroughArgs[arg.Key] );
                }
            }
        }
    }
}
