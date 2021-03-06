﻿/*
 * BaseLineBuffer.cs - Buffer for shader baseline data
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

using System;
using OpenTK.Graphics.OpenGL;

namespace Freeserf.Renderer.OpenTK
{
    internal class BaseLineBuffer : BufferObject<ushort>
    {
        int index = 0;
        bool disposed = false;
        ushort[] buffer = null;
        readonly object bufferLock = new object();
        int size; // count of values
        readonly IndexPool indices = new IndexPool();
        bool changedSinceLastCreation = true;
        readonly BufferUsageHint usageHint = BufferUsageHint.DynamicDraw;

        public override int Size => size;

        public override VertexAttribIntegerType Type => VertexAttribIntegerType.Short;

        public override int Dimension => 1;

        public BaseLineBuffer(bool staticData)
        {
            index = GL.GenBuffer();

            if (staticData)
                usageHint = BufferUsageHint.StaticDraw;
        }

        public int Add(ushort baseLine, int index = -1)
        {
            bool reused = true;

            if (index == -1)
                index = indices.AssignNextFreeIndex(out reused);
            else
                reused = indices.AssignIndex(index);

            if (buffer == null)
            {
                buffer = new ushort[128];
                buffer[0] = baseLine;
                size = 1;
                changedSinceLastCreation = true;
            }
            else
            {
                if (index == buffer.Length) // we need to recreate the buffer
                {
                    if (buffer.Length < 512)
                        Array.Resize(ref buffer, buffer.Length + 128);
                    else if (buffer.Length < 2048)
                        Array.Resize(ref buffer, buffer.Length + 256);
                    else
                        Array.Resize(ref buffer, buffer.Length + 512);

                    changedSinceLastCreation = true;
                }

                if (!reused)
                    ++size;

                if (buffer[index] != baseLine)
                {
                    buffer[index] = baseLine;

                    changedSinceLastCreation = true;
                }
            }

            return index;
        }

        public void Update(int index, ushort baseLine)
        {
            if (buffer[index] != baseLine)
            {
                buffer[index] = baseLine;

                changedSinceLastCreation = true;
            }
        }

        public void Remove(int index)
        {
            indices.UnassignIndex(index);
        }

        public void ReduceSizeTo(int size)
        {
            this.size = size;
        }

        public override void Dispose()
        {
            Dispose(true);
        }

        void Dispose(bool disposing)
        {
            if (!disposed)
            {
                if (disposing)
                {
                    GL.BindBuffer(BufferTarget.ArrayBuffer, 0);

                    if (index != 0)
                    {
                        GL.DeleteBuffer(index);

                        if (buffer != null)
                        {
                            lock (bufferLock)
                            {
                                buffer = null;
                            }
                        }

                        size = 0;
                        index = 0;
                    }

                    disposed = true;
                }
            }
        }

        public override void Bind()
        {
            if (disposed)
                throw new Exception("Tried to bind a disposed buffer.");

            GL.BindBuffer(BufferTarget.ArrayBuffer, index);

            Recreate(); // ensure that the data is up to date
        }

        void Recreate() // is only called when the buffer is bound (see Bind())
        {
            if (!changedSinceLastCreation || buffer == null)
                return;

            lock (bufferLock)
            {
                GL.BufferData(BufferTarget.ArrayBuffer, Size * sizeof(ushort),
                    buffer, usageHint);
            }

            changedSinceLastCreation = false;
        }

        internal override bool RecreateUnbound()
        {
            if (!changedSinceLastCreation || buffer == null)
                return false;

            if (disposed)
                throw new Exception("Tried to recreate a disposed buffer.");

            GL.BindBuffer(BufferTarget.ArrayBuffer, index);

            lock (bufferLock)
            {
                GL.BufferData(BufferTarget.ArrayBuffer, Size * sizeof(ushort),
                    buffer, usageHint);
            }

            changedSinceLastCreation = false;

            return true;
        }
    }
}
