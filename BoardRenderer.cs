using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace PLA_1
{
    internal class BoardRenderer
    {
        private const int CellSize = 8;

        public WriteableBitmap Render(Board board)
        {
            int width = board.Width * CellSize;
            int height = board.Height * CellSize;
            int stride = width * 4;

            byte[] pixels = new byte[height * stride];

            for (int y = 0; y < board.Height; y++)
            {
                for (int x = 0; x < board.Width; x++)
                {
                    Cell cell = board.GetCell(x, y);

                    bool hasPlant = cell.HasPlant;
                    bool hasAnimal = cell.Animal is not null;

                    if (hasPlant && hasAnimal)
                    {
                        // Zielone boki, czerwony środek.
                        FillCell(pixels, stride, x, y, Colors.Green);

                        FillRect(pixels, stride,
                            x * CellSize + CellSize / 3,
                            y * CellSize,
                            CellSize / 3,
                            CellSize,
                            Colors.Crimson);
                    }
                    else if (hasPlant)
                    {
                        FillCell(pixels, stride, x, y, Colors.Green);
                    }
                    else if (hasAnimal)
                    {
                        FillCell(pixels, stride, x, y, Colors.Crimson);
                    }
                    else
                    {
                        FillCell(pixels, stride, x, y, Colors.White);
                    }

                    Color borderColor = Colors.Gray;

                    // Górna i dolna krawędź
                    FillRect(pixels, stride,
                        x * CellSize, y * CellSize,
                        CellSize, 1, borderColor);

                    FillRect(pixels, stride,
                        x * CellSize, (y + 1) * CellSize - 1,
                        CellSize, 1, borderColor);

                    // Lewa i prawa krawędź
                    FillRect(pixels, stride,
                        x * CellSize, y * CellSize,
                        1, CellSize, borderColor);

                    FillRect(pixels, stride,
                        (x + 1) * CellSize - 1, y * CellSize,
                        1, CellSize, borderColor);
                }
            }

            var bitmap = new WriteableBitmap(
                width, height, 96, 96,
                PixelFormats.Bgra32, null);

            bitmap.WritePixels(
                new Int32Rect(0, 0, width, height),
                pixels, stride, 0);

            return bitmap;
        }

        private static void FillCell(
            byte[] pixels, int stride,
            int x, int y, Color color)
        {
            FillRect(pixels, stride,
                x * CellSize, y * CellSize,
                CellSize, CellSize, color);
        }

        private static void FillRect(
            byte[] pixels, int stride,
            int x, int y, int width, int height,
            Color color)
        {
            for (int py = y; py < y + height; py++)
            {
                for (int px = x; px < x + width; px++)
                {
                    int i = py * stride + px * 4;

                    pixels[i] = color.B;
                    pixels[i + 1] = color.G;
                    pixels[i + 2] = color.R;
                    pixels[i + 3] = color.A;
                }
            }
        }
    }
}
