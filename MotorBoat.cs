using System;

namespace Projekt_WPF_Hamnen
{
    public class MotorBoat : Boat
    {
        public int HorsePower { get; set; }

        public MotorBoat(int[] position, int weight, int speed, int horsepower, string identityNumber, int harbour)
        {
            Weight = weight;
            Speed = speed;
            MaxDaysAtPort = 3;
            HorsePower = horsepower;
            Position = position;
            IdentityNumber = identityNumber;
            Harbour = harbour;
        }
    }
}
