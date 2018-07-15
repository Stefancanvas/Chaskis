﻿//
//          Copyright Seth Hendrick 2018.
// Distributed under the Boost Software License, Version 1.0.
//    (See accompanying file LICENSE_1_0.txt or copy at
//          http://www.boost.org/LICENSE_1_0.txt)
//

using System;
using System.Collections.Generic;
using System.IO;
using ChaskisCore;
using SethCS.Basic;

namespace Chaskis.Plugins.IsItDownBot
{
    [ChaskisPlugin( PluginName )]
    public class IsItDownBot : IPlugin
    {
        // ---------------- Fields ----------------

        public const string VersionStr = "0.1.0";

        public const string PluginName = "isitdownbot";

        private IsItDownBotConfig config;

        private GenericLogger logger;

        private List<IIrcHandler> ircHandlers;

        // ---------------- Constructor ----------------

        public IsItDownBot()
        {
            this.ircHandlers = new List<IIrcHandler>();
        }

        // ---------------- Properties ----------------

        public string SourceCodeLocation
        {
            get
            {
                return "https://github.com/xforever1313/Chaskis/tree/master/Chaskis/Plugins/IsItDownBot";
            }
        }

        public string Version
        {
            get
            {
                return VersionStr;
            }
        }

        public string About
        {
            get
            {
                return "I check to see if a website is down for you.";
            }
        }

        // ---------------- Functions ----------------

        public void Init( PluginInitor initor )
        {
            this.logger = initor.Log;

            string pluginDir = Path.Combine(
                initor.ChaskisConfigPluginRoot,
                "IsItDownBot"
            );

            string configPath = Path.Combine(
                pluginDir,
                "IsItDownBotConfig.xml"
            );

            this.config = XmlLoader.LoadConfig( configPath );
        }

        public void HandleHelp( IIrcWriter writer, IrcResponse response, string[] args )
        {
            writer.SendMessage(
                "Usage: " + this.config.CommandPrefix + " https://url",
                response.Channel
            );
        }

        public IList<IIrcHandler> GetHandlers()
        {
            return this.ircHandlers.AsReadOnly();
        }

        public void Dispose()
        {
        }
    }
}