﻿using System.IO;
using System;
using Microsoft.Xna.Framework;

using CloudberryKingdom.Bobs;

namespace CloudberryKingdom.Blocks
{
    public delegate void BlockExtendCallback(NormalBlock block);
    public class NormalBlock : BlockBase
    {
        public override void TextDraw() { }

        public NormalBlockDraw MyDraw;

        public BlockBase HoldBlock;

        public bool Moved;

        public override void Interact(Bob bob) { }

        public void BasicConstruction(bool BoxesOnly)
        {
            Core.BoxesOnly = BoxesOnly;

            MyBox = new AABox();
            MyDraw = new NormalBlockDraw();

            MakeNew();
        }

        public override void Release()
        {
            BlockCore.Release();
            Core.MyLevel = null;
        
            if (MyDraw != null) MyDraw.Release();
            MyDraw = null;
        }

        public override void MakeNew()
        {
            Active = true;            

            Box.MakeNew();

            if (MyDraw != null)
                MyDraw.MakeNew();

            BlockCore.Init();
            BlockCore.Layer = .3f;
            Core.DrawLayer = 1;
            Core.MyType = ObjectType.NormalBlock;
            Core.EditHoldable = Core.Holdable = true;

            Init(Vector2.Zero, Vector2.Zero);
        }

        public NormalBlock(bool BoxesOnly)
        {
            BasicConstruction(BoxesOnly);
        }

        PieceQuad GetPieceTemplate()
        {
            if (TileSets.Get(Core.MyTileSetType).PassableSides)
            {
                BlockCore.UseTopOnlyTexture = false;
                Box.TopOnly = true;
            }

            switch (Core.MyTileSetType)
            {
                case TileSet.Castle:
                    return GetPieceTemplate_Inside1();

                case TileSet.Dungeon:
                    return GetPieceTemplate_Inside2();

                case TileSet.Island:
                    return GetPieceTemplate_Island();

                case TileSet.Dark:
                    return GetPieceTemplate_Dark();

                case TileSet.Rain:
                case TileSet.Terrace:
                    return GetPieceTemplate_Outside();

                case TileSet.DarkTerrace:
                    return GetPieceTemplate_Outside();

                case TileSet.OutsideGrass:
                    return GetPieceTemplate_OutsideGrass();

                case TileSet.TileBlock:
                    return GetPieceTemplate_TileBlock();

                case TileSet.CastlePiece:
                    return GetPieceTemplate_Castle(false);
                case TileSet.CastlePiece2:
                    return GetPieceTemplate_Castle(true);

                case TileSet.Catwalk:
                    Box.TopOnly = true;
                    return PieceQuad.Catwalk;

                case TileSet.Cement:
                    return GetPieceTemplate_Cement();

                default:
                    return GetPieceTemplate_Inside2();
            }

            return null;
        }

        PieceQuad GetPieceTemplate_Castle(bool Shift)
        {
            PieceQuad Template = null;
            if (Box.TopOnly && BlockCore.UseTopOnlyTexture)
            {
                if (Shift) Template = PieceQuad.Castle2;
                else Template = PieceQuad.Castle;
            }
            else
            {
                if (Shift) Template = PieceQuad.Castle2;
                else Template = PieceQuad.Castle;
            }

            return Template;
        }

        PieceQuad GetPieceTemplate_Outside()
        {
            // Check to see if we should make this a pillar, a floater, or a wall
            PieceQuad Template = null;
            if (Box.TopOnly && BlockCore.UseTopOnlyTexture)
            {
                Template = PieceQuad.Inside2_Thin;
            }
            else
            {
                if (Box.Current.Size.X < InfoWad.GetFloat("Outside_Pillar_SmallestWidthCutoff"))
                    Template = PieceQuad.Outside_Smallest;
                else if (Box.Current.Size.X < InfoWad.GetFloat("Outside_Pillar_SmallerWidthCutoff"))
                    Template = PieceQuad.Outside_Smaller;
                else if (Box.Current.Size.X < InfoWad.GetFloat("Outside_Pillar_SmallWidthCutoff"))
                    Template = PieceQuad.Outside_Small;
                else if (Box.Current.Size.X < InfoWad.GetFloat("Outside_Pillar_MediumWidthCutoff"))
                    Template = PieceQuad.Outside_Medium;
                else if (Box.Current.Size.X < InfoWad.GetFloat("Outside_Pillar_LargeWidthCutoff"))
                    Template = PieceQuad.Outside_Large;
                else
                    Template = PieceQuad.Outside_XLarge;
                //OutsideBlock;                
            }

            return Template;
        }

