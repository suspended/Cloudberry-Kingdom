﻿using System.Collections.Generic;
using Microsoft.Xna.Framework;

namespace CloudberryKingdom
{
    public class QuadSelect : GridSelect
    {
        List<QuadClass> Quads;

        public override void Release()
        {
            base.Release();

            foreach (QuadClass quad in Quads) quad.Release();
            Quads = null;
        }

        public QuadSelect() { }
        public QuadSelect(Vector2 Size, int Width, int Height)
        {
            Init(Size, Width, Height);
        }

        public override void Init(Vector2 Size, int Width, int Height)
        {
            base.Init(Size, Width, Height);

            Quads = new List<QuadClass>();
        }

        public void Add(QuadClass quad)
        {
            Add(quad, Quads.Count);
        }

        public void Add(QuadClass quad, object AssociatedObject)
        {
            Add();
            Quads.Add(quad);
            
            AssociatedObjects.Add(AssociatedObject);
        }

        /// <summary>
        /// The amount to scale the selected quad by
        /// </summary>
        public float SelectedScale = 2.3f;

        public override void DrawItem(int i, Vector2 pos, bool selected, int Layer)
        {
            QuadClass quad = Quads[i];

            quad.Pos = pos;

            if (!selected)
            {
                quad.Draw();
            }
            else
            {
                quad.Scale(SelectedScale);
                quad.Draw();
                quad.Scale(1f / SelectedScale);
                quad.Quad.Update(ref quad.Base);
            }
        }

#if PC_VERSION
        public override bool ItemHitTest(int i, Vector2 MousePos)
        {
            QuadClass quad = Quads[i];

            return quad.HitTest(MousePos);
        }

        public override void ItemMouseInteract(int i)
        {
            base.ItemMouseInteract(i);

            SetIndex(i);
        }
#endif
    }
}