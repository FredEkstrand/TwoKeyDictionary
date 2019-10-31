using System;
using System.Collections.Generic;
using System.Text;
using Ekstrand.Collections.Generic;
using Ekstrand.Collections;

namespace NUnitTests
{
    public class PackTwoKeyDictionary
    {
        public string[] kavalues = new string[13];
        public int[] kbvalues = new int[13];
        public int[] vvalues = new int[13];
        public PackTwoKeyDictionary()
        {
            kavalues[(int)KnownExtendedColors.AbaloneShell] = "AbaloneShell";
            kavalues[(int)KnownExtendedColors.AbbeyWhite] = "AbbeyWhite";
            kavalues[(int)KnownExtendedColors.Abyss] = "Abyss";
            kavalues[(int)KnownExtendedColors.AcaciaHaze] = "AcaciaHaze";
            kavalues[(int)KnownExtendedColors.Acanthus] = "Acanthus";
            kavalues[(int)KnownExtendedColors.AcapulcoCliffs] = "AcapulcoCliffs";
            kavalues[(int)KnownExtendedColors.Acceleration] = "Acceleration";
            kavalues[(int)KnownExtendedColors.AccessibleBeige] = "AccessibleBeige";
            kavalues[(int)KnownExtendedColors.Accolade] = "Accolade";
            kavalues[(int)KnownExtendedColors.Acier] = "Acier";
            kavalues[(int)KnownExtendedColors.AcornSquash] = "AcornSquash";
            kavalues[(int)KnownExtendedColors.AdaptiveShade] = "AdaptiveShade";
            kavalues[(int)KnownExtendedColors.Admiralty] = "Admiralty";


            kbvalues[(int)KnownExtendedColors.AbaloneShell] = (int)KnownExtendedColors.AbaloneShell;
            kbvalues[(int)KnownExtendedColors.AbbeyWhite] = (int)KnownExtendedColors.AbbeyWhite;
            kbvalues[(int)KnownExtendedColors.Abyss] = (int)KnownExtendedColors.Abyss;
            kbvalues[(int)KnownExtendedColors.AcaciaHaze] = (int)KnownExtendedColors.AcaciaHaze;
            kbvalues[(int)KnownExtendedColors.Acanthus] = (int)KnownExtendedColors.Acanthus;
            kbvalues[(int)KnownExtendedColors.AcapulcoCliffs] = (int)KnownExtendedColors.AcapulcoCliffs;
            kbvalues[(int)KnownExtendedColors.Acceleration] = (int)KnownExtendedColors.Acceleration;
            kbvalues[(int)KnownExtendedColors.AccessibleBeige] = (int)KnownExtendedColors.AccessibleBeige;
            kbvalues[(int)KnownExtendedColors.Accolade] = (int)KnownExtendedColors.Accolade;
            kbvalues[(int)KnownExtendedColors.Acier] = (int)KnownExtendedColors.Acier;
            kbvalues[(int)KnownExtendedColors.AcornSquash] = (int)KnownExtendedColors.AcornSquash;
            kbvalues[(int)KnownExtendedColors.AdaptiveShade] = (int)KnownExtendedColors.AdaptiveShade;
            kbvalues[(int)KnownExtendedColors.Admiralty] = (int)KnownExtendedColors.Admiralty;


            vvalues[(int)KnownExtendedColors.AbaloneShell] = unchecked((int)0xFFDBC7BD);
            vvalues[(int)KnownExtendedColors.AbbeyWhite] = unchecked((int)0xFFECE5D0);
            vvalues[(int)KnownExtendedColors.Abyss] = unchecked((int)0xFFE0E6EB);
            vvalues[(int)KnownExtendedColors.AcaciaHaze] = unchecked((int)0xFF969C92);
            vvalues[(int)KnownExtendedColors.Acanthus] = unchecked((int)0xFFCDCDB4);
            vvalues[(int)KnownExtendedColors.AcapulcoCliffs] = unchecked((int)0xFF4B99A9);
            vvalues[(int)KnownExtendedColors.Acceleration] = unchecked((int)0xFF8092A3);
            vvalues[(int)KnownExtendedColors.AccessibleBeige] = unchecked((int)0xFFD1C7B8);
            vvalues[(int)KnownExtendedColors.Accolade] = unchecked((int)0xFFF2EEE2);
            vvalues[(int)KnownExtendedColors.Acier] = unchecked((int)0xFF9E9991);
            vvalues[(int)KnownExtendedColors.AcornSquash] = unchecked((int)0xFFFFDF37);
            vvalues[(int)KnownExtendedColors.AdaptiveShade] = unchecked((int)0xFF867E70);
            vvalues[(int)KnownExtendedColors.Admiralty] = unchecked((int)0xFF404D62);
        }

        public TwoKeyDictionary<string,int,int> GetLoadedTwoKeyDictionary(int size)
        {
            TwoKeyDictionary<string, int, int> tkd = new TwoKeyDictionary<string, int, int>();
            for (int i = 0; i < size; i++)
            {
                tkd.Add(kavalues[i], kbvalues[i], vvalues[i]);
            }

            return tkd;
        }
    }
}