        PieceQuad GetPieceTemplate_OutsideGrass()
        {
            // Check to see if we should make this a pillar, a floater, or a wall
            PieceQuad Template = null;
            Template = PieceQuad.OutsideBlock;
            
            return Template;
        }

        PieceQuad GetPieceTemplate_Cement()
        {
            // Check to see if we should make this a pillar, a floater, or a wall
            PieceQuad Template = null;
            Template = PieceQuad.Cement;

            return Template;
        }

        PieceQuad GetPieceTemplate_TileBlock()
        {
            // Check to see if we should make this a pillar, a floater, or a wall
            PieceQuad Template = null;
            Template = PieceQuad.TileBlock;

            return Template;
        }

        PieceQuad GetPieceTemplate_Inside1()
        {
            // Check to see if we should make this a pillar, a floater, or a wall
            PieceQuad Template = null;
            if (Box.TopOnly && BlockCore.UseTopOnlyTexture)
            {
                if (Box.Current.Size.X < InfoWad.GetFloat("FloatingBlock_SmallWidthCutoff"))
                    Template = PieceQuad.Floating_Small;
                else if (Box.Current.Size.X < InfoWad.GetFloat("FloatingBlock_MediumWidthCutoff"))
                    Template = PieceQuad.Floating_Medium;
                else if (Box.Current.Size.X < InfoWad.GetFloat("FloatingBlock_LargeWidthCutoff"))
                    Template = PieceQuad.Floating_Large;
                else
                    Template = PieceQuad.Floating_Xlarge;
            }
            else
            {
                if (Box.Current.Size.X < 270)
                {
                    if (Box.Current.Size.X < InfoWad.GetFloat("BrickPillar_SmallWidthCutoff"))
                        Template = PieceQuad.BrickPillar_Small;
                    else if (Box.Current.Size.X < InfoWad.GetFloat("BrickPillar_MediumWidthCutoff"))
                        Template = PieceQuad.BrickPillar_Medium;
                    else if (Box.Current.Size.X < InfoWad.GetFloat("BrickPillar_LargeWidthCutoff"))
                        Template = PieceQuad.BrickPillar_Large;
                    else if (Box.Current.Size.X < InfoWad.GetFloat("BrickPillar_LargePlusWidthCutoff"))
                        Template = PieceQuad.BrickPillar_LargePlus;
                    else
                        Template = PieceQuad.BrickPillar_Xlarge;
                }
                else
                {
                    Template = PieceQuad.BrickWall;
                }
            }

            return Template;
        }

        PieceQuad GetPieceTemplate_Inside2()
        {
            // Check to see if we should make this a pillar, a floater, or a wall
            PieceQuad Template = null;
            if (Box.TopOnly && BlockCore.UseTopOnlyTexture)
            {
                Template = PieceQuad.Inside2_Thin;
            }
            else
            {
                if (Box.Current.Size.X < InfoWad.GetFloat("Inside2_SmallestWidthCutoff"))
                    Template = PieceQuad.Inside2_Smallest;
                else if (Box.Current.Size.X < InfoWad.GetFloat("Inside2_SmallerWidthCutoff"))
                    Template = PieceQuad.Inside2_Smaller;
                else if (Box.Current.Size.X < InfoWad.GetFloat("Inside2_SmallWidthCutoff"))
                    Template = PieceQuad.Inside2_Small;
                else if (Box.Current.Size.X < InfoWad.GetFloat("Inside2_MediumWidthCutoff"))
                    Template = PieceQuad.Inside2_Medium;
                else if (Box.Current.Size.X < InfoWad.GetFloat("Inside2_LargeWidthCutoff"))
                    Template = PieceQuad.Inside2_Large;
                else
                    Template = PieceQuad.Inside2_XLarge;

                if (Box.Current.Size.X < InfoWad.GetFloat("Inside2_Pillar_SmallestWidthCutoff"))
                    Template = PieceQuad.Inside2_Pillar_Smallest;
                else if (Box.Current.Size.X < InfoWad.GetFloat("Inside2_Pillar_SmallerWidthCutoff"))
                    Template = PieceQuad.Inside2_Pillar_Smaller;
                else if (Box.Current.Size.X < InfoWad.GetFloat("Inside2_Pillar_SmallWidthCutoff"))
                    Template = PieceQuad.Inside2_Pillar_Small;
                else if (Box.Current.Size.X < InfoWad.GetFloat("Inside2_Pillar_MediumWidthCutoff"))
                    Template = PieceQuad.Inside2_Pillar_Medium;
                else if (Box.Current.Size.X < InfoWad.GetFloat("Inside2_Pillar_LargeWidthCutoff"))
                    Template = PieceQuad.Inside2_Pillar_Large;
                else
                    Template = PieceQuad.Inside2_Pillar_XLarge;                
            }

            return Template;
        }

