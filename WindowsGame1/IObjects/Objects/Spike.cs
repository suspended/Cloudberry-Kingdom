﻿using System;
using System.Text;
using System.IO;

using Microsoft.Xna.Framework;

using Drawing;

using CloudberryKingdom.Levels;
using CloudberryKingdom.Bobs;

namespace CloudberryKingdom.Spikes
{
    public class Spike : ObjectBase
    {
        static bool PeakOut = true;

        public override void TextDraw() { }
        public override void Release()
        {
            //MyObject.Release();
            Core.Release();
        }

        public SimpleObject MyObject;        
        public AABox Box;

        public int Dir;
        public float Angle;

        public int Offset, UpT, DownT, WaitT1, WaitT2;

        public bool Exposed;

        public void SetPeriod(int Period)
        {
            Period -= 7 + 7;

            float Total = 20 + 40;

            //UpT = (int)(7 * Period / Total);
            WaitT1 = (int)(20 * Period / Total);
            //DownT = (int)(7 * Period / Total);
            WaitT2 = (int)(40 * Period / Total);
        }

        public override void MakeNew()
        {
            MyObject.Linear = true;

            UpT = 7;
            WaitT1 = 20;
            DownT = 7;
            WaitT2 = 40;

            SetDir(Prototypes.spike.Dir);

            Core.Init();
            Core.MyType = ObjectType.Spike;
            Core.ContinuousEnabled = true;
            Core.DrawLayer = 2;

            Core.GenData.NoBlockOverlap = true;
            Core.GenData.LimitGeneralDensity = true;

            Box.Initialize(Core.Data.Position, Prototypes.spike.MyObject.Boxes[0].Size() / 2);
            
            MyObject.Read(0, 0);
            MyObject.Update();
            UpdateObject();            

            Box.SetTarget(Core.Data.Position, Box.Current.Size);
            Box.SwapToCurrent();

            Core.WakeUpRequirements = true;
        }

        public Spike(bool BoxesOnly)
        {
            MyObject = new SimpleObject(Prototypes.spike.MyObject, BoxesOnly);

            Box = new AABox();

            MakeNew();

            Core.BoxesOnly = BoxesOnly;
        }

        public void SetDir(int dir)
        {
            Dir = dir;
            SetAngle(Dir * (float)Math.PI / 2);
        }

        public void SetAngle(float Ang)
        {
            Angle = Ang;
            Tools.PointxAxisTo(ref MyObject.Base.e1, ref MyObject.Base.e2, new Vector2((float)Math.Cos(Ang), (float)Math.Sin(Ang)));
        }


        public Spike(string file, EzEffectWad EffectWad, EzTextureWad TextureWad)
        {
            ObjectClass SourceObject;
            FileStream stream = File.Open(file, FileMode.Open, FileAccess.Read, FileShare.None);
            BinaryReader reader = new BinaryReader(stream, Encoding.UTF8);
            //SourceObject = new ObjectClass(Tools.QDrawer, Tools.Device, Tools.Device.PresentationParameters, 100, 100, EffectWad.FindByName("BasicEffect"), TextureWad.FindByName("White"));
            SourceObject = new ObjectClass(Tools.QDrawer, Tools.Device, EffectWad.FindByName("BasicEffect"), TextureWad.FindByName("White"));
            SourceObject.ReadFile(reader, EffectWad, TextureWad);
            reader.Close();
            stream.Close();

            SourceObject.ConvertForSimple();            
            MyObject = new SimpleObject(SourceObject);
            Vector2 size = InfoWad.GetVec("Spike_Size");
            MyObject.Base.e1 *= size.X;// *.125f;
            MyObject.Base.e2 *= size.Y;// *.125f;

            MyObject.Quads[1].Animated = false;
            
            MyObject.Read(0, 1);
            MyObject.Play = true;
            MyObject.EnqueueAnimation(0, 0, true);
            MyObject.DequeueTransfers();
            MyObject.Update();


            Core.Data.Position = new Vector2(0, 0);
            Core.Data.Velocity = new Vector2(0, 0);

            Box = new AABox(Core.Data.Position, MyObject.Boxes[0].Size() / 2);
        }

        public override void PhsxStep()
        {
            Core.PosFromParentOffset();

            //float PhsxCutoff = 1800;
            Vector2 PhsxCutoff = new Vector2(200, 200);
            if (Core.MyLevel.BoxesOnly) PhsxCutoff = new Vector2(-150, 200);
            if (!Core.MyLevel.MainCamera.OnScreen(Core.Data.Position, PhsxCutoff))
            {
                Core.SkippedPhsx = true;
                Core.WakeUpRequirements = true;
                return;
            }
            Core.SkippedPhsx = false;

            if (Core.WakeUpRequirements)
            {
                UpdateObject();
            }            

            AnimStep();
            UpdateObject();

            Box.Current.Center = MyObject.Boxes[0].Center();
            Box.Current.Size = Tools.Abs(MyObject.Boxes[0].Size()) / 2;
            Box.SetTarget(Box.Current.Center, Box.Current.Size + new Vector2(.0f, .02f));
            
            if (Core.WakeUpRequirements)
            {
                Box.SwapToCurrent();
                Core.WakeUpRequirements = false;
            }
        }

        public override void PhsxStep2()
        {
            if (Core.SkippedPhsx) return;

            Box.SetCurrent(Box.Current.Center, Box.Current.Size);
        }


