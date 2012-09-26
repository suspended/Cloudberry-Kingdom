﻿using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework.Audio;
using CloudberryKingdom;

namespace Drawing
{
    public class EzSoundWad
    {
        /// <summary>
        /// When true all new sounds to be played are suppressed.
        /// </summary>
        public static bool SuppressSounds = false;

        public List<EzSound> SoundList;
        public int MaxInstancesPerSound;

        public EzSoundWad(int MaxInstancesPerSound)
        {
            this.MaxInstancesPerSound = MaxInstancesPerSound;

            SoundList = new List<EzSound>();
        }

        public void Update()
        {
            //   foreach (EzSound sound in SoundList)
            //     sound.Update();
        }

        public EzSound FindByName(string name)
        {
            foreach (EzSound Snd in SoundList)
                if (String.Compare(Snd.Name, name, StringComparison.OrdinalIgnoreCase) == 0)
                    //if (Snd.Name.CompareTo(name) == 0)
                    return Snd;
            return SoundList[0];
        }

        public void AddSound(SoundEffect sound, string Name)
        {
            EzSound NewSound = new EzSound();
            NewSound.Name = Name;
            NewSound.sound = sound;
            NewSound.MaxInstances = MaxInstancesPerSound;

            SoundList.Add(NewSound);
        }
    }
}