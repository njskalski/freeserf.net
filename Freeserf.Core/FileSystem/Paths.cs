﻿/*
 * Pathes.cs - Default freeserf paths
 *
 * Copyright (C) 2019 Robert Schneckenhaus <robert.schneckenhaus@web.de>
 *
 * This file is part of freeserf.net.freeserf.net is based on freeserf.
 *
 * freeserf.net is free software: you can redistribute it and/or modify
 * it under the terms of the GNU General Public License as published by
 * the Free Software Foundation, either version 3 of the License, or
 * (at your option) any later version.
 *
 * freeserf.net is distributed in the hope that it will be useful,
 * but WITHOUT ANY WARRANTY; without even the implied warranty of
 * MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.See the
 * GNU General Public License for more details.
 *
 * You should have received a copy of the GNU General Public License
 * along with freeserf.net. If not, see<http://www.gnu.org/licenses/>.
 */

using System;
using System.IO;
using System.Reflection;

namespace Freeserf.FileSystem
{
    public static class Paths
    {
        public static readonly string UserConfigPath = "";
        public static readonly string SaveGameFolder = "";
        public static readonly string GameDataFolder = "";

        static Paths()
        {
#if WINDOWS
            SaveGameFolder = Path.Combine(Environment.GetEnvironmentVariable("USERPROFILE"), "Saved Games");
            UserConfigPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "freeserf.net", "user.cfg");
#elif __APPLE__
            SaveGameFolder = Environment.GetEnvironmentVariable("HOME");
            SaveGameFolder += "/Library/Application Support";
#else
            SaveGameFolder = Environment.GetEnvironmentVariable("HOME");
            SaveGameFolder += "/.local/share";
#endif

            SaveGameFolder += Path.DirectorySeparatorChar.ToString() +  "freeserf";

#if !WINDOWS
            UserConfigPath = SaveGameFolder + "/user.cfg";
            SaveGameFolder += "/saves";
#endif

            GameDataFolder = Path.GetDirectoryName(Assembly.GetEntryAssembly().Location);
        }
    }
}