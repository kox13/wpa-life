namespace PLA_1
{
    internal class Board
    {
        public int Width { get; private set; }
        public int Height { get; private set; }
        private Cell[,] cells;

        public Board(int width, int height)
        {
            this.Width = width;
            this.Height = height;
            this.cells = new Cell[width, height];

            RandomizeCells(); // TODO: accept entity count
        }

        public Cell GetCell(int x, int y)
        {
            return cells[x, y];
        }

        public void SetCell(int x, int y, Cell cell) => cells[x, y] = cell;

        private void RandomizeCells()
        {
            Random rnd = new Random();

            for (int y = 0; y < this.Height; y++)
            {
                for (int x = 0; x < this.Width; x++)
                {
                    Cell cell = new Cell();
                    int z = rnd.Next(1, 4);

                    cell.HasPlant = z == 2;
                    if (cell.HasPlant && rnd.Next(0, 2) == 1)
                    {
                        cell.Animal = new Animal();
                    }
                    else if (z == 3)
                    {
                        cell.Animal = new Animal();
                    }

                    cells[y, x] = cell;
                }
            }
        }
    }
}
