using System;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using System.Collections.Generic;

namespace Torpedo.Modell
{
    public class Table
    {
        int tableWidth = 9;

        public int TableWidth
        {
            get { return tableWidth; }
            set { tableWidth = value; }
        }

        int tableHeight = 9;

        public int TableHeight
        {
            get { return tableHeight; }
            set { tableHeight = value; }
        }

        List<Ship> shipList = new List<Ship>();

        public List<Ship> ShipList
        {
            get { return shipList; }
            set { shipList = value; }
        }

        //if value == 1 there is a ship
        //if 2 torpedoed water
        //if 3 torpedoed ship
        short[,] youTable;

        public short[,] YouTable
        {
            get { return youTable; }
            set { youTable = value; }
        }

        short[,] computerTable;

        public short[,] ComputerTable
        {
            get { return computerTable; }
            set { computerTable = value; }
        }

        short shipCout = 4;

        int stepCount = 0;

        public int StepCount
        {
            get { return stepCount; }
            set { stepCount = value; }
        }

        public Point? computerShot()
        {
            for (int i = 0; i < tableWidth; i++)
            {
                for (int j = 0; j < tableHeight; j++)
                {
                    if (youTable[i, j] == 3)
                    {
                        //ha egy bomba körül még nem bombáztunk
                        if ((i - 1 < 0 || youTable[i - 1, j] != 3) && (i + 1 >= tableWidth || youTable[i + 1, j] != 3) && (j - 1 < 0 || youTable[i, j - 1] != 3) && (j + 1 >= tableWidth || youTable[i, j + 1] != 3))
                        {

                            if (i - 1 >= 0 && (youTable[i - 1, j] == 0 || youTable[i - 1, j] == 1))
                                return new Point(i - 1, j);
                            if (i + 1 < tableWidth && (youTable[i + 1, j] == 0 || youTable[i + 1, j] == 1))
                                return new Point(i + 1, j);
                            if (j - 1 >= 0 && (youTable[i, j - 1] == 0 || youTable[i, j - 1] == 1))
                                return new Point(i, j - 1);
                            if (j + 1 < tableWidth && (youTable[i, j + 1] == 0 || youTable[i, j + 1] == 1))
                                return new Point(i, j + 1);
                        }

                        if (i + 1 < tableWidth && youTable[i + 1, j] == 3) 
                        {
                            if(i - 1 >= 0 && (youTable[i - 1, j] == 0 || youTable[i - 1, j] == 1))
                                return new Point(i - 1, j);
                            if(i + 2 < tableWidth && (youTable[i + 2, j] == 0 || youTable[i + 2, j] == 1))
                                return new Point(i + 2, j);                            
                        }

                        if (j + 1 < tableWidth && youTable[i, j + 1] == 3) 
                        {
                            if (j - 1 >= 0 && (youTable[i, j - 1] == 0 || youTable[i, j - 1] == 1))
                                return new Point(i, j - 1);
                            if (j + 2 < tableWidth && (youTable[i, j + 2] == 0 || youTable[i, j + 2] == 1))
                                return new Point(i, j + 2);
                        } 
                    }
                }
            }

            int freespaceCounter = 0;

            for (int i = 0; i < tableWidth; i++)
            {
                for (int j = 0; j < tableHeight; j++)
                {
                    if (youTable[i, j] == 0 || youTable[i, j] == 1)
                    {
                        ++freespaceCounter;
                    }
                }
            }

            Random rand = new Random();
            int shot = rand.Next(freespaceCounter);

            int counter = 0;
            for (int i = 0; i < tableWidth; i++)
            {
                for (int j = 0; j < tableHeight; j++)
                {
                    if (youTable[i, j] == 0 || youTable[i, j] == 1)
                    {

                        if (shot == counter)
                        {
                            return new Point(i, j);
                        }
                        ++counter;
                    }
                }
            }

            return null;
        }

