﻿using System;
using System.Collections.Generic;

using Microsoft.Xna.Framework;

using CoreEngine;

using CloudberryKingdom.Levels;
using CloudberryKingdom.Bobs;
using CloudberryKingdom.Blocks;
using CloudberryKingdom.Obstacles;

namespace CloudberryKingdom
{
    public class BobPhsxSpaceship : BobPhsx
    {
        public static float KeepUnused(float UpgradeLevel)
        {
            return .5f + .03f * UpgradeLevel;
        }

        // Singleton
        protected override void InitSingleton()
        {
            base.InitSingleton();

            Specification = new HeroSpec(4, 0, 0, 0);
            Name = Localization.Words.Spaceship;
            NameTemplate = "spaceship";
            Icon = new PictureIcon(Tools.Texture("Spaceship_Paper"), Color.White, 1.15f * DefaultIconWidth);
        }
        static readonly BobPhsxSpaceship instance = new BobPhsxSpaceship();
        public static BobPhsxSpaceship Instance { get { return instance; } }

        // Instancable class
		protected int AutoMoveLength, AutoMoveType, AutoStrafeLength;
		protected int AutoDirLength, AutoDir;

        protected int RndMoveType;

        public BobPhsxSpaceship()
        {
            DefaultValues();
        }

        public override void DefaultValues()
        {
            //MaxSpeed = 24f;
            MaxSpeed = 30f;

            XAccel = 2.3f;

            base.DefaultValues();
        }

        public override void Init(Bob bob)
        {
            base.Init(bob);

            bob.DieSound = Tools.SoundWad.FindByName("DustCloud_Explode");

            OnGround = false;
        }

        public override void PhsxStep()
        {
            base.PhsxStep();

            if (MyBob.CharacterSelect2) return;

            if (MyBob.MyLevel.PlayMode == 0 && !MyBob.InputFromKeyboard && !MyBob.MyLevel.Watching)
            {
                if (MyBob.CurInput.A_Button)
                    MyBob.CurInput.xVec.X = 1;
                else if (MyBob.CurInput.xVec.X > .5f)
                    MyBob.CurInput.xVec.X = .5f;
            }

            MyLevel.MyCamera.MovingCamera = true;

            MyBob.Core.Data.Velocity *= .86f;
            MyBob.Core.Data.Velocity.X += 2.3f;

            if (MyBob.CurInput.xVec.Length() > .2f)
            {
                float boost = 1;
                if (MyBob.CurInput.xVec.X == 1)
                    boost = 1.2f;

                MyBob.Core.Data.Velocity += boost * XAccel * MyBob.CurInput.xVec;

                float Magnitude = MyBob.Core.Data.Velocity.Length();
                if (Magnitude > boost * MaxSpeed)
                {
                    MyBob.Core.Data.Velocity.Normalize();
                    MyBob.Core.Data.Velocity *= MaxSpeed;
                }
            }
         
            OnGround = false;
        }

        public override void SideHit(ColType side, BlockBase block)
        {
            base.SideHit(side, block);

            if (MyLevel.PlayMode == 0 && !MyBob.Immortal)
                MyBob.Die(Bob.BobDeathType.Other);
        }

        public override void PhsxStep2()
        {
            base.PhsxStep2();

            if (MyBob.Core.MyLevel.PlayMode == 0 && MyBob.CurInput.xVec.X > -.3f)
            {
                float intensity = Math.Min(.3f + (MyBob.CurInput.xVec.X + .3f), 1f);
                if (MyBob.CurInput.xVec.X <= .5f)
                    intensity = Math.Min(intensity, .3f + (.1f + .3f));

                int layer = Math.Max(1, MyBob.Core.DrawLayer - 1);
                ParticleEffects.Thrust(MyBob.Core.MyLevel, layer, Pos + new Vector2(0, 10), new Vector2(-1, 0), new Vector2(-10, yVel), intensity);
            }
        }

        public override bool CheckFor_xFlip()
        {
            return false;
        }

        public virtual void Jump()
        {
        }

