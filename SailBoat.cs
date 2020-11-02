using System;

namespace Projekt_WPF_Hamnen
{
    public class SailBoat : Boat
    {
        public int SailBoatLenght { get; set; }

        public SailBoat(int[] position, int weight, int speed, int sailBoatLenght, string identityNumber, int harbour)
        {
            Weight = weight;
            Speed = speed;
            MaxDaysAtPort = 4;
            SailBoatLenght = sailBoatLenght;
            Position = position;
            IdentityNumber = identityNumber;
            Harbour = harbour;
        }
    }
}