        public bool checkYouWin()
        {
            int counter = 0;
            for (int i = 0; i < tableWidth; i++)
            {
                for (int j = 0; j < tableHeight; j++)
                {
                    if (computerTable[i, j] == 3)
                        ++counter;

                    if (counter >= 11)
                        return true;
                }
            }

            return false;
        }

        public bool checkComputerWin()
        {
            int counter = 0;
            for (int i = 0; i < tableWidth; i++)
            {
                for (int j = 0; j < tableHeight; j++)
                {
                    if (youTable[i, j] == 3)
                        ++counter;

                    if (counter >= 11)
                        return true;
                }
            }

            return false;
        }

        public Table()
        {
            youTable = new short[tableWidth, tableHeight];
            computerTable = new short[tableWidth, tableHeight];

            //inicializing your table
            for (int i = 0; i < shipCout; i++)
            {
                Ship ship = new Ship();

                if (i < 1)
                    ship.Length = 4;
                else if (i < 2)
                    ship.Length = 3;
                else
                    ship.Length = 2;

                Random rand = new Random();

                Point sp = new Point(rand.Next(tableWidth), rand.Next(tableHeight));
                ship.StartPoint = sp;
                ship.Direction = (short)rand.Next(2);

                bool validdeployment = true;
                if (ship.Direction == 0 && ship.StartPoint.X + ship.Length < tableWidth)
                {
                    for (int j = 0; j < ship.Length; j++)
                    {
                        if (youTable[(int)ship.StartPoint.X + j, (int)ship.StartPoint.Y] != 0)
                            validdeployment = false;
                    }
                }
                else if (ship.Direction == 1 && ship.StartPoint.Y + ship.Length < tableHeight)
                {
                    for (int j = 0; j < ship.Length; j++)
                    {
                        if (youTable[(int)ship.StartPoint.X, (int)ship.StartPoint.Y + j] != 0)
                            validdeployment = false;
                    }
                }
                else
                    validdeployment = false;

                if (validdeployment)
                {
                    for (int j = 0; j < ship.Length; j++)
                    {
                        if (ship.Direction == 0)
                            youTable[(int)ship.StartPoint.X + j, (int)ship.StartPoint.Y] = 1;
                        else
                            youTable[(int)ship.StartPoint.X, (int)ship.StartPoint.Y + j] = 1;
                    }

                    shipList.Add(ship);
                }
                else
                    --i;

            }

            //initializing computer table
            for (int i = 0; i < shipCout; i++)
            {
                Ship ship = new Ship();

                if (i < 1)
                    ship.Length = 4;
                else if (i < 2)
                    ship.Length = 3;
                else
                    ship.Length = 2;

                Random rand = new Random();

                Point sp = new Point(rand.Next(tableWidth), rand.Next(tableHeight));
                ship.StartPoint = sp;
                ship.Direction = (short)rand.Next(2);

                bool validdeployment = true;
                if (ship.Direction == 0 && ship.StartPoint.X + ship.Length < tableWidth)
                {
                    for (int j = 0; j < ship.Length; j++)
                    {
                        if (computerTable[(int)ship.StartPoint.X + j, (int)ship.StartPoint.Y] != 0)
                            validdeployment = false;
                    }
                }
                else if (ship.Direction == 1 && ship.StartPoint.Y + ship.Length < tableHeight)
                {
                    for (int j = 0; j < ship.Length; j++)
                    {
                        if (computerTable[(int)ship.StartPoint.X, (int)ship.StartPoint.Y + j] != 0)
                            validdeployment = false;
                    }
                }
                else
                    validdeployment = false;

                if (validdeployment)
                {
                    for (int j = 0; j < ship.Length; j++)
                    {
                        if (ship.Direction == 0)
                            computerTable[(int)ship.StartPoint.X + j, (int)ship.StartPoint.Y] = 1;
                        else
                            computerTable[(int)ship.StartPoint.X, (int)ship.StartPoint.Y + j] = 1;
                    }
                }
                else
                    --i;

            }
        }
    }
}
