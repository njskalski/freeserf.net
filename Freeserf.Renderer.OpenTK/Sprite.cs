﻿/*
 * Sprite.cs - Textured sprite
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

using Freeserf.Render;

namespace Freeserf.Renderer.OpenTK
{
    /// <summary>
    /// A sprite has a fixed size and an offset into the layer's texture atlas.
    /// The layer will sort sprites by size and then by the texture atlas offset.
    /// </summary>
    public class Sprite : Node, ISprite
    {
        protected int drawIndex = -1;
        Position textureAtlasOffset = null;

        public Sprite(int width, int height, int textureAtlasX, int textureAtlasY, Rect virtualScreen)
            : base(Shape.Rect, width, height, virtualScreen)
        {
            textureAtlasOffset = new Position(textureAtlasX, textureAtlasY);
        }

        public Position TextureAtlasOffset
        {
            get => textureAtlasOffset;
            set
            {
                if (textureAtlasOffset == value)
                    return;

                textureAtlasOffset = new Position(value);

                UpdateTextureAtlasOffset();
            }
        }

        protected override void AddToLayer()
        {
            drawIndex = (Layer as RenderLayer).GetDrawIndex(this);
        }

        protected override void RemoveFromLayer()
        {
            (Layer as RenderLayer).FreeDrawIndex(drawIndex);
            drawIndex = -1;
        }

        protected override void UpdatePosition()
        {
            if (drawIndex != -1) // -1 means not attached to a layer
                (Layer as RenderLayer).UpdatePosition(drawIndex, this);
        }

        protected virtual void UpdateTextureAtlasOffset()
        {
            if (drawIndex != -1) // -1 means not attached to a layer
                (Layer as RenderLayer).UpdateTextureAtlasOffset(drawIndex, this);
        }
    }

    public class MaskedSprite : Node, IMaskedSprite
    {
        protected int drawIndex = -1;
        Position textureAtlasOffset = null;
        Position maskTextureAtlasOffset = null;

        public MaskedSprite(int width, int height, int textureAtlasX, int textureAtlasY, Rect virtualScreen)
            : base(Shape.Rect, width, height, virtualScreen)
        {
            textureAtlasOffset = new Position(textureAtlasX, textureAtlasY);
            maskTextureAtlasOffset = new Position(textureAtlasX, textureAtlasY);
        }

        public Position TextureAtlasOffset
        {
            get => textureAtlasOffset;
            set
            {
                if (textureAtlasOffset == value)
                    return;

                textureAtlasOffset = new Position(value);

                UpdateTextureAtlasOffset();
            }
        }

        public Position MaskTextureAtlasOffset
        {
            get => maskTextureAtlasOffset;
            set
            {
                if (maskTextureAtlasOffset == value)
                    return;

                maskTextureAtlasOffset = new Position(value);

                UpdateTextureAtlasOffset();
            }
        }

        public override void Resize(int width, int height)
        {
            if (Width == width && Height == height)
                return;

            base.Resize(width, height);

            UpdatePosition();
            UpdateTextureAtlasOffset();
        }

        protected override void AddToLayer()
        {
            drawIndex = (Layer as RenderLayer).GetDrawIndex(this, maskTextureAtlasOffset);
        }

        protected override void RemoveFromLayer()
        {
            (Layer as RenderLayer).FreeDrawIndex(drawIndex);
            drawIndex = -1;
        }

        protected override void UpdatePosition()
        {
            if (drawIndex != -1) // -1 means not attached to a layer
                (Layer as RenderLayer).UpdatePosition(drawIndex, this);
        }

        protected virtual void UpdateTextureAtlasOffset()
        {
            if (drawIndex != -1) // -1 means not attached to a layer
                (Layer as RenderLayer).UpdateTextureAtlasOffset(drawIndex, this, maskTextureAtlasOffset);
        }
    }

    public class SpriteFactory : ISpriteFactory
    {
        readonly Rect virtualScreen = null;

        public SpriteFactory(Rect virtualScreen)
        {
            this.virtualScreen = virtualScreen;
        }

        public ISprite Create(int width, int height, int textureAtlasX, int textureAtlasY, bool masked)
        {
            if (masked)
                return new MaskedSprite(width, height, textureAtlasX, textureAtlasY, virtualScreen);
            else
                return new Sprite(width, height, textureAtlasX, textureAtlasY, virtualScreen);
        }

        public ISprite CreateScaled(int width, int height, int scaledWidth, int scaledHeight, int textureAtlasX, int textureAtlasY, bool masked)
        {
            var sprite = Create(width, height, textureAtlasX, textureAtlasY, masked);

            (sprite as Sprite).ScaleX = (float)scaledWidth / (float)width;
            (sprite as Sprite).ScaleY = (float)scaledHeight / (float)height;

            return sprite;
        }
    }
}