        public override void LandOnSomething(bool MakeReadyToJump, ObjectBase ThingLandedOn)
        {
            base.LandOnSomething(MakeReadyToJump, ThingLandedOn);

            MyBob.Core.Data.Velocity.Y = MyBob.Core.Data.Velocity.Y + 5;

            MyBob.BottomCol = true;

            OnGround = true;
        }

        public override void HitHeadOnSomething(ObjectBase ThingHit)
        {
            base.HitHeadOnSomething(ThingHit);
        }

        int Dir = 0;
        public void GenerateInput_Vertical(int CurPhsxStep)
        {
            //MyBob.CurInput.A_Button = false;
            //if (TurnCountdown <= 0)
            //{
            //    if (Dir == 0) Dir = 1;

            //    Dir *= -1;
            //    TurnCountdown = MyLevel.Rnd.RndInt(0, 135);
            //}
            //else
            //    TurnCountdown--;

            //Camera cam = MyBob.Core.MyLevel.MainCamera;
            //float HardBound = 1000; float SoftBound = 1500;
            //if (Pos.X > cam.TR.X - HardBound) Dir = -1;
            //if (Pos.X < cam.BL.X + HardBound) Dir = 1;
            //if (Pos.X > cam.TR.X - SoftBound && Dir == 1) TurnCountdown -= 2;
            //if (Pos.X < cam.BL.X + SoftBound && Dir == -1) TurnCountdown -= 2;

            //MyBob.CurInput.xVec.X = Dir;

            MyBob.CurInput.B_Button = false;

            if (MyBob.Core.MyLevel.GetPhsxStep() % 60 == 0)
                RndMoveType = MyLevel.Rnd.Rnd.Next(0, 3);

            if (AutoDirLength == 0)
            {
                if (AutoDir == 1) AutoDir = -1; else AutoDir = 1;
                if (AutoDir == 1)
                    AutoDirLength = MyLevel.Rnd.Rnd.Next(MyBob.Core.MyLevel.CurMakeData.GenData.Get(BehaviorParam.ForwardLengthAdd, MyBob.Core.Data.Position)) + MyBob.Core.MyLevel.CurMakeData.GenData.Get(BehaviorParam.ForwardLengthBase, MyBob.Core.Data.Position);
                else
                    AutoDirLength = MyLevel.Rnd.Rnd.Next(MyBob.Core.MyLevel.CurMakeData.GenData.Get(BehaviorParam.BackLengthAdd, MyBob.Core.Data.Position)) + MyBob.Core.MyLevel.CurMakeData.GenData.Get(BehaviorParam.BackLengthBase, MyBob.Core.Data.Position);
            }

            if (AutoMoveLength == 0)
            {
                int rnd = MyLevel.Rnd.Rnd.Next(MyBob.Core.MyLevel.CurMakeData.GenData.Get(BehaviorParam.MoveWeight, MyBob.Core.Data.Position) + MyBob.Core.MyLevel.CurMakeData.GenData.Get(BehaviorParam.SitWeight, MyBob.Core.Data.Position));
                if (rnd < MyBob.Core.MyLevel.CurMakeData.GenData.Get(BehaviorParam.MoveWeight, MyBob.Core.Data.Position))
                {
                    AutoMoveType = 1;
                    AutoMoveLength = MyLevel.Rnd.Rnd.Next(MyBob.Core.MyLevel.CurMakeData.GenData.Get(BehaviorParam.MoveLengthAdd, MyBob.Core.Data.Position)) + MyBob.Core.MyLevel.CurMakeData.GenData.Get(BehaviorParam.MoveLengthBase, MyBob.Core.Data.Position);
                }
                else
                {
                    AutoMoveType = 0;
                    AutoMoveLength = MyLevel.Rnd.Rnd.Next(MyBob.Core.MyLevel.CurMakeData.GenData.Get(BehaviorParam.SitLengthAdd, MyBob.Core.Data.Position)) + MyBob.Core.MyLevel.CurMakeData.GenData.Get(BehaviorParam.SitLengthBase, MyBob.Core.Data.Position);
                }
            }

            AutoMoveLength--;
            AutoStrafeLength--;
            AutoDirLength--;

            if (AutoMoveType == 1)
                MyBob.CurInput.xVec.Y = AutoDir;


            float RetardFactor = .01f * MyBob.Core.MyLevel.CurMakeData.GenData.Get(DifficultyParam.JumpingSpeedRetardFactor, MyBob.Core.Data.Position);
            if (MyBob.Core.Data.Velocity.Y > RetardFactor * MaxSpeed)
                MyBob.CurInput.xVec.Y = 0;

            MyBob.CurInput.xVec.Y *= Math.Min(1, (float)Math.Cos(MyBob.Core.MyLevel.GetPhsxStep() / 65f) + 1.35f);

            float t = 0;
            if (RndMoveType == 0)
                t = ((float)Math.Cos(MyBob.Core.MyLevel.GetPhsxStep() / 40f) + 1) / 2;
            if (RndMoveType == 1)
                t = ((float)Math.Sin(MyBob.Core.MyLevel.GetPhsxStep() / 40f) + 1) / 2;
            if (RndMoveType == 2)
                t = Math.Abs((MyBob.Core.MyLevel.GetPhsxStep() % 120) / 120f);

            MyBob.TargetPosition.X = MyBob.MoveData.MinTargetY - 160 + t * (200 + MyBob.MoveData.MaxTargetY - MyBob.MoveData.MinTargetY);
            //+ 200 * (float)Math.Cos(MyBob.Core.MyLevel.GetPhsxStep() / 20f);

            if (MyBob.Core.Data.Position.X < MyBob.TargetPosition.X)
                MyBob.CurInput.xVec.X = 1;
            if (MyBob.Core.Data.Position.X > MyBob.TargetPosition.X)
                MyBob.CurInput.xVec.X = -1;
            MyBob.CurInput.xVec.X *= Math.Min(1, Math.Abs(MyBob.TargetPosition.X - MyBob.Core.Data.Position.X) / 100);

            if (Pos.X > MyBob.TargetPosition.X + 400) MyBob.CurInput.xVec.X = 1;
            if (Pos.X < MyBob.TargetPosition.X - 400) MyBob.CurInput.xVec.X = -1;

            if (MyBob.Core.Data.Position.Y > MyBob.Core.MyLevel.CurMakeData.TRBobMoveZone.Y ||
                MyBob.Core.Data.Position.X > MyBob.Core.MyLevel.CurMakeData.TRBobMoveZone.X)
            {
                MyBob.CurInput.xVec.Y = 0;
            }
        }

