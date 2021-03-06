﻿using Microsoft.Xna.Framework;

using CoreEngine;
using CloudberryKingdom.Bobs;

namespace CloudberryKingdom
{
    public class BobPhsxBox : BobPhsxNormal
    {
        // Singleton
        protected override void InitSingleton()
        {
            base.InitSingleton();

            Specification = new HeroSpec(1, 0, 0, 0);
            Name = Localization.Words.HeroInABox;
            NameTemplate = "hero in a box";
            
            //Icon = new PictureIcon(Tools.TextureWad.FindByName("HeroIcon_Box"), Color.White, DefaultIconWidth * 1.125f);
            Icon = new PictureIcon(Tools.TextureWad.FindByName("Bob_Box_Duck_0000"), Color.White, DefaultIconWidth * 1.35f);
            ((PictureIcon)Icon).IconQuad.Quad.Shift(new Vector2(0, .0485f));
        }
        static readonly BobPhsxBox instance = new BobPhsxBox();
        public static new BobPhsxBox Instance { get { return instance; } }

        // Instancable class
        bool InitializedAnim;

        public BobPhsxBox()
        {
            LandSound = Tools.SoundWad.FindByName("BoxHero_Land");

            DefaultValues();
        }

        public override void DefaultValues()
        {
 	        base.DefaultValues();

            BobJumpAccel = (Gravity + 3.45f) / 19;
            BobInitialJumpSpeed = 6f;
            BobInitialJumpSpeedDucking = 6f;
            BobJumpLength = 18;
            BobJumpLengthDucking = 17;

            BobJumpAccel2 = (Gravity + 3.42f) / 19;
            BobInitialJumpSpeed2 = 14f;
            BobInitialJumpSpeedDucking2 = 12f;
            BobJumpLength2 = 17;
            BobJumpLengthDucking2 = 17;

            MaxSpeed = 13.6f;// 15f;
            XAccel = .45f;
            XFriction = .85f;
            BobXDunkFriction =
                        .65f;
        }

        public override void Init(Bob bob)
        {
            base.Init(bob);

            MyBob.JumpSound = Tools.SoundWad.FindByName("BoxHero_Jump");

            InitializedAnim = false;

            OnGround = false;
            StartedJump = false;
            JumpCount = 0;
            FallingCount = 10000;

            Ducking = false;

            MyBob.PlayerObject.Read(6, 1);
        }

        protected string ExtraQuadString = "MainQuad";
        protected string ExtraTextureString = "BoxAlone";

        public override void DuckingPhsx()
        {
            base.DuckingPhsx();

            if (Ducking)
            {
                var p = MyBob.PlayerObject;
                p.DrawExtraQuad = true;
                p.ExtraQuadToDraw = (Quad)p.FindQuad(ExtraQuadString);
                p.ExtraQuadToDrawTexture = Tools.Texture(ExtraTextureString);
            }
            else
            {
                var p = MyBob.PlayerObject;
                p.DrawExtraQuad = false;
            }
        }

        protected virtual void ParentDoXAccel() { base.DoXAccel(); }
        public override void DoXAccel()
        {
            bool HoldDucking = Ducking;
            Ducking = true;
            base.DoXAccel();
            Ducking = HoldDucking;
        }

        protected virtual void ParentGenerateInput(int CurPhsxStep) { base.GenerateInput(CurPhsxStep); }
        public override void GenerateInput(int CurPhsxStep)
        {
            base.GenerateInput(CurPhsxStep);

            if (MyBob.CurInput.xVec.X > 0)
                MyBob.CurInput.A_Button = true;
        }

        protected int StandAnim = 6, JumpAnim = 7, DuckAnim = 8;

        public override void AnimStep()
        {
            if (MyBob.PlayerObject.DestinationAnim() != StandAnim && OnGround || !InitializedAnim)
            {
                if (!InitializedAnim)
                {
                    MyBob.PlayerObject.AnimQueue.Clear();
                    InitializedAnim = true;
                }
                MyBob.PlayerObject.EnqueueAnimation(StandAnim, 0, true);
                MyBob.PlayerObject.DequeueTransfers();
            }

            if (ShouldStartJumpAnim())
            {
                MyBob.PlayerObject.AnimQueue.Clear();
                MyBob.PlayerObject.EnqueueAnimation(JumpAnim, 0.3f, false);
                MyBob.PlayerObject.DequeueTransfers();
                MyBob.PlayerObject.LastAnimEntry.AnimSpeed *= .85f;

                StartJumpAnim = false;
            }

            // Ducking animation
            //int DuckAnim = 8;
            if (Ducking && MyBob.PlayerObject.DestinationAnim() != DuckAnim)
            {
                MyBob.PlayerObject.AnimQueue.Clear();
                MyBob.PlayerObject.EnqueueAnimation(DuckAnim, 1, false);
                MyBob.PlayerObject.DequeueTransfers();
                MyBob.PlayerObject.LastAnimEntry.AnimSpeed *= 2.5f;
            }
            // Reverse ducking animation
            if (!Ducking && MyBob.PlayerObject.DestinationAnim() == DuckAnim)
            {
                //MyBob.PlayerObject.DoSpriteAnim = false;

                MyBob.PlayerObject.AnimQueue.Clear();
                if (yVel > 0)
                    MyBob.PlayerObject.EnqueueAnimation(StandAnim, .8f, false);
                else
                    MyBob.PlayerObject.EnqueueAnimation(JumpAnim, .8f, false);
                MyBob.PlayerObject.DequeueTransfers();
                MyBob.PlayerObject.LastAnimEntry.AnimSpeed *= 200f;
            }

            if (!Ducking)
            if (MyBob.Core.Data.Velocity.Y < -.1f && !OnGround && MyBob.PlayerObject.anim == JumpAnim && MyBob.PlayerObject.LastAnimEntry.AnimSpeed > 0)
            {
                MyBob.PlayerObject.AnimQueue.Clear();
                MyBob.PlayerObject.EnqueueAnimation(JumpAnim, .9f, false);
                MyBob.PlayerObject.DequeueTransfers();
                MyBob.PlayerObject.LastAnimEntry.AnimSpeed *= -1f;
            }

            if (MyBob.IsSpriteBased)
                MyBob.PlayerObject.PlayUpdate(1);
            else
                MyBob.PlayerObject.PlayUpdate(1000f / 60f / 150f);
        }
    }
}