using System;

namespace Projekt_WPF_Hamnen
{
    public class CargoShip : Boat
    {
        public int ContainersCarried { get; set; }

        public CargoShip(int[] position, int weight, int speed, int containersCarried, string identityNumber, int harbour)
        {
            Weight = weight;
            Speed = speed;
            MaxDaysAtPort = 6;
            ContainersCarried = containersCarried;
            Position = position;
            IdentityNumber = identityNumber;
            Harbour = harbour;
        }
    }
}
