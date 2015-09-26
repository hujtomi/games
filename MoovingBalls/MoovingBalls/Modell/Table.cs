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
using RouteEngine;

namespace MoovingBalls.Modell
{
    public class Table
    {
        private int score = 0;

        public int Score
        {
            get { return score; }
            set { score = value; }
        }

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

        short[,] gametable;

        public short[,] GameTable
        {
            get { return gametable; }
            set { gametable = value; }
        }

        public bool IsBallSelected { get; set; }
        public Point SelectedPoint { get; set; }

        Location[,] locations;
        RouteEngine.RouteEngine engine;

        public Table()
        {
            IsBallSelected = false;
            SelectedPoint = new Point(-1, -1);

            gametable = new short[tableWidth, tableHeight];

            for (int i = 0; i < 3; )
            {
                Random rand = new Random();

                int x = rand.Next(tableWidth);
                int y = rand.Next(tableHeight);

                int value = rand.Next(7);
                ++value;

                if (gametable[x, y] == 0)
                {
                    gametable[x, y] = (short)value;
                    ++i;
                }
            }

            locations = new Location[tableWidth, tableHeight];
            engine = new RouteEngine.RouteEngine();

            for (int i = 0; i < tableWidth; i++)
            {
                for (int j = 0; j < tableHeight; j++)
                {
                    locations[i, j] = new Location() { Identifier = i + "," + j };
                    engine.Locations.Add(locations[i, j]);
                }
            }
        }


        public int[,] addNewBalls()
        {
            Random rand = new Random();

            int freeSpaceCoutnter = 0;

            for (int i = 0; i < tableWidth; i++)
            {
                for (int j = 0; j < tableHeight; j++)
                {
                    if (gametable[i, j] == 0)
                        ++freeSpaceCoutnter;
                }
            }

            int[,] newplaces = new int[3, 3];

            if (freeSpaceCoutnter > 3)
            {
                for (int t = 0; t < 3; t++)
                {
                    int place = rand.Next(freeSpaceCoutnter + 1 - t);
                    int fsp = 0;

                    for (int i = 0; i < tableWidth; i++)
                    {
                        for (int j = 0; j < tableHeight; j++)
                        {
                            if (gametable[i, j] == 0)
                            {
                                ++fsp;
                                if (fsp == place)
                                {
                                    int value = rand.Next(7);
                                    ++value;
                                    gametable[i, j] = (short)value;
                                    newplaces[t, 0] = i;
                                    newplaces[t, 1] = j;
                                    newplaces[t, 2] = value;
                                }
                            }
                        }
                    }
                }

                return newplaces;
            }
            else
                return null;
        }

        public List<Point> checkMatching()
        {
            short prev = 0;
            int counter = 0;

            List<Point> matchingPoints = new List<Point>();

            for (int i = 0; i < tableWidth; i++)
            {
                for (int j = 0; j < tableHeight; j++)
                {
                    if (j == tableWidth - 1 || prev != gametable[i, j])
                    {
                        if (j == tableWidth - 1 && prev == gametable[i, j])
                            counter++;

                        if (prev != 0)
                        {
                            if (counter > 4)
                            {
                                for (int t = counter; t > 0; t--)
                                {
                                    if (j == tableWidth - 1 && prev == gametable[i, j])
                                        matchingPoints.Add(new Point(i, j - t + 1));
                                    else
                                        matchingPoints.Add(new Point(i, j - t));
                                }
                            }

                            if (counter == 5)
                                score += 5;
                            else if (counter == 6)
                                score += 7;
                            else if (counter == 7)
                                score += 10;
                            else if (counter == 8)
                                score += 14;
                            else if (counter == 9)
                                score += 19;
                        }

                        prev = gametable[i, j];
                        counter = 0;
                        ++counter;
                    }
                    else
                    {
                        ++counter;
                    }
                }

                counter = 0;
                prev = 0;
            }


            for (int j = 0; j < tableHeight; j++)
            {
                for (int i = 0; i < tableWidth; i++)
                {
                    if (i == tableWidth - 1 || prev != gametable[i, j])
                    {
                        if (i == tableWidth - 1 && prev == gametable[i, j])
                            counter++;

                        if (prev != 0)
                        {
                            if (counter > 4)
                            {
                                for (int t = counter; t > 0; t--)
                                {
                                    if (i == tableWidth - 1 && prev == gametable[i, j])
                                        matchingPoints.Add(new Point(i - t + 1, j));
                                    else
                                        matchingPoints.Add(new Point(i - t, j));
                                }
                            }

                            if (counter == 5)
                                score += 5;
                            else if (counter == 6)
                                score += 7;
                            else if (counter == 7)
                                score += 10;
                            else if (counter == 8)
                                score += 14;
                            else if (counter == 9)
                                score += 19;
                        }

                        prev = gametable[i, j];
                        counter = 0;
                        ++counter;
                    }
                    else
                    {
                        ++counter;
                    }
                }

                counter = 0;
                prev = 0;
            }

            return matchingPoints;
        }

        internal Route calculateRoute(Point from, Point to)
        {
            engine.Connections.Clear();

            for (int i = 0; i < tableWidth; i++)
            {
                for (int j = 0; j < tableHeight; j++)
                {
                    if (i > 0 && gametable[i - 1, j] == 0)
                    {
                        engine.Connections.Add(new Connection(locations[i, j], locations[i - 1, j], 1));
                    }
                    if (i + 1 < tableWidth && gametable[i + 1, j] == 0)
                    {
                        engine.Connections.Add(new Connection(locations[i, j], locations[i + 1, j], 1));
                    }
                    if (j > 0 && gametable[i, j - 1] == 0)
                    {
                        engine.Connections.Add(new Connection(locations[i, j], locations[i, j - 1], 1));
                    }
                    if (j + 1 < tableHeight && gametable[i, j + 1] == 0)
                    {
                        engine.Connections.Add(new Connection(locations[i, j], locations[i, j + 1], 1));
                    }
                }
            }

            Dictionary<Location, Route> mincosts = engine.CalculateMinCost(locations[(int)from.X, (int)from.Y]);

            return mincosts[locations[(int)to.X, (int)to.Y]];

        }
    }
}