        public void AnimStep() { AnimStep(Core.SkippedPhsx); }
        public void AnimStep(bool Skip)
        {
            if (Skip) return;

            Exposed = true;

            float PeakHeight;
            if (PeakOut) PeakHeight = .2f; 
            else PeakHeight = .01f;

            float AnimSpeed = 0;
            //float t = (float)Tools.Modulo(Core.GetPhsxStep() + Offset, UpT + DownT + WaitT1 + WaitT2);
            float t = (float)Tools.Modulo(Core.GetIndependentPhsxStep() + Offset, UpT + DownT + WaitT1 + WaitT2);
            if (t < UpT) MyObject.t = PeakHeight + (1 - PeakHeight) * t / (float)UpT;
            else if (t < UpT + WaitT1) MyObject.t = 1;
            else if (t < UpT + WaitT1 + DownT) MyObject.t = 1 + .9f * (t - UpT - WaitT1) / (float)DownT;
            else MyObject.t = 1.9f +.1f * (t - UpT - WaitT1 - DownT) / (float)WaitT2;

            if (!Core.BoxesOnly)
            {
                // Peak out before showing
                if (PeakOut)
                {
                    float PeakTime = 1.9655f;
                    float MaxPeakTime = 1.98f;
                    if (MyObject.t > PeakTime)
                    {
                        MyObject.t = PeakHeight * Math.Min(1f, (MyObject.t - PeakTime) / (MaxPeakTime - PeakTime));
                    }
                }
                // Shake before showing
                else
                {
                    if (MyObject.t > 1.9655f)
                    {
                        float HoldAngle = Angle;
                        SetAngle(Angle + MyLevel.Rnd.RndFloat(-.385f, .385f));
                        Angle = HoldAngle;
                    }
                    else
                        SetAngle(Angle);
                }
            }

            if (t < UpT / 3) Exposed = false;
            if (t > UpT + WaitT1 + .66f * DownT) Exposed = false;

            MyObject.PlayUpdate(AnimSpeed * 1000f / 60f / 150f);
        }

        public void UpdateObject()
        {
            if (MyObject != null)
            {
                MyObject.Base.Origin = Core.Data.Position;
                MyObject.Update();
            }
        }

        public override void Draw()
        {
            /*
            Vector2 BL = Box.Current.BL - new Vector2(50, 50); // MyQuad.BL();
            if (BL.X > Core.MyLevel.MainCamera.TR.X || BL.Y > Core.MyLevel.MainCamera.TR.Y)
                return;
            Vector2 TR = Box.Current.BL + new Vector2(50, 50); // MyQuad.TR();
            if (TR.X < Core.MyLevel.MainCamera.BL.X || TR.Y < Core.MyLevel.MainCamera.BL.Y)
                return;
            */
            if (Core.SkippedPhsx) return;

            if (Tools.DrawGraphics)
                MyObject.Draw(Tools.QDrawer, Tools.EffectWad);
            if (Tools.DrawBoxes)
            {
                Box.Draw(Tools.QDrawer, Color.Blue, 10);
            }
        }

        public override void Move(Vector2 shift)
        {
            Core.Data.Position += shift;

            Box.Move(shift);
            
            MyObject.Base.Origin += shift;
            MyObject.Update();
        }

        public override void Reset(bool BoxesOnly)
        {
            Core.Active = true;
        }

        public override void Interact(Bob bob)
        {
            if (!Core.SkippedPhsx && Exposed)
            {
                bool Col = Phsx.BoxBoxOverlap(bob.Box2, Box);
                if (Col)
                {
                    if (Core.MyLevel.PlayMode == 0)
                        bob.Die(Bob.BobDeathType.Spike, this);

                    if (Core.MyLevel.PlayMode == 1)
                    {
                        bool col = Phsx.BoxBoxOverlap_Tiered(Box, Core, bob, Spike_AutoGen.Instance);

                        if (col)
                            Core.Recycle.CollectObject(this);
                    }
                }
            }
        }

        public void CloneBoxObject(SimpleObject SimpleObjA, SimpleObject SimpleObjB)
        {
            SimpleObjA.Base = SimpleObjB.Base;
            for (int i = 0; i < SimpleObjA.Boxes.Length; i++)
            {
                SimpleObjA.Boxes[i].TR = SimpleObjB.Boxes[i].TR;
                SimpleObjA.Boxes[i].BL = SimpleObjB.Boxes[i].BL;
            }
        }

        public override void Clone(ObjectBase A)
        {
            Core.Clone(A.Core);
                        
            Spike SpikeA = A as Spike;

            SetDir(SpikeA.Dir);

            Angle = SpikeA.Angle;
            Offset = SpikeA.Offset;
            UpT = SpikeA.UpT;
            DownT = SpikeA.DownT;
            WaitT1 = SpikeA.WaitT1;
            WaitT2 = SpikeA.WaitT2;

            Core.WakeUpRequirements = true;
            UpdateObject();

            Exposed = SpikeA.Exposed;
        }

        public override void Write(BinaryWriter writer)
        {
            Core.Write(writer);
        }
        public override void Read(BinaryReader reader) { Core.Read(reader); }
//StubStubStubStart
public override void OnUsed() { }
public override void OnMarkedForDeletion() { }
public override void OnAttachedToBlock() { }
public override bool PermissionToUse() { return true; }
public Vector2 Pos { get { return Core.Data.Position; } set { Core.Data.Position = value; } }
public GameData Game { get { return Core.MyLevel.MyGame; } }
public override void Smash(Bob bob) { }
public override bool PreDecision(Bob bob) { return false; }
//StubStubStubEnd7
    }
}
