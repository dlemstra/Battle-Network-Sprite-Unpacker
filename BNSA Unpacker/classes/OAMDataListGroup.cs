﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BNSA_Unpacker.classes
{
    class OAMDataListGroup
    {
        public long Pointer;
        public List<OAMDataList> OAMDataLists = new List<OAMDataList>();
        public OAMDataListGroup(FileStream stream)
        {
            Pointer = stream.Position;
            Console.WriteLine("--Reading OAM Data List Pointer Table (Group) at 0x" + stream.Position.ToString("X2"));

            int firstOAMEntryPointer = int.MaxValue;

            while (stream.Position < firstOAMEntryPointer + Pointer)
            {
                int oamDataEntryPointer = BNSAFile.ReadIntegerFromStream(stream);
                firstOAMEntryPointer = Math.Min(firstOAMEntryPointer, oamDataEntryPointer); //should only be triggered by the first pointer as it goes ascending.
                long nextPosition = stream.Position; //Address of next pointer in the list
                stream.Seek(oamDataEntryPointer + Pointer, SeekOrigin.Begin);
                OAMDataList oamDataList = new OAMDataList(stream);

                OAMDataLists.Add(oamDataList);
                if (nextPosition < firstOAMEntryPointer + Pointer)
                {
                    //Read the next 4 bytes in the pointer table as its a new pointer
                    stream.Seek(nextPosition, SeekOrigin.Begin);
                }
            }
        }

        internal void Export(string outputDirectory, int oamDataListGroupIndex)
        {
            int i = 0;
            foreach (OAMDataList oamDataList in OAMDataLists)
            {
                //Console.WriteLine("--Resolving Frame " + i + " references");
                oamDataList.Export(outputDirectory, oamDataListGroupIndex, i);
                i++;
            }
        }
    }
}
