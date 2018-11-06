﻿/*
 * Gui.cs - Base functions for the GUI hierarchy
 *
 * Copyright (C) 2012  Jon Lund Steffensen <jonlst@gmail.com>
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
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Freeserf
{
    public abstract class GuiObject
    {
        readonly List<GuiObject> floatWindows = new List<GuiObject>();
        bool redraw = true;
        IFrame frame = null;
        static GuiObject FocusedObject = null;
        bool focused = false;
        protected bool displayed = false;

        public int X { get; private set; } = 0;
        public int Y { get; private set; } = 0;
        public uint Width { get; private set; } = 0;
        public uint Height { get; private set; } = 0;
        public bool Enabled { get; set; } = true;
        public bool Displayed
        {
            get => displayed;
            set
            {
                if (displayed != value)
                {
                    displayed = value;

                    SetRedraw();
                }
            }
        }
        public GuiObject Parent { get; set; } = null;

        protected abstract void InternalDraw();

        protected virtual void Layout()
        {
            // empty
        }

        void DeleteFrame()
        {
            frame = Graphics.Instance.RecreateFrame(frame, Width, Height);
        }

        protected virtual bool HandleClickLeft(int x, int y)
        {
            return false;
        }

        protected virtual bool HandleDoubleClick(int x, int y, Event.Button button)
        {
            return false;
        }

        protected virtual bool HandleDrag(int dx, int dy)
        {
            return true;
        }

        protected virtual bool HandleKeyPressed(char key, int modifier)
        {
            return false;
        }

        protected virtual bool HandleFocusLoose()
        {
            return false;
        }

        public void Draw(IFrame frame)
        {
            if (!Displayed)
            {
                return;
            }

            if (this.frame == null)
            {
                this.frame = Graphics.Instance.CreateFrame(Width, Height);
            }

            if (redraw)
            {
                InternalDraw();

                foreach (GuiObject floatWindow in floatWindows)
                {
                    floatWindow.Draw(this.frame);
                }

                redraw = false;
            }

            frame.DrawFrame(X, Y, 0, 0, this.frame, Width, Height);
        }

        public void MoveTo(int x, int y)
        {
            X = x;
            Y = y;
            SetRedraw();
        }

        public void SetSize(uint width, uint height)
        {
            Width = width;
            Height = height;
            Layout();
            SetRedraw();
        }

        public void SetRedraw()
        {
            redraw = true;

            if (Parent != null)
                Parent.SetRedraw();
        }

        public bool PointInside(int pointX, int pointY)
        {
            return pointX >= X && pointY >= Y &&
                   pointX < X + Width && pointY < Y + Height;
        }

        public void AddFloatWindow(GuiObject obj, int x, int y)
        {
            obj.Parent = this;
            floatWindows.Add(obj);
            obj.MoveTo(x, y); // will call SetRedraw
        }

        public void DeleteFloatWindow(GuiObject obj)
        {
            obj.Parent = null;
            floatWindows.Remove(obj);
            SetRedraw();
        }

        public void SetFocused()
        {
            if (FocusedObject != this)
            {
                if (FocusedObject != null)
                {
                    FocusedObject.focused = false;
                    FocusedObject.HandleFocusLoose();
                    FocusedObject.SetRedraw();
                }

                focused = true;
                FocusedObject = this;
                SetRedraw();
            }
        }

        public void PlaySound(Audio.TypeSfx sound)
        {
            Audio audio = Audio.Instance;
            Audio.Player player = audio.GetSoundPlayer();

            if (player != null)
            {
                player.PlayTrack((int)sound);
            }
        }

        public static int GuiGetSliderClickValue(int x)
        {
            return 1310 * Misc.Clamp(0, x - 7, 50);
        }

        public virtual bool HandleEvent(Event.EventArgs e)
        {
            if (!Enabled || !Displayed)
            {
                return false;
            }

            int eventX = e.X;
            int eventY = e.Y;
          
            if (e.Type == Event.Type.Click ||
                e.Type == Event.Type.DoubleClick ||
                e.Type == Event.Type.Drag)
            {
                eventX = e.X - X;
                eventY = e.Y - Y;

                if (eventX < 0 || eventY < 0 || eventX > Width || eventY > Height)
                {
                    return false;
                }
            }

            Event.EventArgs internalEvent = new Event.EventArgs(
                e.Type, eventX, eventY, e.Dx, e.Dy, e.Button);

            /* Find the corresponding float element if any */
            foreach (var floatWindow in floatWindows)
            {
                if (floatWindow.HandleEvent(internalEvent))
                {
                    return true;
                }
            }

            bool result = false;

            switch (e.Type)
            {
                case Event.Type.Click:
                    if (e.Button == Event.Button.Left)
                        result = HandleClickLeft(eventX, eventY);
                    break;
                case Event.Type.Drag:
                    result = HandleDrag(e.Dx, e.Dy);
                    break;
                case Event.Type.DoubleClick:
                    result = HandleDoubleClick(e.X, e.Y, e.Button);
                    break;
                case Event.Type.KeyPressed:
                    result = HandleKeyPressed((char)e.Dx, e.Dy);
                    break;
                default:
                  break;
            }

            if (result && FocusedObject != this)
            {
                if (FocusedObject != null)
                {
                    FocusedObject.focused = false;
                    FocusedObject.HandleFocusLoose();
                    FocusedObject.SetRedraw();
                    FocusedObject = null;
                }
            }

            return result;
        }
    }
}