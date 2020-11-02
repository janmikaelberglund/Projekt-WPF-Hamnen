using System;

namespace Projekt_WPF_Hamnen
{
    public class Catamaran : Boat
    {
        public int Beds { get; set; }

        public Catamaran(int[] position, int weight, int speed, int beds, string identityNumber, int harbour)
        {
            Weight = weight;
            Speed = speed;
            MaxDaysAtPort = 3;
            Beds = beds;
            Position = position;
            IdentityNumber = identityNumber;
            Harbour = harbour;
        }
    }
}
