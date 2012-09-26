﻿using System;
using Microsoft.Xna.Framework;

namespace CloudberryKingdom
{
    public partial class TileSets
    {
        static TileSet Load_Cave()
        {
            var t = GetOrMakeTileset("Cloud");
            var info = t.MyTileSetInfo;

            t._Start();

t.Name = "cave";

t.Pillars.Add(new PieceQuad(50, "pillar_cave_50", -15, 15, 3));
t.Pillars.Add(new PieceQuad(100, "pillar_cave_100", -15, 15, 12));
t.Pillars.Add(new PieceQuad(150, "pillar_cave_150", -15, 15, 0));
t.Pillars.Add(new PieceQuad(250, "pillar_cave_250", -15, 15, 0));
t.Pillars.Add(new PieceQuad(300, "pillar_cave_300", -15, 15, 0));
t.Pillars.Add(new PieceQuad(600, "pillar_cave_600", -15, 15, 0));
t.Pillars.Add(new PieceQuad(1000, "pillar_cave_1000", -15, 15, 0));

//t.Pillars.Add(new PieceQuad(50, "pillar_cave_50_v3", -15, 15, 3));
//t.Pillars.Add(new PieceQuad(100, "pillar_cave_100_v3", -15, 15, 12));
//t.Pillars.Add(new PieceQuad(150, "pillar_cave_150_v3", -15, 15, 0));
//t.Pillars.Add(new PieceQuad(250, "pillar_cave_250_v3", -15, 15, 0));
//t.Pillars.Add(new PieceQuad(300, "pillar_cave_300_v3", -15, 15, 0));
//t.Pillars.Add(new PieceQuad(600, "pillar_cave_600_v3", -15, 15, 0));
//t.Pillars.Add(new PieceQuad(1000, "pillar_cave_1000_v3", -15, 15, 0));

t.Ceilings.Add(new PieceQuad(50, "pillar_cave_50", -20, 20, 0, true));
t.Ceilings.Add(new PieceQuad(100, "pillar_cave_100", -20, 20, 0, true));
t.Ceilings.Add(new PieceQuad(150, "pillar_cave_150", -20, 20, 0, true));
t.Ceilings.Add(new PieceQuad(250, "pillar_cave_250", -20, 20, 0, true));
t.Ceilings.Add(new PieceQuad(300, "pillar_cave_300", -20, 20, 0, true));
t.Ceilings.Add(new PieceQuad(600, "pillar_cave_600", -20, 20, 0, true));
t.Ceilings.Add(new PieceQuad(1000, "pillar_cave_1000", -20, 20, 0, true));

t.StartBlock.Add(new PieceQuad(400, "wall_cave", -650, 120, 1548));
t.EndBlock.Add(new PieceQuad(400, "wall_cave", -34, 736, 1548));

info.ShiftStartDoor = -180;
info.ShiftStartBlock = new Vector2(300, 0);

sprite_anim("door_cave", "door_cave", 1, 2, 2);
info.Doors.Sprite.Sprite = "door_cave";
info.Doors.Sprite.Size = new Vector2(350, -1);
info.Doors.Sprite.Offset = new Vector2(-140, 38);
info.Doors.Sprite.Size = new Vector2(600, 270);
info.Doors.Sprite.Offset = new Vector2(-330, 56);
info.Doors.Sprite.Size = new Vector2(600, -1);
info.Doors.Sprite.Offset = new Vector2(-330, 200);
info.Doors.Sprite.Size = new Vector2(500, -1);
info.Doors.Sprite.Offset = new Vector2(-250, 135);
info.Doors.ShiftStart = new Vector2(0, 190);

info.Walls.Sprite.Sprite = "pillar_cave_1000";
info.Walls.Sprite.Size = new Vector2(1500, -1);
info.Walls.Sprite.Offset = new Vector2(0, 4550);
info.Walls.Sprite.Degrees = -90;

info.LavaDrips.Line.End1 = "Flow_cave_1";
info.LavaDrips.Line.Sprite = "Flow_cave_2";
info.LavaDrips.Line.End2 = "Flow_Cave_3";

info.Lasers.Line.Sprite = "Laser_Cave";
info.Lasers.Line.RepeatWidth = 135;
info.Lasers.Line.Dir = 0;
info.Lasers.Scale = 1;
info.Lasers.Tint_Full = new Vector4(1, 1, 1, .95f);
info.Lasers.Tint_Half = new Vector4(1, 1, 1, .4f);

sprite_anim("fblock_cave", "fblock_cave", 1, 3, 2);
info.FallingBlocks.Group.Add(new PieceQuad(103, "fblock_cave", -3, 3, 2));

sprite_anim("Bouncy_cave", "Bouncy_cave", 1, 3, 2);
info.BouncyBlocks.Group.Add(new PieceQuad(124, "bouncy_cave", -6, 6, 13));

//sprite_anim("flame_cave", "firespinner_flame_cave", 1, 4, 6);
sprite_anim("flame_cave", "firespinner_flame_cloud", 1, 4, 6);
info.Spinners.Flame.Sprite = "flame_cave";
info.Spinners.Flame.Size = new Vector2(45, -1);
info.Spinners.Rotate = false;
info.Spinners.RotateStep = .13f;
//info.Spinners.Base.Sprite = "firespinner_base_cave";
//info.Spinners.Base.Sprite = "firespinner_gear_dkpurp";
info.Spinners.Base.Sprite = "firespinner_base_cloud";
info.Spinners.Base.Size = new Vector2(90, -1);
info.Spinners.Base.Offset = new Vector2(0, -25);
info.Spinners.SegmentSpacing = 65;
info.Spinners.SpaceFromBase = 55;

info.GhostBlocks.Sprite = "ghostblock_cave";
info.GhostBlocks.Shift = new Vector2(0, -15);

info.MovingBlocks.Group.Add(new PieceQuad(190, "movingblock_cave_190", -1, 1, 7));
info.MovingBlocks.Group.Add(new PieceQuad(135, "movingblock_cave_135", -1, 1, 7));
info.MovingBlocks.Group.Add(new PieceQuad(80, "movingblock_cave_80", -1, 1, 3));
info.MovingBlocks.Group.Add(new PieceQuad(40, "movingblock_cave_40", -1, 1, 3));

info.Elevators.Group.Add(new PieceQuad(40, "Elevator_Cave_40", -1, 1, 1));
info.Elevators.Group.Add(new PieceQuad(80, "Elevator_Cave_80", -1, 1, 1));
info.Elevators.Group.Add(new PieceQuad(135, "Elevator_Cave_135", -1, 1, 1));
info.Elevators.Group.Add(new PieceQuad(190, "Elevator_Cave_190", -1, 1, 1));
//info.Elevators.Group.Add(40, "Cave_40_v2", -1, 1, 1));
//info.Elevators.Group.Add(80, "Cave_80_v2", -1, 1, 1));
//info.Elevators.Group.Add(135, "Cave_135_v2", -1, 1, 1));
//info.Elevators.Group.Add(190, "Cave_190_v2", -1, 1, 1));

info.Pendulums.Group.Add(new PieceQuad(40, "Elevator_Cave_40", -1, 1, 1));
info.Pendulums.Group.Add(new PieceQuad(80, "Elevator_Cave_80", -1, 1, 1));
info.Pendulums.Group.Add(new PieceQuad(135, "Elevator_Cave_135", -1, 1, 1));
info.Pendulums.Group.Add(new PieceQuad(190, "Elevator_Cave_190", -1, 1, 1));

sprite_anim("Serpent_Cave", "Serpent_Cloud", 1, 2, 8);
info.Serpents.Serpent.Sprite = "Serpent_Cave";
info.Serpents.Serpent.Offset = new Vector2(0, -.675f);
sprite_anim("Serpent_Fish_Cave", "Serpent_Fish_Cloud", 1, 2, 5);
info.Serpents.Fish.Sprite = "Serpent_Fish_Cave";
info.Serpents.Fish.Size = new Vector2(60, -1);
info.Serpents.Fish.Offset = new Vector2(55, 0);

info.Serpents.Serpent.Sprite = "Serpent_cave";

info.Spikes.Spike.Sprite = "spike_cave";
info.Spikes.Spike.Size = new Vector2(38, -1);
info.Spikes.Spike.Offset = new Vector2(0, 1);
info.Spikes.Spike.RelativeOffset = true;
info.Spikes.Base.Sprite = "spike_base_cave_1";
info.Spikes.Base.Size = new Vector2(54, -1);
info.Spikes.PeakHeight =.335f;

info.SpikeyGuys.Ball.Sprite = "floater_spikey_cave";
info.SpikeyGuys.Ball.Size = new Vector2(150, -1);
info.SpikeyGuys.Radius = 120;
info.SpikeyGuys.Chain.Sprite = "floater_chain_cave";
info.SpikeyGuys.Chain.Width = 55;
info.SpikeyGuys.Chain.RepeatWidth = 1900;

info.SpikeyGuys.Ball.Sprite = "Floater_Boulder_Cloud";
info.SpikeyGuys.Ball.Size = new Vector2(200, -1);
info.SpikeyGuys.Radius = 140;
info.SpikeyGuys.Chain.Sprite = "Floater_Rope_Cave";
info.SpikeyGuys.Chain.RepeatWidth = 1900;
info.SpikeyGuys.Chain.Width = 55;

info.Orbs.Ball.Sprite = "floater_spikey_cave";
info.Orbs.Ball.Size = new Vector2(150, -1);
info.Orbs.Ball.Offset = new Vector2(0, 8);
info.Orbs.Base.Sprite = null;
info.Orbs.Rotate = true;
info.Orbs.Radius = 116;
info.Orbs.RotateOffset = -1.57f;
info.Orbs.Chain.Sprite = "floater_chain_cave";
info.Orbs.Chain.Width = 55;
info.Orbs.Chain.RepeatWidth = 1900;

//info.Orbs.Ball.Sprite = "Floater_Boulder_Cloud";
//info.Orbs.Ball.Size = new Vector2(200, -1);
//info.Orbs.Radius = 140;

info.SpikeyLines.Ball.Sprite = "Floater_Spikey_Cave";
info.SpikeyLines.Ball.Size = new Vector2(150, -1);
info.SpikeyLines.Ball.Offset = new Vector2(-8, 12);
info.SpikeyLines.Radius = 100;
info.SpikeyLines.Rotate = true;
info.SpikeyLines.RotateSpeed = .05f;

sprite_anim("blob_cave", "blob_cave", 1, 4, 2);
info.Blobs.Body.Sprite = "blob_cave";
info.Blobs.Body.Size = new Vector2(130, -1);
info.Blobs.Body.Offset = new Vector2(20, 20);
info.Blobs.GooSprite = "BlobGoo5";

info.Clouds.Sprite.Sprite = "cloud_cave";

info.Coins.Sprite.Sprite = "coin_blue";
info.Coins.Sprite.Size = new Vector2(105, -1);
info.Coins.ShowCoin = true;
info.Coins.ShowEffect = true;
info.Coins.ShowText = true;

info.AllowLava = false;
info.ObstacleCutoff =  40;

            t._Finish();

            return t;
        }
    }
}