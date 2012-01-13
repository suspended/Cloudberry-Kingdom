﻿using System.Collections.Generic;
using Microsoft.Xna.Framework;

using Drawing;

namespace CloudberryKingdom
{
    public class DrawPile : IViewable, IViewableList
    {
        public string[] GetViewables()
        {
            return new string[] { "BackdropShift" };
        }

        public void GetChildren(List<InstancePlusName> ViewableChildren)
        {
            if (MyQuadList != null)
                foreach (QuadClass quad in MyQuadList)
                {
                    string name = quad.Name;
                    if (name.Length == 0)
                        name = quad.TextureName;
                    ViewableChildren.Add(new InstancePlusName(quad, name));
                }

            if (MyTextList != null)
                foreach (EzText text in MyTextList)
                {
                    string name = text.MyString;
                    ViewableChildren.Add(new InstancePlusName(text, name));
                }
        }

        public FancyVector2 FancyScale;
        public Vector2 Size
        {
            get
            {
                return FancyScale.RelVal;
            }
            set
            {
                FancyScale.RelVal = value;
            }
        }

        public FancyVector2 FancyPos;
        public Vector2 Pos
        {
            get { return FancyPos.RelVal; }
            set { FancyPos.RelVal = value; }
        }

        public List<EzText> MyTextList = new List<EzText>();
        public List<QuadClass> MyQuadList = new List<QuadClass>();

        public PieceQuad Backdrop = null;

        public DrawPile()
        {
            FancyPos = new FancyVector2();

            FancyScale = new FancyVector2();
            Size = new Vector2(1, 1);
        }

        public DrawPile(FancyVector2 Center)
        {
            FancyPos = new FancyVector2(Center);

            FancyScale = new FancyVector2();
            Size = new Vector2(1, 1);
        }

        public void Clear()
        {
            MyTextList.Clear();
            MyQuadList.Clear();
        }

        public void Add(QuadClass quad)
        {
            quad.MakeFancyPos();
            quad.FancyPos.SetCenter(FancyPos, true);
            MyQuadList.Add(quad);
        }

        public void Insert(int index, QuadClass quad)
        {
            quad.MakeFancyPos();
            quad.FancyPos.SetCenter(FancyPos, true);
            MyQuadList.Insert(index, quad);
        }

        public void Add(EzText text)
        {
            text.MakeFancyPos();
            text.FancyPos.SetCenter(FancyPos, true);
            MyTextList.Add(text);
        }

        public void Remove(EzText text)
        {
            MyTextList.Remove(text);
        }

        public QuadClass FindQuad(string Name)
        {
            return QuadClass.FindQuad(MyQuadList, Name);
        }

        public float AlphaVel = 0;

        public FancyColor MyFancyColor = new FancyColor();
        public float Alpha
        {
            get { return MyFancyColor.A; }
            set { MyFancyColor.Color = new Color(255, 255, 255, value); }
        }

        public void Scale(float scale)
        {
            foreach (QuadClass quad in MyQuadList)
            {
                quad.Pos *= scale;
                //quad.ShadowOffset *= scale;
                quad.ShadowScale = 1f / scale;
                quad.Scale(scale);

                quad.ShadowOffset *= 18f * (scale - 1) + 1;

                Vector2 PosShift = .5f * (scale - 1) * quad.Size;
                Vector2 Hold = quad.ShadowOffset;
                quad.ShadowOffset += 1f * PosShift * new Vector2(1, 1.5f);
                quad.ShadowOffset.X = .5f * (quad.ShadowOffset.X + Hold.X - PosShift.X);
            }

            foreach (EzText text in MyTextList)
            {
                text.Pos *= scale;
                text.Scale *= scale;
            }
        }

        List<Vector2> SavedScales, SavedPositions, SavedShadowOffsets;
        public void SaveScale()
        {
            SavedScales = new List<Vector2>();

            foreach (QuadClass quad in MyQuadList)
                SavedScales.Add(quad.Size);

            SavedPositions = new List<Vector2>();

            foreach (QuadClass quad in MyQuadList)
                SavedPositions.Add(quad.Pos);

            SavedShadowOffsets = new List<Vector2>();

            foreach (QuadClass quad in MyQuadList)
                SavedShadowOffsets.Add(quad.ShadowOffset);
        }

        public void RevertScale()
        {
            foreach (QuadClass quad in MyQuadList)
            {
                quad.Size = SavedScales[MyQuadList.IndexOf(quad)];

                quad.Pos = SavedPositions[MyQuadList.IndexOf(quad)];

                quad.ShadowOffset = SavedShadowOffsets[MyQuadList.IndexOf(quad)];
                quad.ShadowScale = 1;
            }
        }

        public void SetBackdrop(Vector2 BL, Vector2 TR, PieceQuad Template)
        {
            if (Backdrop == null)
                Backdrop = new PieceQuad();

            Backdrop.Clone(Template);

            Vector2 Size = TR - BL;
            Vector2 Shift = (TR + BL) / 2;

            Backdrop.CalcQuads(Size / 2);
            BackdropShift = Shift;//TR + TR_Shift - Size / 2;
        }

