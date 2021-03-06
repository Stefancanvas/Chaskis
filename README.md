[![Icon](https://files.shendrick.net/projects/chaskis/assets/icon.png)](https://github.com/xforever1313/Chaskis/)
Chaskis 
==========
A generic framework written in C# for making IRC Bots.

Build Status
--------------
[![Build status](https://ci.appveyor.com/api/projects/status/n8sbo1ay6wr2xxyc/branch/master?svg=true)](https://ci.appveyor.com/project/xforever1313/chaskis/branch/master)
[![Codacy Badge](https://api.codacy.com/project/badge/Grade/92071784a63e4d6ba070cb88c1b6c99f)](https://www.codacy.com/app/xforever1313/Chaskis?utm_source=github.com&amp;utm_medium=referral&amp;utm_content=xforever1313/Chaskis&amp;utm_campaign=Badge_Grade)

Packages
--------------

[![NuGet](https://img.shields.io/nuget/v/ChaskisCore.svg)](https://www.nuget.org/packages/ChaskisCore/)
[![Chocolatey](https://img.shields.io/chocolatey/v/chaskis.svg)](https://chocolatey.org/packages/chaskis/)
[![AUR](https://img.shields.io/aur/version/chaskis.svg)](https://aur.archlinux.org/packages/chaskis/)

Docker Images
--------------

[![Windows](https://img.shields.io/docker/v/xforever1313/chaskis.windows?label=Windows&style=flat-square)](https://hub.docker.com/r/xforever1313/chaskis.windows)
[![Ubuntu](https://img.shields.io/docker/v/xforever1313/chaskis.ubuntu?label=Ubuntu&style=flat-square)](https://hub.docker.com/r/xforever1313/chaskis.ubuntu)
[![Raspbian](https://img.shields.io/docker/v/xforever1313/chaskis.raspbian?label=Raspbian&style=flat-square)](https://hub.docker.com/r/xforever1313/chaskis.raspbian)

About
--------
Chaskis is a framework for creating IRC Bots in an easy way.  It is a plugin-based architecture written in C# that can be run on Windows or Linux.  Users of the bot can add or remove plugins to run, or even write their own.

Chaskis is named after the [Chasqui](https://en.wikipedia.org/wiki/Chasqui), messengers who ran trails in the Inca Empire to deliver messages.

Live Demo
---------

You can talk to chaskisbot in our IRC channel on [Freenode](https://webchat.freenode.net/?channels=chaskis) (#chaskis @ irc.freenode.net).  Also, feel free to ask any questions there, someone will answer your questions as soon as possible.

You can also join our [Telegram Group](https://t.me/ChaskisIrc).  This is bridged with the same IRC channel that has chaskisbot in it, so you'll be able to talk with anyone in the IRC channel as well.

Install Instructions
----------------------

### Windows ###
Run the Windows [installer](https://files.shendrick.net/projects/chaskis/releases/latest/windows/ChaskisInstaller.msi).  This will install Chaskis to ```C:\Program Files\Chaskis```.  The service will also be installed but NOT enabled.

You can also install via [chocolatey](https://chocolatey.org/packages/chaskis/) by running ```choco install chaskis```.

### Linux ###

 * **Arch** - Install with the AUR: ```yaourt -S chaskis```.  There is a dependency of the dotnet runtime
 * **Debian** - [Install](https://docs.microsoft.com/en-us/dotnet/core/install/linux) an up-to-date version of the dotnet core runtime, and install the [.deb file](https://files.shendrick.net/projects/chaskis/releases/latest/debian/chaskis.deb).

Configuration
---------------
Once Chaskis is installed, run ```Chaskis.exe --bootstrap``` to create an empty configuration in side of your Application Data folder.  On Windows, this is ```C:\Users\you\AppData\Roaming\Chaskis```.  On Linux, this is ```/home/you/.config/Chaskis```.  If you wish to install a default config else where, specify that in the ```--chaskisroot argument``` (e.g. ```Chaskis.exe --bootstrap --chaskisroot=/home/you/chakisconfig```).

Note, if running Chaskis as a Service, you MUST store your user's configuration in the Application Data folder.

After running Chaskis.exe with the bootstrap argument, default configurations will appear in the folder.  They are XML Files, and their instructions live as comments in those files.  Plugin configuration lives in the Plugins folder.

Running
---------------
### Chaskis.exe ###

There are two ways to run Chaskis.  The first is with ```Chaskis.exe```. By default, this will look for configuration in your Application Data folder, but you can override this by passing in the ```--chaskisroot``` argument (e.g. ```Chaskis.exe --chaskisroot=/home/you/chakisconfig```).  You can run multiple instances of Chaskis.exe per user this way.  Running Chaskis in a tool such as tmux or screen an keep it running in the background.

### ChaskisService.exe ###

The other way to run Chaskis is by the service.  The advantage of a service is you can tell Chaskis to run when your system starts up.  The disadvantage is you can only have on configuration per user, which lives in the user's Application Data folder.

* [Windows Instructions](https://github.com/xforever1313/Chaskis/wiki/Running-as-a-Windows-Service)
* [Linux Instructions](https://github.com/xforever1313/Chaskis/wiki/Running-as-a-Linux-Service)
* [Docker Instructions](https://github.com/xforever1313/Chaskis/wiki/Running-with-Docker)

Writing Plugins
----------------

Visit our Wiki page [here](https://github.com/xforever1313/Chaskis/wiki/Writing-Plugins).
