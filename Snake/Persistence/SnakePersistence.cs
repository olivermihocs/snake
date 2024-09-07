using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;

namespace Snake.Persistence
{
    public struct Position
    {
        public int i, j;

        public Position(int p1, int p2)
        {
            i = p1;
            j = p2;
        }
    }

    public class SnakePersistence
    {

        //Input
        public int ReadData(string filePath, ref List<string> input, ref int row, ref int col, ref int counter, ref List<Position> player)
        {
            List<string> lines = File.ReadAllLines(filePath).ToList();
            counter = Int32.Parse(lines[0].Split(' ')[0]);
            int direction = Int32.Parse(lines[0].Split(' ')[1]);

            int playerLength = Int32.Parse(lines[1]);

            for (int i = 2; i < 2 + playerLength; ++i)
            {
                player.Add(new Position(Int32.Parse(lines[i].Split(' ')[0]), Int32.Parse(lines[i].Split(' ')[1])));
            }

            lines.RemoveAll(t => t.Contains(" "));
            lines.RemoveAt(0);
            row = lines.Count;
            col = lines[2].Length;
            input = lines;

            return direction;
        }

        //Output
        public void WriteData(string filePath, List<string> output, int counter, int direction, List<Position> player)
        {
            List<string> playerString = new List<string>();
            foreach (Position pos in player)
                playerString.Add(pos.i.ToString() + " " + pos.j.ToString());
            playerString.Insert(0, player.Count.ToString());
            playerString.Insert(0, counter.ToString() + " " + direction.ToString());
            File.WriteAllLines(filePath, playerString.Concat(output).ToList());
        }
    }
}