        public override void GenerateInput(int CurPhsxStep)
        {
            base.GenerateInput(CurPhsxStep);

            switch (Geometry)
            {
                case LevelGeometry.Right:
                    GenerateInput_Right(CurPhsxStep);
                    break;

                case LevelGeometry.Down:
                case LevelGeometry.Up:
                    GenerateInput_Vertical(CurPhsxStep);
                    break;

                default:
                    break;
            }

            AdditionalGenerateInputChecks(CurPhsxStep);
        }

        protected virtual void GenerateInput_Right(int CurPhsxStep)
        {
            MyBob.CurInput.B_Button = false;

            if (MyBob.Core.MyLevel.GetPhsxStep() % 60 == 0)
                RndMoveType = MyLevel.Rnd.Rnd.Next(0, 3);

            if (AutoDirLength == 0)
            {
                if (AutoDir == 1) AutoDir = -1; else AutoDir = 1;
                if (AutoDir == 1)
                    AutoDirLength = MyLevel.Rnd.Rnd.Next(MyBob.Core.MyLevel.CurMakeData.GenData.Get(BehaviorParam.ForwardLengthAdd, MyBob.Core.Data.Position)) + MyBob.Core.MyLevel.CurMakeData.GenData.Get(BehaviorParam.ForwardLengthBase, MyBob.Core.Data.Position);
                else
                    AutoDirLength = MyLevel.Rnd.Rnd.Next(MyBob.Core.MyLevel.CurMakeData.GenData.Get(BehaviorParam.BackLengthAdd, MyBob.Core.Data.Position)) + MyBob.Core.MyLevel.CurMakeData.GenData.Get(BehaviorParam.BackLengthBase, MyBob.Core.Data.Position);
            }

            if (AutoMoveLength == 0)
            {
                int rnd = MyLevel.Rnd.Rnd.Next(MyBob.Core.MyLevel.CurMakeData.GenData.Get(BehaviorParam.MoveWeight, MyBob.Core.Data.Position) + MyBob.Core.MyLevel.CurMakeData.GenData.Get(BehaviorParam.SitWeight, MyBob.Core.Data.Position));
                if (rnd < MyBob.Core.MyLevel.CurMakeData.GenData.Get(BehaviorParam.MoveWeight, MyBob.Core.Data.Position))
                {
                    AutoMoveType = 1;
                    AutoMoveLength = MyLevel.Rnd.Rnd.Next(MyBob.Core.MyLevel.CurMakeData.GenData.Get(BehaviorParam.MoveLengthAdd, MyBob.Core.Data.Position)) + MyBob.Core.MyLevel.CurMakeData.GenData.Get(BehaviorParam.MoveLengthBase, MyBob.Core.Data.Position);
                }
                else
                {
                    AutoMoveType = 0;
                    AutoMoveLength = MyLevel.Rnd.Rnd.Next(MyBob.Core.MyLevel.CurMakeData.GenData.Get(BehaviorParam.SitLengthAdd, MyBob.Core.Data.Position)) + MyBob.Core.MyLevel.CurMakeData.GenData.Get(BehaviorParam.SitLengthBase, MyBob.Core.Data.Position);
                }
            }
        
            AutoMoveLength--;
            AutoStrafeLength--;          
            AutoDirLength--;
           
            if (AutoMoveType == 1)
                MyBob.CurInput.xVec.X = AutoDir;

      
            float RetardFactor = .01f * MyBob.Core.MyLevel.CurMakeData.GenData.Get(DifficultyParam.JumpingSpeedRetardFactor, MyBob.Core.Data.Position);
            if (!OnGround && MyBob.Core.Data.Velocity.X > RetardFactor * MaxSpeed)
                MyBob.CurInput.xVec.X = 0;

            MyBob.CurInput.xVec.X *= Math.Min(1, (float)Math.Cos(MyBob.Core.MyLevel.GetPhsxStep() / 65f) + 1.35f);

            float t = 0;
            if (RndMoveType == 0)
                t = ((float)Math.Cos(MyBob.Core.MyLevel.GetPhsxStep() / 40f) + 1) / 2;
            if (RndMoveType == 1)
                t = ((float)Math.Sin(MyBob.Core.MyLevel.GetPhsxStep() / 40f) + 1) / 2;
            if (RndMoveType == 2)
                t = Math.Abs((MyBob.Core.MyLevel.GetPhsxStep() % 120) / 120f);

            MyBob.TargetPosition.Y = MyBob.MoveData.MinTargetY - 200 + t * (-90 + MyBob.MoveData.MaxTargetY - MyBob.MoveData.MinTargetY);
                    //+ 200 * (float)Math.Cos(MyBob.Core.MyLevel.GetPhsxStep() / 20f);
            
            if (MyBob.Core.Data.Position.Y < MyBob.TargetPosition.Y)
                MyBob.CurInput.xVec.Y = 1;
            if (MyBob.Core.Data.Position.Y > MyBob.TargetPosition.Y)
                MyBob.CurInput.xVec.Y = -1;
            MyBob.CurInput.xVec.Y *= Math.Min(1, Math.Abs(MyBob.TargetPosition.Y - MyBob.Core.Data.Position.Y) / 100);
            
            if (Pos.X > CurPhsxStep * 1.1f * (4000f / 600f))
            {
                if (Pos.Y > MyBob.TargetPosition.Y && (CurPhsxStep / 40) % 3 == 0)
                    MyBob.CurInput.xVec.X = -1;
                if (Pos.Y < MyBob.TargetPosition.Y && (CurPhsxStep / 25) % 4 == 0)
                    MyBob.CurInput.xVec.X = -1;
            }
            if (Pos.Y < MyBob.TargetPosition.Y && Pos.X < CurPhsxStep * (4000f / 900f))
            {
                MyBob.CurInput.xVec.X = 1;
            }

            if (Pos.X < MyLevel.MainCamera.BL.X + 400)
                MyBob.CurInput.xVec.X = 1;
            if (Pos.X > MyLevel.MainCamera.TR.X - 500 && MyBob.CurInput.xVec.X > 0)
                MyBob.CurInput.xVec.X /= 2;

            if (Pos.X > MyLevel.Fill_TR.X - 1200)
            {
                if (Pos.Y > MyLevel.MainCamera.TR.Y - 600) MyBob.CurInput.xVec.Y = -1;
                if (Pos.Y < MyLevel.MainCamera.BL.Y + 600) MyBob.CurInput.xVec.Y = 1;
            }

            if (MyBob.Core.Data.Position.X > MyBob.Core.MyLevel.CurMakeData.TRBobMoveZone.X ||
                MyBob.Core.Data.Position.Y > MyBob.Core.MyLevel.CurMakeData.TRBobMoveZone.Y)
            {
                MyBob.CurInput.xVec.X = 0;
            }
        }