        PieceQuad GetPieceTemplate_Dark()
        {
            // Check to see if we should make this a pillar, a floater, or a wall
            PieceQuad Template = null;
            if (Box.TopOnly && BlockCore.UseTopOnlyTexture)
                Template = PieceQuad.Inside2_Thin;
            else
                Template = PieceQuad.DarkPillars.Get(Box.Current.Size.X);

            return Template;
        }

        PieceQuad GetPieceTemplate_Island()
        {
            Box.TopOnly = true;
            PieceQuad Template = PieceQuad.Islands.Get(Box.Current.Size.X);

            return Template;
        }

        public void ResetPieces()
        {
            MyDraw.Init(this, GetPieceTemplate());
        }

        public void Init(Vector2 center, Vector2 size)
        {
            BlockCore.Data.Position = BlockCore.StartData.Position = center;

            MyBox.Initialize(center, size);

            if (!Core.BoxesOnly)
                MyDraw.Init(this, GetPieceTemplate());
                        
            Update();

            Box.Validate();
        }

        static float TopOnlyHeight = 60;

        public void CheckHeight()
        {
            if (BlockCore.DisableFlexibleHeight || !Core.MyTileSet.FlexibleHeight)
            {
                if (MyBox.Current.BL.Y > Core.MyLevel.MainCamera.BL.Y - 20)
                    MakeTopOnly();
            }
            else
            {
                if (Box.Current.Size.Y < TopOnlyHeight)
                    MakeTopOnly();
            }
        }

        public void MakeTopOnly()
        {
            if (Core.MyTileSetType == TileSet.Cement)
                Core.MyTileSetType = TileSet.Catwalk;

            Box.TopOnly = true;
            Extend(Side.Bottom, Box.Current.TR.Y - TopOnlyHeight);
            Update();
        }

        public override void Move(Vector2 shift)
        {
            BlockCore.Data.Position += shift;
            BlockCore.StartData.Position += shift;

            Box.Move(shift);

            Update();
        }

        public override void LandedOn(Bob bob) { }
        public override void HitHeadOn(Bob bob) { }
        public override void SideHit(Bob bob) { }
        public override void Hit(Bob bob) { }

        public override void Reset(bool BoxesOnly)
        {
            if (Core.AlwaysBoxesOnly)
                BoxesOnly = true;
            else
                BlockCore.BoxesOnly = BoxesOnly;

            if (!Core.BoxesOnly)
                ResetPieces();

            Active = true;

            BlockCore.Data = BlockCore.StartData;

            MyBox.SetTarget(MyBox.Current.Center, MyBox.Current.Size);
            MyBox.SwapToCurrent();

            Update();
        }

        public override void PhsxStep()
        {
            //if (!Active) return;

            Active = Core.Active = true;
            Vector2 BL = MyBox.Current.BL;
            if (MyBox.Current.BL.X > BlockCore.MyLevel.MainCamera.TR.X || MyBox.Current.BL.Y > BlockCore.MyLevel.MainCamera.TR.Y + 500)//+ 1250)
                Active = Core.Active = false;
            Vector2 TR = MyBox.Current.TR;
            if (MyBox.Current.TR.X < BlockCore.MyLevel.MainCamera.BL.X || MyBox.Current.TR.Y < BlockCore.MyLevel.MainCamera.BL.Y - 250)//- 500)
                Active = Core.Active = false;

            //if (Core.MyLevel != null)
            //{
            //    MyBox.Target.Center = Pos + 100 * new Vector2(0, (float)Math.Sin(Core.GetPhsxStep() * .01f));
            //    Moved = true;
            //}
        }

        public override void PhsxStep2()
        {
            if (Moved)
                MyBox.SwapToCurrent();

            if (!Active) return;
        }


