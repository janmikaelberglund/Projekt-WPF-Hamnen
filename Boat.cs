using System;


namespace Projekt_WPF_Hamnen
{

    public class Boat
    {
        public string IdentityNumber { get; set; }
        public int Weight { get; set; }
        public int Speed { get; set; }
        public int MaxDaysAtPort { get; set; }
        public int CurrentDaysAtPort = 0;
        public int[] Position { get; set; }
        public int Harbour { get; set; }
    }
}
