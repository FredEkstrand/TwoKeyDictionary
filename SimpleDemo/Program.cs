using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Ekstrand.Collections.Generic;

namespace ConsoleTester
{
    class Program
    {
        private static TwoKeyDictionary<string, CrayolaCrayons, string> CrayolaColors;
        static void Main(string[] args)
        {
            init();
            ColorExamples();
            Console.ReadLine();
        }

        private static void init()
        {
            CrayolaColors = new TwoKeyDictionary<string, CrayolaCrayons, string>();
            for(int i = 0; i < HexColor.Length; i++)
            {
                CrayolaColors.Add(CrayolaNames[i], (CrayolaCrayons)i, HexColor[i]);
            }
        }

        private static void ColorExamples()
        {
            Console.WriteLine("Initializing two key dictionary for colors.");
            Console.WriteLine("TwoKeyDictionary<string, CrayolaCrayons, string>");
            Console.WriteLine("Key-A = color name, Key-B = enum value, Value = hex color value.");
            Console.WriteLine("Number of CrayolaCrayons entries: {0}\n", CrayolaColors.Count);
            Console.WriteLine("Example 1: Index by A-Key for hex color value.");
           
            string result = "FF" + CrayolaColors["AntiqueBrass"]; 
            Console.WriteLine("CrayolaColors[\"AntiqueBrass\"] == {0} HexColor index 18 = {1}",result, "FFCD9575");

            result = "FF" + CrayolaColors["Fern"]; 
            Console.WriteLine("CrayolaColors[\"Fern\"] == {0} HexColor index 34 = {1}", result, "FF71BC78");

            Console.WriteLine("\nExample 2: Index by B-Key for hex color value.");
            
            result = "FF" + CrayolaColors[CrayolaCrayons.Fern];
            Console.WriteLine("CrayolaColors[CrayolaCrayons.Fern] == {0} HexColor index 34 = {1}", result, "FF71BC78");
           
            result = "FF" + CrayolaColors[CrayolaCrayons.AntiqueBrass];
            Console.WriteLine("CrayolaColors[CrayolaCrayons.AntiqueBrass] == {0} HexColor index 18 = {1}", result, "FFCD9575");

            Console.WriteLine("\nExample 3: Remove Key-A.");
            Console.WriteLine("CrayolaColors.RemoveKeyA(\"Bittersweet\")\n");
            CrayolaColors.RemoveKeyA("Bittersweet"); // goodbye 

            Console.WriteLine("Now check if the corresponding Key-A is removed.");
            bool bresult = CrayolaColors.ContainsKeyA("Bittersweet");
            Console.WriteLine("CrayolaColors.ContainsKeyA(\"Bittersweet\") {0}\n", bresult);


            Console.WriteLine("Now check if the corresponding Key-B is removed.");
            bresult = CrayolaColors.ContainsKeyB(CrayolaCrayons.Bittersweet);
            Console.WriteLine("CrayolaColors.ContainsKeyB(CrayolaCrayons.Bittersweet) {0}\n",bresult);
            Console.WriteLine("Number of CrayolaCrayons entries: {0}\n", CrayolaColors.Count);

        }

        public enum CrayolaCrayons
        {
            Mahogany = 0,
            FuzzyWuzzyBrown,
            Chestnut,
            RedOrange,
            SunsetOrange,
            Bittersweet,
            Melon,
            OutrageousOrange,
            VividTangerine,
            BurntSienna,
            Brown,
            Sepia,
            Orange,
            BurntOrange,
            Copper,
            MangoTango,
            AtomicTangerine,
            Beaver,
            AntiqueBrass,
            DesertSand,
            RawSienna,
            Tumbleweed,
            Tan,
            Peach,
            MacaroniandCheese,
            Apricot,
            NeonCarrot,
            Almond,
            YellowOrange,
            Gold,
            Shadow,
            BananaMania,
            Sunglow,
            Goldenrod,
            Dandelion,
            Yellow,
            GreenYellow,
            SpringGreen,
            OliveGreen,
            LaserLemon,
            UnmellowYellow,
            Canary,
            YellowGreen,
            InchWorm,
            Asparagus,
            GrannySmithApple,
            ElectricLime,
            ScreaminGreen,
            Fern,
            ForestGreen,
            SeaGreen,
            Green,
            MountainMeadow,
            Shamrock,
            JungleGreen,
            CaribbeanGreen,
            TropicalRainForest,
            PineGreen,
            RobinEggBlue,
            Aquamarine,
            TurquoiseBlue,
            SkyBlue,
            OuterSpace,
            BlueGreen,
            PacificBlue,
            Cerulean,
            Cornflower,
            MidnightBlue,
            NavyBlue,
            Denim,
            Blue,
            Periwinkle,
            CadetBlue,
            Indigo,
            WildBlueYonder,
            Manatee,
            BlueBell,
            BlueViolet,
            PurpleHeart,
            RoyalPurple,
            PurpleMountainsMajesty,
            Violet,
            Wisteria,
            VividViolet,
            Fuchsia,
            ShockingPink,
            PinkFlamingo,
            Plum,
            HotMagenta,
            PurplePizzazz,
            RazzleDazzleRose,
            Orchid,
            RedViolet,
            Eggplant,
            Cerise,
            WildStrawberry,
            Magenta,
            Lavender,
            CottonCandy,
            VioletRed,
            CarnationPink,
            Razzmatazz,
            PiggyPink,
            JazzberryJam,
            Blush,
            TickleMePink,
            PinkSherbet,
            Maroon,
            Red,
            RadicalRed,
            Mauvelous,
            WildWatermelon,
            Scarlet,
            Salmon,
            BrickRed,
            White,
            Timberwolf,
            Silver,
            Gray,
            Black

        }