        public void Update()
        {
            /*
            if (!Core.GenData.Used && CoreData.BoxesOnly && Core.MyLevel != null && Core.MyLevel.PlayMode == 2)
            {
                //return;
                if (Core.MyTileSetType == TileSet.Cement)
                {
                    MyBox.Current.Size = MyBox.Target.Size =
                        new IntVector2(MyBox.Current.Size / 150) * 150;
                }
            }*/

            if (Core.MyTileSetType == TileSet.Island)
                Box.TopOnly = true;

            MyDraw.Update();
        }

        public void MoveTo(Vector2 Pos)
        {
            Core.Data.Position = Pos;
            MyBox.Target.Center = Pos;

            Box.SetTarget(Box.Target.Center, Box.Target.Size);
            Moved = true;
        }

        public override void Extend(Side side, float pos)
        {
            MyBox.Invalidated = true;

            MyBox.Extend(side, pos);

            Update();
            MyBox.Validate();

            // This is a hack to make sure dungeon blocks don't repeat vertically
            if (Core.MyTileSetType.DungeonLike() && !BlockCore.Ceiling && MyBox.Current.Size.Y > 2000)
                Extend(Side.Bottom, MyBox.Current.TR.Y - 1950);

            if (!Core.BoxesOnly)
                MyDraw.Init(this, GetPieceTemplate());

            BlockCore.StartData.Position = MyBox.Current.Center;

            if (!Core.BoxesOnly)
                ResetPieces();
        }

        public override void Draw()
        {
            if (!Active) return;
            if (!Core.Active) return;

            Update();

            if (Tools.DrawBoxes)
                MyBox.Draw(Tools.QDrawer, Color.Olive, 15);

            if (BlockCore.BoxesOnly) return;

            if (Tools.DrawGraphics && Core.Show)
            {
                if (Core.MyTileSetType != TileSet.None)
                {
                    MyDraw.Draw();
                    Tools.QDrawer.Flush();
                }

                if (Core.Encased)
                {
                    MyBox.DrawFilled(Tools.QDrawer, new Color(100, 100, 200, 100));
                    MyBox.Draw(Tools.QDrawer, new Color(120, 120, 240, 150), 18, true);
                }

                BlockCore.Draw();
            }
        }

        public override void Clone(ObjectBase A)
        {
            NormalBlock BlockA = A as NormalBlock;
            BlockCore.Clone(A.Core);

            if (BlockA == null) return;

            Box.TopOnly = BlockA.Box.TopOnly;

            Init(BlockA.Box.Current.Center, BlockA.Box.Current.Size);

            if (MyDraw != null && BlockA.MyDraw != null)
                MyDraw.Clone(BlockA.MyDraw);
        }
        
        public override void Write(BinaryWriter writer)
        {
            BlockCore.Write(writer);

            Box.Write(writer);            
        }
        public override void Read(BinaryReader reader)
        {
            BlockCore.Read(reader);

            Box.Read(reader);
            ResetPieces();
        }

        public override bool PreDecision(Bob bob)
        {
            if (BlockCore.Ceiling)
            {
                if (bob.Core.Data.Position.X > Box.Current.BL.X - 100 &&
                    bob.Core.Data.Position.X < Box.Current.TR.X + 100)
                {
                    float NewBottom = Box.Current.BL.Y;

                    // If ceiling has a left neighbor make sure we aren't too close to it
                    if (BlockCore.TopLeftNeighbor != null)
                    {
                        if (NewBottom > BlockCore.TopLeftNeighbor.Box.Current.BL.Y - 100)
                            NewBottom = Math.Max(NewBottom, BlockCore.TopLeftNeighbor.Box.Current.BL.Y + 120);
                    }
                    Extend(Side.Bottom,
                        Math.Max(NewBottom,
                                 Math.Max(bob.Box.Target.TR.Y, bob.Box.Current.TR.Y)
                                    + bob.CeilingParams.BufferSize.GetVal(bob.Core.Data.Position)));

                    if (Box.Current.Size.Y < 170 ||
                        Box.Current.BL.Y > bob.Core.MyLevel.MainCamera.TR.Y - 75)
                    {
                        bob.DeleteObj(this);
                    }
                }

                return true;
            }

            return false;
        }

        public override bool PostCollidePreDecision(Bob bob)
        {
            if (BlockCore.Ceiling)
            {
                Extend(Side.Bottom, Math.Max(Box.Current.BL.Y, Math.Max(bob.Box.Target.TR.Y, bob.Box.Current.TR.Y) + bob.CeilingParams.BufferSize.GetVal(bob.Core.Data.Position)));
                return true;
            }

            return false;
        }

