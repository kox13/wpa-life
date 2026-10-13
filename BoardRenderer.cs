using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace PLA_1
{
    internal class BoardRenderer
    {
        private const int CELL_SIZE = 32;
        public CellShape CellShape { get; set; }

        public WriteableBitmap Render(Board board)
        {
            return CellShape switch
            {
                CellShape.SQUARE => RenderSquareBoard(board),
                CellShape.TRIANGLE => RenderTriangleBoard(board),
                CellShape.HEXAGON => RenderHexagonBoard(board),
                _ => throw new ArgumentOutOfRangeException()
            };
        }

        private WriteableBitmap RenderSquareBoard(Board board)
        {
            int width = board.Width * CELL_SIZE;
            int height = board.Height * CELL_SIZE;
            int stride = width * 4;

            byte[] pixels = new byte[height * stride];

            for (int y = 0; y < board.Height; y++)
            {
                for (int x = 0; x < board.Width; x++)
                {
                    Cell cell = board.GetCell(x, y);

                    int cellX = x * CELL_SIZE;
                    int cellY = y * CELL_SIZE;

                    if (cell.HasPlant && cell.Animal != null)
                    {
                        FillRectangle(pixels, cellX, cellY, stride, CELL_SIZE, CELL_SIZE, Colors.Green);
                        FillRectangle(pixels, cellX + CELL_SIZE / 2 - 3, cellY, stride, 7, CELL_SIZE, Colors.Red);
                    }
                    else if (cell.HasPlant)
                    {
                        FillRectangle(pixels, cellX, cellY, stride, CELL_SIZE, CELL_SIZE, Colors.Green);
                    }
                    else if (cell.Animal != null)
                    {
                        FillRectangle(pixels, cellX, cellY, stride, CELL_SIZE, CELL_SIZE, Colors.Red);
                    }
                    else
                    {
                        FillRectangle(pixels, cellX, cellY, stride, CELL_SIZE, CELL_SIZE, Colors.LightGray);
                    }

                    // Borders
                    int border = 2;

                    // Top
                    FillRectangle(pixels, cellX, cellY, stride,
                        CELL_SIZE, border, Colors.Black);

                    // Bottom
                    FillRectangle(pixels, cellX, cellY + CELL_SIZE - border, stride,
                        CELL_SIZE, border, Colors.Black);

                    // Left
                    FillRectangle(pixels, cellX, cellY, stride,
                        border, CELL_SIZE, Colors.Black);

                    // Right
                    FillRectangle(pixels, cellX + CELL_SIZE - border, cellY, stride,
                        border, CELL_SIZE, Colors.Black);


                }
            }

            var bitmap = new WriteableBitmap(width, height, 96, 96, PixelFormats.Bgra32, null);
            bitmap.WritePixels(new Int32Rect(0, 0, width, height), pixels, stride, 0);

            return bitmap;
        }

        private void FillRectangle(byte[] pixels, int x, int y, int stride, int width, int height, Color color)
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

        private WriteableBitmap RenderTriangleBoard(Board board)
        {
            double side = CELL_SIZE;
            double triangleHeight = Math.Sqrt(3) / 2 * side;

            int width = (int)Math.Ceiling((board.Width + 1) * side / 2);
            int height = (int)Math.Ceiling(board.Height * triangleHeight);

            int stride = width * 4;
            byte[] pixels = new byte[height * stride];

            for (int y = 0; y < board.Height; y++)
            {
                for (int x = 0; x < board.Width; x++)
                {
                    Cell cell = board.GetCell(x, y);

                    Color color = cell.HasPlant && cell.Animal != null
                        ? Colors.Purple
                        : cell.HasPlant
                            ? Colors.Green
                            : cell.Animal != null
                                ? Colors.Red
                                : Colors.LightGray;

                    double x0 = x * side / 2.0;
                    double x1 = x0 + side / 2.0;
                    double x2 = x0 + side;
                    double y0 = y * triangleHeight;
                    double y1 = y0 + triangleHeight;

                    bool pointsUp = (x + y) % 2 == 0;

                    Point[] points = pointsUp
                        ? new[]
                        {
                            new Point(x0, y1),
                            new Point(x1, y0),
                            new Point(x2, y1)
                        }
                        : new[]
                        {
                            new Point(x0, y0),
                            new Point(x2, y0),
                            new Point(x1, y1)
                        };

                    FillTriangle(pixels, stride, width, height, points, color);
                    DrawLine(pixels, stride, width, height,
                            (int)points[0].X, (int)points[0].Y,
                            (int)points[1].X, (int)points[1].Y,
                            Colors.Black);
                }
            }

            var bitmap = new WriteableBitmap(
                width, height, 96, 96, PixelFormats.Bgra32, null);

            bitmap.WritePixels(
                new Int32Rect(0, 0, width, height), pixels, stride, 0);

            return bitmap;
        }

        private void FillTriangle(
            byte[] pixels,
            int stride,
            int bitmapWidth,
            int bitmapHeight,
            Point[] points,
            Color color)
        {
            int minX = Math.Max(0, (int)Math.Floor(points.Min(p => p.X)));
            int maxX = Math.Min(bitmapWidth - 1, (int)Math.Ceiling(points.Max(p => p.X)));
            int minY = Math.Max(0, (int)Math.Floor(points.Min(p => p.Y)));
            int maxY = Math.Min(bitmapHeight - 1, (int)Math.Ceiling(points.Max(p => p.Y)));

            double Edge(Point a, Point b, double x, double y)
            {
                return (x - a.X) * (b.Y - a.Y) -
                       (y - a.Y) * (b.X - a.X);
            }

            double DistanceToLine(Point a, Point b, double x, double y)
            {
                double dx = b.X - a.X;
                double dy = b.Y - a.Y;

                return Math.Abs(
                    dy * x - dx * y + b.X * a.Y - b.Y * a.X)
                    / Math.Sqrt(dx * dx + dy * dy);
            }

            for (int y = minY; y <= maxY; y++)
            {
                for (int x = minX; x <= maxX; x++)
                {
                    double px = x + 0.5;
                    double py = y + 0.5;

                    double e0 = Edge(points[0], points[1], px, py);
                    double e1 = Edge(points[1], points[2], px, py);
                    double e2 = Edge(points[2], points[0], px, py);

                    bool hasNegative = e0 < 0 || e1 < 0 || e2 < 0;
                    bool hasPositive = e0 > 0 || e1 > 0 || e2 > 0;

                    if (hasNegative && hasPositive)
                        continue;

                    bool isBorder =
                        DistanceToLine(points[0], points[1], px, py) < 0.7 ||
                        DistanceToLine(points[1], points[2], px, py) < 0.7 ||
                        DistanceToLine(points[2], points[0], px, py) < 0.7;

                    Color pixelColor = isBorder ? Colors.Black : color;

                    int i = y * stride + x * 4;

                    pixels[i] = pixelColor.B;
                    pixels[i + 1] = pixelColor.G;
                    pixels[i + 2] = pixelColor.R;
                    pixels[i + 3] = pixelColor.A;
                }
            }
        }

        // Bresenham
        private void DrawLine(
            byte[] pixels,
            int stride,
            int width,
            int height,
            int x0,
            int y0,
            int x1,
            int y1,
            Color color)
        {
            int dx = Math.Abs(x1 - x0);
            int dy = Math.Abs(y1 - y0);

            int sx = x0 < x1 ? 1 : -1;
            int sy = y0 < y1 ? 1 : -1;
            int error = dx - dy;

            while (true)
            {
                if (x0 >= 0 && x0 < width && y0 >= 0 && y0 < height)
                {
                    int i = y0 * stride + x0 * 4;

                    pixels[i] = color.B;
                    pixels[i + 1] = color.G;
                    pixels[i + 2] = color.R;
                    pixels[i + 3] = color.A;
                }

                if (x0 == x1 && y0 == y1)
                    break;

                int e2 = 2 * error;

                if (e2 > -dy)
                {
                    error -= dy;
                    x0 += sx;
                }

                if (e2 < dx)
                {
                    error += dx;
                    y0 += sy;
                }
            }
        }

        private WriteableBitmap RenderHexagonBoard(Board board)
        {
            double side = CELL_SIZE;
            double hexWidth = Math.Sqrt(3) * side;
            double rowHeight = 1.5 * side;

            int width = (int)Math.Ceiling((board.Width + 0.5) * hexWidth);
            int height = (int)Math.Ceiling(
                2 * side + (board.Height - 1) * rowHeight);

            var visual = new DrawingVisual();

            using (DrawingContext dc = visual.RenderOpen())
            {
                for (int y = 0; y < board.Height; y++)
                {
                    for (int x = 0; x < board.Width; x++)
                    {
                        Cell cell = board.GetCell(x, y);

                        Brush fill = GetCellBrush(cell);

                        // Co drugi rząd przesuwamy o połowę szerokości.
                        double centerX =
                            hexWidth / 2 + x * hexWidth
                            + (y % 2) * hexWidth / 2;

                        double centerY = side + y * rowHeight;

                        Point[] points =
                        {
                    new Point(centerX, centerY - side),
                    new Point(centerX + hexWidth / 2, centerY - side / 2),
                    new Point(centerX + hexWidth / 2, centerY + side / 2),
                    new Point(centerX, centerY + side),
                    new Point(centerX - hexWidth / 2, centerY + side / 2),
                    new Point(centerX - hexWidth / 2, centerY - side / 2)
                };

                        var geometry = new StreamGeometry();

                        using (StreamGeometryContext ctx = geometry.Open())
                        {
                            ctx.BeginFigure(points[0], true, true);
                            ctx.PolyLineTo(points.Skip(1).ToArray(), true, true);
                        }

                        geometry.Freeze();

                        dc.DrawGeometry(
                            fill,
                            new Pen(Brushes.Black, 2),
                            geometry);
                    }
                }
            }

            var rendered = new RenderTargetBitmap(
                width, height, 96, 96, PixelFormats.Pbgra32);

            rendered.Render(visual);

            var bitmap = new WriteableBitmap(
                width, height, 96, 96, PixelFormats.Bgra32, null);

            int stride = width * 4;
            byte[] pixels = new byte[height * stride];

            rendered.CopyPixels(pixels, stride, 0);

            bitmap.WritePixels(
                new Int32Rect(0, 0, width, height),
                pixels,
                stride,
                0);

            return bitmap;
        }

        private Brush GetCellBrush(Cell cell)
        {
            if (cell.HasPlant && cell.Animal != null)
                return Brushes.Gold;

            if (cell.HasPlant)
                return Brushes.Green;

            if (cell.Animal != null)
                return Brushes.Red;

            return Brushes.LightGray;
        }
    }
}