        public Vector2 BackdropShift = Vector2.Zero;
        public void UpdateBackdrop(Vector2 TR_Shift, Vector2 BL_Shift)
        {
            if (Backdrop == null)
                Backdrop = new PieceQuad();

            //Backdrop.Clone(PieceQuad.Get("DullMenu");
            Backdrop.Clone(PieceQuad.SpeechBubble);

            Vector2 TR = new Vector2(-10000000, -10000000);
            Vector2 BL = new Vector2(10000000, 10000000);
            foreach (QuadClass quad in MyQuadList)
            {
                quad.Update();
                TR = Vector2.Max(TR, quad.TR);
                BL = Vector2.Min(BL, quad.BL);
            }

            foreach (EzText text in MyTextList)
            {
                text.CalcBounds();
                TR = Vector2.Max(TR, text.TR);
                BL = Vector2.Min(BL, text.BL);
            }

            Vector2 Size = TR - BL + TR_Shift - BL_Shift;
            Backdrop.CalcQuads(Size / 2);
            BackdropShift = TR + TR_Shift - Size / 2;
        }

        public void Update()
        {
            Alpha += AlphaVel;
        }

        public void Draw() { Draw(0); }
        public void Draw(int Layer)
        {
            if (Fading) Fade();

            if (FancyScale != null) FancyScale.Update();
            if (MyFancyColor != null) MyFancyColor.Update();

            if (Alpha <= 0) return;

            DrawNonText(Layer);

            DrawText(Layer);
            Tools.EndSpriteBatch();
        }

        public void DrawNonText(int Layer)
        {
            FancyPos.Update();

            if (Backdrop != null && Backdrop.Layer == Layer)
            {
                Backdrop.Base.Origin = FancyPos.AbsVal + BackdropShift;
                Backdrop.Draw();
            }

            foreach (QuadClass quad in MyQuadList)
            {
                if (quad.Layer == Layer)
                {
                    quad.ParentScaling = Size;
                    quad.ParentAlpha = Alpha;
                    quad.Draw();
                }
            }

            if (Layer == 0)
                foreach (EzText text in MyTextList) if (text.Backdrop != null) text.DrawBackdrop();
            Tools.QDrawer.Flush();
        }

        public void DrawText(int Layer)
        {
            foreach (EzText text in MyTextList)
            {
                if (text.Layer == Layer)
                {
                    text.ParentScaling = Size;
                    text.ParentAlpha = Alpha;
                    text.Draw(Tools.CurCamera, false);
                }
            }
        }

        public OscillateParams MyOscillateParams;
        public void Draw(bool Selected)
        {
            if (Selected)
            {
                SaveScale();
                Scale(MyOscillateParams.GetScale());
                Draw();
                RevertScale();
            }
            else
            {
                MyOscillateParams.Reset();
                Draw();
            }
        }

        public bool Fading = false;
        public float FadeSpeed = 0;
        public void FadeIn(float speed)
        {
            Alpha = 0;
            Fading = true;
            FadeSpeed = speed;
        }
        public void FadeOut(float speed)
        {
            Alpha = 1;
            Fading = true;
            FadeSpeed = -speed;
        }
        void Fade()
        {
            Alpha += FadeSpeed;
        }

        public void BubbleDownAndFade(bool sound)
        {
            BubbleDown(sound, 5);
            FadeOut(1f / 20f);
        }

        public static Vector2[] BubbleScale = { new Vector2(0.001f), new Vector2(1.15f), new Vector2(.94f), new Vector2(1.05f), new Vector2(1f) };
        public void BubbleUp(bool sound) { BubbleUp(sound, 5, 1); }
        public void BubbleUp(bool sound, int Length, float Intensity)
        {
            Vector2[] scales;
            if (Intensity == 1)
                scales = BubbleScale;
            else
                scales = BubbleScale.Map(v => (v - Vector2.One) * Intensity + Vector2.One);

            FancyScale.MultiLerp(Length, scales);
            MyFancyColor.LerpTo(new Vector4(1f, 1f, 1f, 1f), Length);
            if (sound)
                Tools.CurGameData.WaitThenDo(2, () =>
                    Tools.Pop(MyPopPitch));
        }
        /// <summary>
        /// The pitch of the pop noise when the draw pile is popped. Must be 1, 2, 3.
        /// </summary>
        public int MyPopPitch = 2;
        public void BubbleDown(bool sound) { BubbleDown(sound, 5); }
        public void BubbleDown(bool sound, int Length)
        {
            FancyScale.LerpTo(new Vector2(.1f), Length + 1);
            MyFancyColor.LerpTo(new Vector4(1f, 1f, 1f, 0f), Length);
            if (sound) Tools.SoundWad.FindByName("Pop 2").Play();
        }
        static Vector2[] JiggleScale = { new Vector2(1.15f), new Vector2(.94f), new Vector2(1.05f), new Vector2(1f) };
        public void Jiggle(bool sound) { Jiggle(sound, 5, 1f); }
        public void Jiggle(bool sound, int Length, float Intensity)
        {
            FancyScale.MultiLerp(Length, JiggleScale.Map(v => (v - Vector2.One) * Intensity + Vector2.One));
            MyFancyColor.LerpTo(new Vector4(1f, 1f, 1f, 1f), Length);
            if (sound)
                Tools.CurGameData.WaitThenDo(2, () =>
                    Tools.SoundWad.FindByName("Pop 2").Play());
        }
    }
}