        public override void PostCollideDecision(Bob bob, ref ColType Col, ref bool Overlap, ref bool Delete)
        {
            Block_PostCollideDecision(this as BlockBase, bob, ref Col, ref Overlap, ref Delete);
            base.PostCollideDecision(bob, ref Col, ref Overlap, ref Delete);
        }

        public static void Block_PostCollideDecision(BlockBase block, Bob bob, ref ColType Col, ref bool Overlap, ref bool Delete)
        {
            bool MakeTopOnly = false;

            // If we interact with the block in any way besides landing on top of it, make it top only
            if ((Col == ColType.Bottom || Overlap) && Col != ColType.Top) MakeTopOnly = true;
            if (Col == ColType.Left || Col == ColType.Right) MakeTopOnly = true;

            //// Note if we use the left or right side of the block
            //if ((Col == ColType.Left || Col == ColType.Right) && Col != ColType.Top)
            //{
            //    if (bob.Box.Current.TR.Y < block.Box.Current.TR.Y)
            //        MakeTopOnly = true;
            //    else
            //        MakeTopOnly = true;
            //}

            // If we've used something besides the top of the block already,
            // make sure we don't make the block top only
            if (block.BlockCore.NonTopUsed || !(block is NormalBlock))
            {
                if (MakeTopOnly)
                {
                    MakeTopOnly = false;
                    Delete = true;
                }
            }

            // If we are trying to make a block be top only that can't be, delete it
            if (MakeTopOnly && block.BlockCore.DeleteIfTopOnly)
            {
                if (block.Core.GenData.Used)
                    MakeTopOnly = Delete = false;
                else
                    Delete = true;
            }

            // If we have decided to make the block top only, actually do so
            if (MakeTopOnly)
            {
                block.Extend(Side.Bottom, Math.Max(block.Box.Current.BL.Y, Math.Max(bob.Box.Target.TR.Y, bob.Box.Current.TR.Y) + bob.CeilingParams.BufferSize.GetVal(bob.Core.Data.Position)));
                ((NormalBlock)block).CheckHeight();
                if (Col != ColType.Top)
                    Col = ColType.NoCol;
            }
        }

        public override void PostInteractWith(Bob bob)
        {
            base.PostInteractWith(bob);

            BlockBase block = (BlockBase)this;

            // Normal blocks delete surrounding blocks when stamped as used
            if (block.Core.GenData.DeleteSurroundingOnUse && block is NormalBlock)
                foreach (BlockBase nblock in Core.MyLevel.Blocks)
                {
                    NormalBlock Normal = nblock as NormalBlock;
                    if (null != Normal && !Normal.Core.MarkedForDeletion && !Normal.Core.GenData.AlwaysUse)
                        if (!Normal.Core.GenData.Used &&
                            Math.Abs(Normal.Box.Current.TR.Y - block.Box.TR.Y) < 15 &&
                            !(Normal.Box.Current.TR.X < block.Box.Current.BL.X - 350 || Normal.Box.Current.BL.X > block.Box.Current.TR.X + 350))
                        {
                            bob.DeleteObj(Normal);
                            Normal.IsActive = false;
                        }
                }
        }

        public override void PostKeep(Bob bob, ref ColType Col, ref bool Overlap)
        {
            base.PostKeep(bob, ref Col, ref Overlap);

            BlockBase block = (NormalBlock)this;

            // Shift bottom of block if necessary
            if (block is NormalBlock && !block.BlockCore.DeleteIfTopOnly)
            {
                float NewBottom = Math.Max(block.Box.Current.BL.Y,
                                           Math.Max(Box.Target.TR.Y, Box.Current.TR.Y) + bob.CeilingParams.BufferSize.GetVal(Core.Data.Position));

                if ((Col == ColType.Bottom || Overlap) && Col != ColType.Top &&
                    !block.BlockCore.NonTopUsed)
                {
                    block.Extend(Side.Bottom, NewBottom);
                    ((NormalBlock)block).CheckHeight();
                }
            }
        }

//StubStubStubStart
public override void OnUsed() { }
public override void OnMarkedForDeletion() { }
public override void OnAttachedToBlock() { }
public override bool PermissionToUse() { return true; }
public Vector2 Pos { get { return Core.Data.Position; } set { Core.Data.Position = value; } }
public GameData Game { get { return Core.MyLevel.MyGame; } }
public override void Smash(Bob bob) { }
//StubStubStubEnd7
    }
}
