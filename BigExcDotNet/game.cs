using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BigExcDotNet
{
    public class game
    {
        public class cell
        {
            public int value;
            public bool marked;
            public bool opened;
            public cell()
            {
                value = 0; // -1 if bomb
                marked = false;
                opened = false;
            }
        }
        public int colls, rows, mines, openedCells = 0, markedCells = 0, totalCells;
        public bool firstTurn;
        public cell[][] mat;

        public game()
        {
            firstTurn = true;
            colls = 9;
            rows = 9;
            mines = 10;
            totalCells = colls * rows;
            generateEmpty();
            placeMines();
        }
        public game(int Ccolls, int Crows, int Cmines)
        {
            firstTurn = true;
            colls = Ccolls;
            rows = Crows;
            mines = Cmines;
            totalCells = colls * rows;
            generateEmpty();
            placeMines();
        }
        void generateEmpty()
        {
            mat = new cell[rows][];
            for (int i = 0; i < rows; i++)
            {
                mat[i] = new cell[colls];
                for (int j = 0; j < colls; j++)
                    mat[i][j] = new cell();
            }
        }
        void setNeighbors(int y, int x)
        {
            for (int i = -1; i < 2; i++)
                for (int j = -1; j < 2; j++)
                    if ((y + i >= 0) && (x + j >= 0) && (y + i < rows) && (x + j < colls))
                        mat[y + i][x + j].value = (mat[y + i][x + j].value == -1) ? -1 : (mat[y + i][x + j].value + 1);
        }
        void placeMines()
        {
            var rand = new Random();
            for (int i = 0; i < mines; i++)
            {
                int x = rand.Next(colls);
                int y = rand.Next(rows);
                if (mat[y][x].value != -1)
                {
                    mat[y][x].value = -1;
                    setNeighbors(y, x);
                }
                else
                    i--;
            }
        }
        public void openCell(int i, int j)
        {
            while ((firstTurn) && (mat[i][j].value == -1)) // if first turn was unlucky - reset
            {
                for (int x = 0; x < colls; x++)
                    for (int y = 0; y < rows; y++)
                    {
                        mat[y][x].value = 0;
                        mat[y][x].marked = false;
                        mat[y][x].opened = false;
                    }
                placeMines();
            }
            firstTurn = false;
            mat[i][j].opened = true;
            openedCells++;
        }
        public bool markCell(int i, int j) // return true if cell wasnt marked
        {
            if (mat[i][j].marked)
            {
                mat[i][j].marked = false;
                markedCells--;
                return false;
            }
            else
            {
                mat[i][j].marked = true;
                markedCells++;
                return true;
            }
        }
        public bool checkWin()
        {
            if ((markedCells == mines) && (markedCells + openedCells == totalCells))
            {
                return true;
            }
            else return false;
        }
    }
}
