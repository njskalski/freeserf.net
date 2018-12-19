# freeserf.net
freeserf.net is a C# port of [freeserf](https://github.com/freeserf/freeserf).

At the moment there is still a lot of work to do.


## Download the game

[Latest build download](https://github.com/Pyrdacor/freeserf.net/raw/master/builds/Windows/Build%20v0.4.zip)

Note: For now you need the english DOS data file 'SPAE.PA' to run the game. It is not included in the zip file.


## Current State

Most of the code is ported or re-implemented. There is a working OpenTK renderer and a small sound engine for Windows. Sound for other platforms will follow later. The sound system is not perfect yet.

Things that are missing are mostly AI logic, game loading and fights.

There are still some bugs (mainly in serf logic and map position calculation).

But the game is playable for most parts.


## Roadmap

### Phase 1: Porting

The first step is to port everything from C++ to C# and ensure that the game runs.
There may be some quick&dirty implementations or things that could be done better.

### Phase 2: Optimizing

This includes bug fixing and C#-specific optimizations.
Moreover this includes performance and stability optimizations if needed.
Also the plan is to make everything cross-plattform as much as possible.

### Phase 3: Extending

This includes:

- New features
- Better usability
- Other things like mod support, tools and so on


## Future Goal

At the end this should become a stable and performant game that runs on many platforms and can be easily compiled and extended by .NET developers.

I am not sure how far this project will go as my time is very limited. I can not promise anything at this point.


## Implementation details

The core is implemented as a .NET Standard 2.0 DLL. The renderer is also a .NET Standard 2.0 DLL and uses OpenTK for rendering. The sound engine is only implemented for Windows at the moment and uses the WinMM.dll with its WAVE and MIDI functionality.

The main executable is a .NET Framework 4.6.1 project that depends on OpenTK and OpenTK.GLControl. It is easy to create another executable project with a different .NET version as the project only contains a Form, a GLControl and forwards input events.

You can even implement your own renderer if you want. There are a bunch of interfaces in the Freeserf.Render namespace inside the core project that you can use for that.

At the moment the sound engine is part of the renderer. This will change in the future. But you can implement your own sound engine independent of the renderer already if you want.
