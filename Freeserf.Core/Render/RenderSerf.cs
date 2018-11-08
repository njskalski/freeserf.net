﻿/*
 * RenderSerf.cs - Handles serf rendering
 *
 * Copyright (C) 2018  Robert Schneckenhaus <robert.schneckenhaus@web.de>
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

namespace Freeserf.Render
{
    public class RenderSerf
    {
        Serf serf = null;
        ISprite sprite = null;

        public RenderSerf(Serf serf, ISpriteFactory spriteFactory)
        {
            this.serf = serf;

            Create(spriteFactory);
        }

        void Create(ISpriteFactory spriteFactory)
        {
            // TODO
            // sprite = spriteFactory.Create(...);
        }

        public void Delete()
        {
            sprite.Delete();
            sprite = null;
            serf = null;
        }

        public void Draw(Rect visibleMapArea)
        {
            if (sprite == null || serf == null)
                return;

            // TODO: if serf is outside the visible map area -> return
            // TODO: set position and texture coords based on:
            // serf.Position
            // serf.Animation
            // serf.Counter
        }
    }
}