        public static string[] HexColor = new string[]
        {
            "CD4A4A","CC6666","BC5D58","FF5349","FD5E53","FD7C6E","FDBCB4","FF6E4A","FFA089","EA7E5D",
            "B4674D","A5694F","FF7538","FF7F49","DD9475","FF8243","FFA474","9F8170","CD9575","EFCDB8",
            "D68A59","DEAA88","FAA76C","FFCFAB","FFBD88","FDD9B5","FFA343","EFDBC5","FFB653","E7C697",
            "8A795D","FAE7B5","FFCF48","FCD975","FDDB6D","FCE883","F0E891","ECEABE","BAB86C","FDFC74",
            "FDFC74","FFFF99","C5E384","B2EC5D","87A96B","A8E4A0","1DF914","76FF7A","71BC78","6DAE81",
            "9FE2BF","1CAC78","30BA8F","45CEA2","3BB08F","1CD3A2","17806D","158078","1FCECB","78DBE2",
            "77DDE7","80DAEB","414A4C","199EBD","1CA9C9","1DACD6","9ACEEB","1A4876","1974D2","2B6CC4",
            "1F75FE","C5D0E6","B0B7C6","5D76CB","A2ADD0","979AAA","ADADD6","7366BD","7442C8","7851A9",
            "9D81BA","926EAE","CDA4DE","8F509D","C364C5","FB7EFD","FC74FD","8E4585","FF1DCE","FF1DCE",
            "FF48D0","E6A8D7","C0448F","6E5160","DD4492","FF43A4","F664AF","FCB4D5","FFBCD9","F75394",
            "FFAACC","E3256B","FDD7E4","CA3767","DE5D83","FC89AC","F780A1","C8385A","EE204D","FF496C",
            "EF98AA","FC6C85","FC2847","FF9BAA","CB4154","EDEDED","DBD7D2","CDC5C2","95918C","232323"        };

        public static string[] CrayolaNames = new string[]
        {
            "Mahogany", "FuzzyWuzzyBrown",  "Chestnut", "RedOrange",    "SunsetOrange", "Bittersweet",  "Melon",    "OutrageousOrange", "VividTangerine",   "BurntSienna",  "Brown",
            "Sepia",    "Orange",   "BurntOrange",  "Copper",   "MangoTango",   "AtomicTangerine",  "Beaver",   "AntiqueBrass", "DesertSand",   "RawSienna",    "Tumbleweed",
            "Tan",  "Peach",    "MacaroniandCheese",    "Apricot",  "NeonCarrot",   "Almond",   "YellowOrange", "Gold", "Shadow",   "BananaMania",  "Sunglow",
            "Goldenrod",    "Dandelion",    "Yellow",   "GreenYellow",  "SpringGreen",  "OliveGreen",   "LaserLemon",   "UnmellowYellow",   "Canary",   "YellowGreen",  "InchWorm",
            "Asparagus",    "GrannySmithApple", "ElectricLime", "ScreaminGreen",    "Fern", "ForestGreen",  "SeaGreen", "Green",    "MountainMeadow",   "Shamrock", "JungleGreen",
            "CaribbeanGreen",   "TropicalRainForest",   "PineGreen",    "RobinEggBlue", "Aquamarine",   "TurquoiseBlue",    "SkyBlue",  "OuterSpace",   "BlueGreen",    "PacificBlue",  "Cerulean",
            "Cornflower",   "MidnightBlue", "NavyBlue", "Denim",    "Blue", "Periwinkle",   "CadetBlue",    "Indigo",   "WildBlueYonder",   "Manatee",  "BlueBell",
            "BlueViolet",   "PurpleHeart",  "RoyalPurple",  "PurpleMountainsMajesty",   "Violet",   "Wisteria", "VividViolet",  "Fuchsia",  "ShockingPink", "PinkFlamingo", "Plum",
            "HotMagenta",   "PurplePizzazz",    "RazzleDazzleRose", "Orchid",   "RedViolet",    "Eggplant", "Cerise",   "WildStrawberry",   "Magenta",  "Lavender", "CottonCandy",
            "VioletRed",    "CarnationPink",    "Razzmatazz",   "PiggyPink",    "JazzberryJam", "Blush",    "TickleMePink", "PinkSherbet",  "Maroon",   "Red",  "RadicalRed",
            "Mauvelous",    "WildWatermelon",   "Scarlet",  "Salmon",   "BrickRed", "White",    "Timberwolf",   "Silver",   "Gray", "Black",

        };
    }
}
