﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BNSA_Unpacker.classes
{
    class MiniAnim
    {
        public long Pointer;
        public List<MiniFrame> MiniFrames;

        public bool IsValid = false; //turns true if final frame of anim ends properly.

        /// <summary>
        /// Constructs a MiniAnim object from the current position of the stream. Will create sub-mini frames.
        /// </summary>
        /// <param name="stream">Stream to construct minianim from.</param>
        public MiniAnim(FileStream stream, long MiniAnimTablePointer)
        {
            Pointer = stream.Position - MiniAnimTablePointer;
            Console.WriteLine("Reading MiniAnimation 0x" + stream.Position.ToString("X2")); //"X8" = 8 byte hex output
            stream.Seek(Pointer+MiniAnimTablePointer, SeekOrigin.Begin);
            MiniFrames = new List<MiniFrame>();
            int frameindex = 0;
            while (true)
            {
                //Read Frame Until Stop or Loop at Pos + 0x12
                Console.WriteLine("--Reading MiniFrame " + frameindex + " at 0x" + stream.Position.ToString("X2"));
                MiniFrame miniframe = new MiniFrame(stream);
                MiniFrames.Add(miniframe);
                if (miniframe.EndFrame) //End of animation indicator
                {
                    IsValid = miniframe.IsValid;
                    break;
                }
                frameindex++;
            }
        }

        internal void Export(string outputDirectory, int miniAnimGroupIndex, int miniAnimIndex)
        {
            int i = 0;
            foreach (MiniFrame frame in MiniFrames)
            {
                //Console.WriteLine("--Resolving Frame " + i + " references");
                frame.Export(outputDirectory, miniAnimGroupIndex, miniAnimIndex,i);
                i++;
            }
        }

        internal void ResolveReferences(BNSAFile parsedBNSA)
        {
            int i = 0;
            foreach (MiniFrame miniFrame in MiniFrames)
            {
                //Console.WriteLine("--Resolving Frame " + i + " references");
                miniFrame.ResolveReferences(parsedBNSA);
                i++;
            }
        }
    }
}
