﻿/*
 * IServer.cs - Interface for network servers
 *
 * Copyright (C) 2019  Robert Schneckenhaus <robert.schneckenhaus@web.de>
 *
 * This file is part of freeserf.net. freeserf.net is based on freeserf.
 *
 * freeserf.net is free software: you can redistribute it and/or modify
 * it under the terms of the GNU General Public License as published by
 * the Free Software Foundation, either version 3 of the License, or
 * (at your option) any later version.
 *
 * freeserf.net is distributed in the hope that it will be useful,
 * but WITHOUT ANY WARRANTY; without even the implied warranty of
 * MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
 * GNU General Public License for more details.
 *
 * You should have received a copy of the GNU General Public License
 * along with freeserf.net. If not, see <http://www.gnu.org/licenses/>.
 */

using System;
using System.Collections.Generic;
using System.Net;

namespace Freeserf.Network
{
    public partial class Global
    {
        public const int NetworkPort = 5067;
    }

    public enum ServerState
    {
        Offline,
        Lobby,
        Loading,
        Game,
        Outro
    }

    public interface IServer
    {
        string Name { get; }
        IPAddress Ip { get; }
        ServerState State { get; }
    }

    public interface ILocalServer : IServer
    {
        void Init(bool useServerValues, bool useSameValues, string mapSeed);
        void Close();

        List<IRemoteClient> Clients { get; }
        bool AcceptClients { get; set; }
    }

    public interface IRemoteServer : IServer, IRemote
    {

    }

    public interface IServerFactory
    {
        ILocalServer CreateLocal(string name, GameInfo gameInfo);
    }
}
