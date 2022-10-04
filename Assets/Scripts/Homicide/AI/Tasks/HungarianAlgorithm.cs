using System;

namespace Homicide.AI.Tasks
{
	public static class HungarianAlgorithm
	{
        public static int[] FindAssignments(this int[,] costs)
        {
            if (costs == null)
            {
                throw new ArgumentNullException("costs null");
            }

            var h = costs.GetLength(0);
            var w = costs.GetLength(1);

            for (int i = 0; i < h; i++)
            {
                var min = int.MaxValue;
                for (int j = 0; j < w; j++)
                {
                    min = Math.Min(min, costs[i, j]);
                }
                for (int j = 0; j < w; j++)
                {
                    costs[i, j] -= min;
                }
            }

            var masks = new byte[h, w];
            var rowsCovered = new bool[h];
            var colsCovered = new bool[w];
            for (int i = 0; i < h; i++)
            {
                for (int j = 0; j < w; j++)
                {
                    if (costs[i, j] == 0 && !rowsCovered[i] && !colsCovered[j])
                    {
                        masks[i, j] = 1;
                        rowsCovered[i] = true;
                        colsCovered[j] = true;
                    }
                }
            }
            ClearCovers(rowsCovered, colsCovered, w, h);

            var path = new Location[w * h];
            Location pathStart = default(Location);
            var step = 1;
            while (step != -1)
            {
                switch (step)
                {
                    case 1:
                        step = RunStep1(costs, masks, rowsCovered, colsCovered, w, h);
                        break;
                    case 2:
                        step = RunStep2(costs, masks, rowsCovered, colsCovered, w, h, ref pathStart);
                        break;
                    case 3:
                        step = RunStep3(costs, masks, rowsCovered, colsCovered, w, h, path, pathStart);
                        break;
                    case 4:
                        step = RunStep4(costs, masks, rowsCovered, colsCovered, w, h);
                        break;
                }
            }

            var agentsTasks = new int[h];
            for (int i = 0; i < h; i++)
            {
                for (int j = 0; j < w; j++)
                {
                    if (masks[i, j] == 1)
                    {
                        agentsTasks[i] = j;
                        break;
                    }
                }
            }
            return agentsTasks;
        }

        private static int RunStep1(int[,] costs, byte[,] masks, bool[] rowsCovered, bool[] colsCovered, int w, int h)
        {
            for (int i = 0; i < h; i++)
            {
                for (int j = 0; j < w; j++)
                {
                    if (masks[i, j] == 1)
                    {
                        colsCovered[j] = true;
                    }
                }
            }
            var colsCoveredCount = 0;
            for (int j = 0; j < w; j++)
            {
                if (colsCovered[j])
                {
                    colsCoveredCount++;
                }
            }
            if (colsCoveredCount == h)
            {
                return -1;
            }
            else
            {
                return 2;
            }
        }

        private static int RunStep2(int[,] costs, byte[,] masks, bool[] rowsCovered, bool[] colsCovered, int w, int h,
            ref Location pathStart)
        {
            Location loc;
            while (true)
            {
                loc = FindZero(costs, masks, rowsCovered, colsCovered, w, h);
                if (loc.row == -1)
                {
                    return 4;
                }
                else
                {
                    masks[loc.row, loc.column] = 2;
                    var starCol = FindStarInRow(masks, w, loc.row);
                    if (starCol != -1)
                    {
                        rowsCovered[loc.row] = true;
                        colsCovered[starCol] = false;
                    }
                    else
                    {
                        pathStart = loc;
                        return 3;
                    }
                }
            }
        }

        private static int RunStep3(int[,] costs, byte[,] masks, bool[] rowsCovered, bool[] colsCovered, int w, int h,
            Location[] path, Location pathStart)
        {
            var pathIndex = 0;
            path[0] = pathStart;
            while (true)
            {
                var row = FindStarInColumn(masks, h, path[pathIndex].column);
                if (row == -1)
                {
                    break;
                }
                pathIndex++;
                path[pathIndex] = new(row, path[pathIndex - 1].column);
                var col = FindPrimeInRow(masks, w, path[pathIndex].row);
                pathIndex++;
                path[pathIndex] = new(path[pathIndex - 1].row, col);
            }
            ConvertPath(masks, path, pathIndex + 1);
            ClearCovers(rowsCovered, colsCovered, w, h);
            ClearPrimes(masks, w, h);
            return 1;
        }

        private static int RunStep4(int[,] costs, byte[,] masks, bool[] rowsCovered, bool[] colsCovered, int w, int h)
        {
            var minValue = FindMinimum(costs, rowsCovered, colsCovered, w, h);
            for (int i = 0; i < h; i++)
            {
                for (int j = 0; j < w; j++)
                {
                    if (rowsCovered[i])
                    {
                        costs[i, j] += minValue;
                    }
                    if (!colsCovered[j])
                    {
                        costs[i, j] -= minValue;
                    }
                }
            }
            return 2;
        }

        private static void ConvertPath(byte[,] masks, Location[] path, int pathLength)
        {
            for (int i = 0; i < pathLength; i++)
            {
                if (masks[path[i].row, path[i].column] == 1)
                {
                    masks[path[i].row, path[i].column] = 0;
                }
                else if (masks[path[i].row, path[i].column] == 2)
                {
                    masks[path[i].row, path[i].column] = 1;
                }
            }
        }

        private static Location FindZero(int[,] costs, byte[,] masks, bool[] rowsCovered, bool[] colsCovered,
            int w, int h)
        {
            for (int i = 0; i < h; i++)
            {
                for (int j = 0; j < w; j++)
                {
                    if (costs[i, j] == 0 && !rowsCovered[i] && !colsCovered[j])
                    {
                        return new(i, j);
                    }
                }
            }
            return new(-1, -1);
        }

        private static int FindMinimum(int[,] costs, bool[] rowsCovered, bool[] colsCovered, int w, int h)
        {
            var minValue = int.MaxValue;
            for (int i = 0; i < h; i++)
            {
                for (int j = 0; j < w; j++)
                {
                    if (!rowsCovered[i] && !colsCovered[j])
                    {
                        minValue = Math.Min(minValue, costs[i, j]);
                    }
                }
            }
            return minValue;
        }

        private static int FindStarInRow(byte[,] masks, int w, int row)
        {
            for (int j = 0; j < w; j++)
            {
                if (masks[row, j] == 1)
                {
                    return j;
                }
            }
            return -1;
        }

        private static int FindStarInColumn(byte[,] masks, int h, int col)
        {
            for (int i = 0; i < h; i++)
            {
                if (masks[i, col] == 1)
                {
                    return i;
                }
            }
            return -1;
        }

        private static int FindPrimeInRow(byte[,] masks, int w, int row)
        {
            for (int j = 0; j < w; j++)
            {
                if (masks[row, j] == 2)
                {
                    return j;
                }
            }
            return -1;
        }

        private static void ClearCovers(bool[] rowsCovered, bool[] colsCovered, int w, int h)
        {
            for (int i = 0; i < h; i++)
            {
                rowsCovered[i] = false;
            }
            for (int j = 0; j < w; j++)
            {
                colsCovered[j] = false;
            }
        }

        private static void ClearPrimes(byte[,] masks, int w, int h)
        {
            for (int i = 0; i < h; i++)
            {
                for (int j = 0; j < w; j++)
                {
                    if (masks[i, j] == 2)
                    {
                        masks[i, j] = 0;
                    }
                }
            }
        }

        private struct Location
        {
            public int row;
            public int column;

            public Location(int row, int col)
            {
                this.row = row;
                this.column = col;
            }
        }
    }
}
