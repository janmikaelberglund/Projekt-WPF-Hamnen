using System;

namespace Projekt_WPF_Hamnen
{
    public class RowBoat : Boat
    {
        public int MaxPassengers { get; set; }

        public RowBoat(int[] position, int weight, int speed, int maxPassengers, string identityNumber, int harbour)
        {
            Weight = weight;
            Speed = speed;
            MaxDaysAtPort = 1;
            MaxPassengers = maxPassengers;
            Position = position;
            IdentityNumber = identityNumber;
            Harbour = harbour;
        }
    }
}