        public override void AnimStep()
        {
            base.AnimStep();
        }

        public override void ToSprites(Dictionary<int, SpriteAnim> SpriteAnims, Vector2 Padding)
        {
            ObjectClass Obj = MyBob.PlayerObject;
            SpriteAnims.Add(0, Obj.AnimToSpriteFrames(0, 1, true, Padding));
        }

        public override void Die(Bob.BobDeathType DeathType)
        {
            base.Die(DeathType);

            MyBob.Core.Data.Velocity = new Vector2(0, 25);
            MyBob.Core.Data.Acceleration = new Vector2(0, -1.9f);

            Fireball.Explosion(MyBob.Core.Data.Position, MyBob.Core.MyLevel);
        }

        public override void BlockInteractions()
        {
            if (Core.MyLevel.PlayMode != 0) return;

            foreach (BlockBase block in Core.MyLevel.Blocks)
            {
                if (!block.Core.MarkedForDeletion && block.Core.Real && block.IsActive && block.Core.Active && Phsx.BoxBoxOverlap(MyBob.Box2, block.Box))
                {
                    if (!MyBob.Immortal)
                        MyBob.Die(Bob.BobDeathType.Other);
                    else
                        block.Hit(MyBob);
                }
            }            
        }

        public override void ModData(ref Level.MakeData makeData, StyleData Style)
        {
            base.ModData(ref makeData, Style);

            Style.MyInitialPlatsType = StyleData.InitialPlatsType.Spaceship;
            Style.TopSpace = 0;
            makeData.SparsityMultiplier = 1.5f;

            Style.BlockFillType = StyleData._BlockFillType.Invertable;
            Style.OverlapCleanupType = StyleData._OverlapCleanupType.Sophisticated;

            Style.DoorHitBoxPadding = new Vector2(-60, 0);

            Style.MinBlockDist = 250;
            Style.RemovedUnusedOverlappingBlocks = true;
            Style.RemoveBlockOnOverlap = true;

            Style.BottomSpace = 150;
            Style.TopSpace = 0;

            Style.SafeStartPadding = 400;
            Style.SafeEndPadding = -1000;
            Style.LengthPadding = 1200;
            Style.AutoOpenDoor = true;

            makeData.TopLikeBottom = true;

            Style.MyGroundType = StyleData.GroundType.VirginUsed;
            Style.MyTopType = StyleData.GroundType.InvertedUsed;
            Style.UpperSafetyNetOffset = -100;
            Style.LowerSafetyNetOffset = -200;

            var GhParams = (GhostBlock_Parameters)Style.FindParams(GhostBlock_AutoGen.Instance);
            GhParams.BoxType = GhostBlock_Parameters.BoxTypes.Full;
        }

        public override void ModLadderPiece(PieceSeedData piece)
        {
 	        base.ModLadderPiece(piece);

            piece.ElevatorBoxStyle = BlockEmitter_Parameters.BoxStyle.FullBox;
        }
    }
}