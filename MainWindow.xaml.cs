using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.IO;

namespace Projekt_WPF_Hamnen
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public static Random random = new Random();
        static int harbourSpace = 64;
        static int[] harbour1 = new int[harbourSpace];
        static int[] harbour2 = new int[harbourSpace];
        static int dismissedShips = 0;
        static int day = 0;

        public static List<Boat> boats = new List<Boat>();
        static string[] textFileBoats;
        static string[] textFileMiscInfo;
        public MainWindow()
        {
            InitializeComponent();
            ImportProgramState();
            PlaceBoats();
            FillMyListBoxHarbour();
            if (!File.Exists("Saved Boats.txt") || new FileInfo("Saved Boats.txt").Length != 0)
            {
                FillmyListBoxStatistics();
            }
            labelDay.Text = $" Dag: {day}";
        }

        private void FillMyListBoxHarbour()
        {
            myListBoxHarbour.Items.Clear();
            myListBoxHarbour.Items.Add("Plats\tBåttyp\t\tNr\tVikt\tKnop\tÖvrigt");
            var q1 = boats
                    .OrderBy(b => b.Position[0])
                    .GroupBy(b => b.Harbour);

            int count = 1;
            foreach (var harbour in q1)
            {
                myListBoxHarbour.Items.Add($"Hamn {count}:");
                count++;
                foreach (var boat in harbour)
                {
                    myListBoxHarbour.Items.Add($"{GetHarbourPosition(boat)}\t{GetBoatType(boat)}\t{boat.IdentityNumber}\t{boat.Weight}\t{boat.Speed}\t{GetSpecialAttribute(boat)}");
                }
            }
        }

        private string GetSpecialAttribute(Boat boat)
        {
            switch (boat.IdentityNumber[0])
            {
                case 'R':
                    return $"Passagerarkapacitet: {((RowBoat)boat).MaxPassengers}st";
                case 'M':
                    return $"Hästkrafter: {((MotorBoat)boat).HorsePower} hk";
                case 'S':
                    return $"Längd: {Math.Round(((SailBoat)boat).SailBoatLenght / 3.2808399, 0)} fot";
                case 'K':
                    return $"Sängar: {((Catamaran)boat).Beds}st";
                case 'L':
                    return $"Containrar: {((CargoShip)boat).ContainersCarried}st";
                default:
                    return null;
            }
        }

        private string GetBoatType(Boat boat)
        {
            switch (boat.IdentityNumber[0])
            {
                case 'R':
                    return "Roddbåt\t";
                case 'M':
                    return "Motorbåt";
                case 'S':
                    return "Segelbåt\t";
                case 'K':
                    return "Katamaran";
                case 'L':
                    return "Lastfartyg";
                default:
                    return null;
            }
        }

        private string GetHarbourPosition(Boat boat) // ändra sorteringen så hamn1 alltid är överst
        {
            if (boat is RowBoat)
            {
                return (boat.Position[0] / 2 + 1).ToString();
            }
            else if (boat is MotorBoat)
            {
                return (boat.Position[0] / 2 + 1).ToString();
            }
            else
            {
                return $"{(boat.Position[0]) / 2 + 1}-{(boat.Position[boat.Position.Length-1]+ 1) / 2}";
            }
        }

        private void PlaceImage(string boatType, int position, int harbour)
        {
            Image image = new Image();
            BitmapImage boatImage = new BitmapImage();
            boatImage.BeginInit();
            boatImage.UriSource = new Uri(AppDomain.CurrentDomain.BaseDirectory + boatType);
            boatImage.EndInit();
            image.Source = boatImage;
            image.Margin = new Thickness(position * 22 + 1 , 1, 1, 1);
            if (harbour == 1)
            {
                harbour1Pictures.Children.Add(image);
            }
            else if (harbour == 2)
            {
                harbour2Pictures.Children.Add(image);
            }
        }

        private static void ExportProgramState()
        {
            using (StreamWriter sw = new StreamWriter("Saved Boats.txt"))
            {
                foreach (var boat in boats)
                {
                    string position = null;
                    foreach (var element in boat.Position)
                    {
                        position += element + ";";
                    }
                    position = position[..^1];
                    int uniqueProp = 0;
                    if (boat is RowBoat)
                    {
                        uniqueProp = ((RowBoat)boat).MaxPassengers;
                    }
                    else if (boat is MotorBoat)
                    {
                        uniqueProp = ((MotorBoat)boat).HorsePower;
                    }
                    else if (boat is SailBoat)
                    {
                        uniqueProp = ((SailBoat)boat).SailBoatLenght;
                    }
                    else if (boat is Catamaran)
                    {
                        uniqueProp = ((Catamaran)boat).Beds;
                    }
                    else if (boat is CargoShip)
                    {
                        uniqueProp = ((CargoShip)boat).ContainersCarried;
                    }
                    string writeBoatInfo = boat.IdentityNumber + "," + boat.Weight + "," + boat.Speed + "," + position
                                           + "," + boat.CurrentDaysAtPort + "," + uniqueProp + "," + boat.Harbour;
                    sw.WriteLine(writeBoatInfo);
                }
            }

            using (StreamWriter sw = new StreamWriter("Saved Misc Info.txt"))
            {
                sw.WriteLine(day);
                sw.WriteLine(dismissedShips);
                string harbourString = null;

                foreach (var element in harbour1)
                {
                    harbourString += element + ",";
                }
                harbourString = harbourString[..^1];
                sw.WriteLine(harbourString);

                harbourString = null;
                foreach (var element in harbour2)
                {
                    harbourString += element + ",";
                }
                harbourString = harbourString[..^1];
                sw.WriteLine(harbourString);
            }
        }

        public static void ImportProgramState()
        {
            if (!File.Exists("Saved Boats.txt"))
            {
                using (_ = new StreamWriter("Saved Boats.txt")) { };
            }
            else if (new FileInfo("Saved Boats.txt").Length != 0)
            {
                textFileBoats = File.ReadAllLines("Saved Boats.txt");
                string[] tempBoat = new string[6];
                foreach (var line in textFileBoats)
                {
                    tempBoat = line.Split(',');
                    int[] tempPosition = Array.ConvertAll(tempBoat[3].Split(';'), int.Parse);
                    switch (tempBoat[0].First())
                    {
                        case 'R':
                            boats.Add(new RowBoat(tempPosition, int.Parse(tempBoat[1]), int.Parse(tempBoat[2]), int.Parse(tempBoat[5])
                                      , tempBoat[0], int.Parse(tempBoat[6]))
                            { CurrentDaysAtPort = int.Parse(tempBoat[4]) });
                            break;
                        case 'M':
                            boats.Add(new MotorBoat(tempPosition, int.Parse(tempBoat[1]), int.Parse(tempBoat[2]), int.Parse(tempBoat[5])
                                      , tempBoat[0], int.Parse(tempBoat[6]))
                            { CurrentDaysAtPort = int.Parse(tempBoat[4]) });
                            break;
                        case 'S':
                            boats.Add(new SailBoat(tempPosition, int.Parse(tempBoat[1]), int.Parse(tempBoat[2]), int.Parse(tempBoat[5])
                                      , tempBoat[0], int.Parse(tempBoat[6]))
                            { CurrentDaysAtPort = int.Parse(tempBoat[4]) });
                            break;
                        case 'K':
                            boats.Add(new Catamaran(tempPosition, int.Parse(tempBoat[1]), int.Parse(tempBoat[2]), int.Parse(tempBoat[5])
                                      , tempBoat[0], int.Parse(tempBoat[6]))
                            { CurrentDaysAtPort = int.Parse(tempBoat[4]) });
                            break;
                        case 'L':
                            boats.Add(new CargoShip(tempPosition, int.Parse(tempBoat[1]), int.Parse(tempBoat[2]), int.Parse(tempBoat[5])
                                      , tempBoat[0], int.Parse(tempBoat[6]))
                            { CurrentDaysAtPort = int.Parse(tempBoat[4]) });
                            break;
                        default:
                            break;
                    }
                }
            }
            if (!File.Exists("Saved Misc Info.txt"))
            {
                using (_ = new StreamWriter("Saved Misc Info.txt")) { };
            }
            else if (new FileInfo("Saved Misc Info.txt").Length != 0)
            {
                textFileMiscInfo = File.ReadAllLines("Saved Misc Info.txt");
                day = int.Parse(textFileMiscInfo[0]);
                dismissedShips = int.Parse(textFileMiscInfo[1]);
                string[] tempHarbour1 = textFileMiscInfo[2].Split(',');
                string[] tempHarbour2 = textFileMiscInfo[3].Split(',');

                for (int i = 0; i < tempHarbour1.Count(); i++)
                {
                    harbour1[i] = int.Parse(tempHarbour1[i]);
                }

                for (int i = 0; i < tempHarbour2.Count(); i++)
                {
                    harbour2[i] = int.Parse(tempHarbour2[i]);
                }

            }
        }

        private static void RemoveBoats()
        {
            List<Boat> boatsToRemove = new List<Boat>();
            foreach (var boat in boats)
            {
                boat.CurrentDaysAtPort++;
                if (boat.CurrentDaysAtPort >= boat.MaxDaysAtPort)
                {
                    boatsToRemove.Add(boat);
                    if (boat is RowBoat)
                    {
                        if (boat.Harbour == 1)
                        {
                            harbour1[boat.Position[0]] = 0;
                        }
                        else if (boat.Harbour == 2)
                        {
                            harbour2[boat.Position[0]] = 0;
                        }
                    }
                    else if (boat is MotorBoat)
                    {
                        if (boat.Harbour == 1)
                        {
                            for (int i = 0; i < 2; i++)
                            {
                                harbour1[boat.Position[i]] = 0;
                            }
                        }

                        else if (boat.Harbour == 2)
                        {
                            for (int i = 0; i < 2; i++)
                            {
                                harbour2[boat.Position[i]] = 0;
                            }
                        }
                    }
                    else if (boat is SailBoat)
                    {
                        if (boat.Harbour == 1)
                        {
                            for (int i = 0; i < 4; i++)
                            {
                                harbour1[boat.Position[i]] = 0;
                            }
                        }

                        else if (boat.Harbour == 2)
                        {
                            for (int i = 0; i < 4; i++)
                            {
                                harbour2[boat.Position[i]] = 0;
                            }
                        }
                    }
                    else if (boat is Catamaran)
                    {
                        if (boat.Harbour == 1)
                        {
                            for (int i = 0; i < 6; i++)
                            {
                                harbour1[boat.Position[i]] = 0;
                            }
                        }

                        else if (boat.Harbour == 2)
                        {
                            for (int i = 0; i < 6; i++)
                            {
                                harbour2[boat.Position[i]] = 0;
                            }
                        }
                    }
                    else if (boat is CargoShip)
                    {
                        if (boat.Harbour == 1)
                        {
                            for (int i = 0; i < 8; i++)
                            {
                                harbour1[boat.Position[i]] = 0;
                            }
                        }

                        else if (boat.Harbour == 2)
                        {
                            for (int i = 0; i < 8; i++)
                            {
                                harbour2[boat.Position[i]] = 0;
                            }
                        }
                    }
                }
            }

            foreach (var boat in boatsToRemove)
            {
                boats.Remove(boat);
            }
        }

        private static void CreateNewBoats(int numberOfBoats)
        {
            for (int i = 0; i < numberOfBoats; i++)
            {
                int rng = random.Next(1, 5 + 1);
                bool filledSpot = false;
                switch (rng)
                {
                    case 1:
                        for (int j = 0; j < harbourSpace - 1; j++)
                        {
                            if (HarbourHasSpace("RowBoat", j, harbour1))
                            {
                                int[] position = { j };
                                boats.Add(new RowBoat(position, random.Next(100, 300 + 1), random.Next(0, 3 + 1), random.Next(1, 6 + 1), CreateIdentityNumber("R"), 1));
                                harbour1[j] = 1;
                                filledSpot = true;
                                break;
                            }
                        }

                        if (!filledSpot)
                        {
                            for (int j = 0; j < harbourSpace - 1; j++)
                            {
                                if (HarbourHasSpace("RowBoat", j, harbour2))
                                {
                                    int[] position = { j };
                                    boats.Add(new RowBoat(position, random.Next(100, 300 + 1), random.Next(0, 3 + 1), random.Next(1, 6 + 1), CreateIdentityNumber("R"), 2));
                                    harbour2[j] = 1;
                                    filledSpot = true;
                                    break;
                                }
                            }
                        }
                        if (!filledSpot)
                        {
                            dismissedShips++;
                        }
                        break;

                    case 2:
                        for (int j = harbourSpace - 2; j >= 0; j -= 2)
                        {
                            if (HarbourHasSpace("MotorBoat", j, harbour2) && j % 2 == 0)
                            {
                                int[] position = { j, j + 1 };
                                boats.Add(new MotorBoat(position, random.Next(200, 3000 + 1), random.Next(0, 60 + 1), random.Next(10, 1000 + 1), CreateIdentityNumber("M"), 2));
                                for (int k = 0; k < 2; k++)
                                {
                                    harbour2[j + k] = 2;
                                }
                                filledSpot = true;
                                break;
                            }
                        }

                        if (!filledSpot)
                        {
                            for (int j = harbourSpace - 2; j >= 0; j -= 2)
                            {
                                if (HarbourHasSpace("MotorBoat", j, harbour1) && j % 2 == 0)
                                {
                                    int[] position = { j, j + 1 };
                                    boats.Add(new MotorBoat(position, random.Next(200, 3000 + 1), random.Next(0, 60 + 1), random.Next(10, 1000 + 1), CreateIdentityNumber("M"), 1));
                                    for (int k = 0; k < 2; k++)
                                    {
                                        harbour1[j + k] = 2;
                                    }
                                    filledSpot = true;
                                    break;
                                }
                            }
                        }

                        if (!filledSpot)
                        {
                            dismissedShips++;
                        }
                        break;

                    case 3:
                        for (int j = harbourSpace - 4; j >= 0; j -= 2)
                        {
                            if (HarbourHasSpace("SailBoat", j, harbour2) && j % 2 == 0)
                            {
                                int[] position = { j, j + 1, j + 2, j + 3 };
                                boats.Add(new SailBoat(position, random.Next(800, 6000 + 1), random.Next(0, 12 + 1), random.Next(10, 60 + 1), CreateIdentityNumber("S"), 2));
                                for (int k = 0; k < 4; k++)
                                {
                                    harbour2[j + k] = 3;
                                }
                                filledSpot = true;
                                break;
                            }
                        }

                        if (!filledSpot)
                        {
                            for (int j = harbourSpace - 4; j >= 0; j -= 2)
                            {
                                if (HarbourHasSpace("SailBoat", j, harbour1) && j % 2 == 0)
                                {
                                    int[] position = { j, j + 1, j + 2, j + 3 };
                                    boats.Add(new SailBoat(position, random.Next(800, 6000 + 1), random.Next(0, 12 + 1), random.Next(10, 60 + 1), CreateIdentityNumber("S"), 1));
                                    for (int k = 0; k < 4; k++)
                                    {
                                        harbour1[j + k] = 3;
                                    }
                                    filledSpot = true;
                                    break;
                                }
                            }
                        }

                        if (!filledSpot)
                        {
                            dismissedShips++;
                        }
                        break;

                    case 4:
                        for (int j = harbourSpace - 6; j >= 0; j -= 2)
                        {
                            if (HarbourHasSpace("Catamaran", j, harbour2) && j % 2 == 0)
                            {
                                int[] position = { j, j + 1, j + 2, j + 3, j + 4, j + 5 };
                                boats.Add(new Catamaran(position, random.Next(1200, 8000 + 1), random.Next(0, 12 + 1), random.Next(1, 4 + 1), CreateIdentityNumber("K"), 2));
                                for (int k = 0; k < 6; k++)
                                {
                                    harbour2[j + k] = 4;
                                }
                                filledSpot = true;
                                break;
                            }
                        }

                        if (!filledSpot)
                        {
                            for (int j = harbourSpace - 6; j >= 0; j -= 2)
                            {
                                if (HarbourHasSpace("Catamaran", j, harbour1) && j % 2 == 0)
                                {
                                    int[] position = { j, j + 1, j + 2, j + 3, j + 4, j + 5 };
                                    boats.Add(new Catamaran(position, random.Next(1200, 8000 + 1), random.Next(0, 12 + 1), random.Next(1, 4 + 1), CreateIdentityNumber("K"), 1));
                                    for (int k = 0; k < 6; k++)
                                    {
                                        harbour1[j + k] = 4;
                                    }
                                    filledSpot = true;
                                    break;
                                }
                            }
                        }
                        if (!filledSpot)
                        {
                            dismissedShips++;
                        }
                        break;

                    case 5:
                        for (int j = harbourSpace - 8; j >= 0; j -= 2)
                        {
                            if (HarbourHasSpace("CargoShip", j, harbour2) && j % 2 == 0)
                            {
                                int[] position = { j, j + 1, j + 2, j + 3, j + 4, j + 5, j + 6, j + 7 };
                                boats.Add(new CargoShip(position, random.Next(3000, 20000 + 1), random.Next(0, 20 + 1), random.Next(0, 500 + 1), CreateIdentityNumber("L"), 2));
                                for (int k = 0; k < 8; k++)
                                {
                                    harbour2[j + k] = 5;
                                }
                                filledSpot = true;
                                break;
                            }
                        }

                        if (!filledSpot)
                        {
                            for (int j = harbourSpace - 8; j >= 0; j -= 2)
                            {
                                if (HarbourHasSpace("CargoShip", j, harbour1) && j % 2 == 0)
                                {
                                    int[] position = { j, j + 1, j + 2, j + 3, j + 4, j + 5, j + 6, j + 7 };
                                    boats.Add(new CargoShip(position, random.Next(3000, 20000 + 1), random.Next(0, 20 + 1), random.Next(0, 500 + 1), CreateIdentityNumber("L"), 1));
                                    for (int k = 0; k < 8; k++)
                                    {
                                        harbour1[j + k] = 5;
                                    }
                                    filledSpot = true;
                                    break;
                                }
                            }
                        }

                        if (!filledSpot)
                        {
                            dismissedShips++;
                        }
                        break;
                    default:
                        break;
                }
            }
        }
        private static string CreateIdentityNumber(string firstLetter)
        {
            bool testing = true;
            string testString = null;
            while (testing)
            {
                testString = firstLetter + "-";
                for (int i = 0; i < 3; i++)
                {
                    int number = random.Next(0, 25 + 1);
                    char letter = (char)('a' + number);
                    testString += letter;
                }
                int count = 0;
                foreach (var boat in boats)
                {
                    if (testString == boat.IdentityNumber)
                    {
                        break;
                    }
                    count++;
                }
                if (count == boats.Count)
                {
                    testing = false;
                }
            }
            return testString.ToUpper();
        }
        public static bool HarbourHasSpace(string boatType, int position, int[] harbour)
        {
            switch (boatType)
            {
                case "RowBoat":
                    if (harbour[position] == 0)
                    {
                        return true;
                    }
                    break;
                case "MotorBoat":
                    if (harbour[position] == 0 && harbour[position + 1] == 0)
                    {
                        return true;
                    }
                    break;
                case "SailBoat":
                    if (harbour[position] == 0 && harbour[position + 1] == 0 && harbour[position + 2] == 0 && harbour[position + 3] == 0)
                    {
                        return true;
                    }
                    break;
                case "Catamaran":
                    if (harbour[position] == 0 && harbour[position + 1] == 0 && harbour[position + 2] == 0
                        && harbour[position + 3] == 0 && harbour[position + 4] == 0 && harbour[position + 5] == 0)
                    {
                        return true;
                    }
                    break;
                case "CargoShip":
                    if (harbour[position] == 0 && harbour[position + 1] == 0 && harbour[position + 2] == 0 && harbour[position + 3] == 0
                        && harbour[position + 4] == 0 && harbour[position + 5] == 0 && harbour[position + 6] == 0 && harbour[position + 7] == 0)
                    {
                        return true;
                    }
                    break;
                default:
                    break;
            }
            return false;
        }

        private void NewDay_click(object sender, RoutedEventArgs e)
        {
            bool tryDays = int.TryParse(numberOfDays.Text, out int daysAmount);
            if (tryDays)
            {
                for (int i = 0; i < daysAmount; i++)
                {
                    RemoveBoats();
                    CreateNewBoats(5);
                    day++;
                }
                PlaceBoats();
                ExportProgramState();
                labelDay.Text = $" Dag: {day}";
            }
            if (!File.Exists("Saved Boats.txt") || new FileInfo("Saved Boats.txt").Length != 0)
            {
                FillmyListBoxStatistics();
            }
            FillMyListBoxHarbour();
        }
        private void FillmyListBoxStatistics()
        {
            myListBoxStatistics.Items.Clear();
            string outputText = null;
            var q1 = boats
                    .OrderBy(b => b.MaxDaysAtPort)
                    .GroupBy(b => b.IdentityNumber[0]);

            foreach (var boat in q1)
            {
                if (boat.Key == 'R')
                {
                    outputText = $"Roddbåtar: {boat.Count()} | ";
                }
                else if (boat.Key == 'M')
                {
                    outputText += $"Motorbåtar: {boat.Count()} | ";
                }
                else if (boat.Key == 'S')
                {
                    outputText += $"Segelbåtar: {boat.Count()} | ";
                }
                else if (boat.Key == 'K')
                {
                    outputText += $"Katamaraner: {boat.Count()} | ";
                }
                else if (boat.Key == 'L')
                {
                    outputText += $"Lastfartyg: {boat.Count()}";
                }
            }
            myListBoxStatistics.Items.Add(outputText);

            var q2 = boats
                    .Sum(b => b.Weight);
            myListBoxStatistics.Items.Add($"Total vikt av hamnens båtar: {q2}kg");

            var q3 = boats
                    .Average(b => b.Speed);
            myListBoxStatistics.Items.Add($"Medelhastigheten av hamnens båtar: {Math.Round(q3 * 1.852, 1)}km/h");

            double q4 = 0;
            foreach (var spot in harbour1)
            {
                if (spot == 0)
                {
                    q4++;
                }
            }
            foreach (var spot in harbour2)
            {
                if (spot == 0)
                {
                    q4++;
                }
            }
            q4 /= 2;
            myListBoxStatistics.Items.Add($"{q4} platser lediga.");
            myListBoxStatistics.Items.Add($"Antal avfärdade båtar: {dismissedShips}");
        }

        private void PlaceBoats()
        {
            harbour1Pictures.Children.Clear();
            harbour2Pictures.Children.Clear();
            foreach (var boat in boats)
            {
                if (boat is RowBoat)
                {
                    PlaceImage("rowingboat.bmp", boat.Position[0], boat.Harbour);
                }
                else if (boat is MotorBoat)
                {
                    PlaceImage("motorboat.bmp", boat.Position[0], boat.Harbour);
                }
                else if (boat is SailBoat)
                {
                    PlaceImage("sailboat.bmp", boat.Position[0], boat.Harbour);
                }
                else if (boat is Catamaran)
                {
                    PlaceImage("catamaran.bmp", boat.Position[0], boat.Harbour);
                }
                else if (boat is CargoShip)
                {
                    PlaceImage("cargoship.bmp", boat.Position[0], boat.Harbour);
                }
            }
        }

        private void Remove_Information_Click(object sender, RoutedEventArgs e)
        {
            using (StreamWriter sw = new StreamWriter("Saved Boats.txt")) { };
            using (StreamWriter sw = new StreamWriter("Saved Misc Info.txt")) { };
            day = 0;
            dismissedShips = 0;
            harbour1 = new int[harbourSpace];
            harbour2 = new int[harbourSpace];
            boats = new List<Boat>();
            myListBoxStatistics.Items.Clear();
            myListBoxHarbour.Items.Clear();
            PlaceBoats();
            if (!File.Exists("Saved Boats.txt") || new FileInfo("Saved Boats.txt").Length != 0)
            {
                FillmyListBoxStatistics();
            }
            labelDay.Text = $" Dag: {day}";
        }
    }
}
