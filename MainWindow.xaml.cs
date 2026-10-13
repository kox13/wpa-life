using System.Windows;
using System.Windows.Input;

namespace PLA_1
{
    public partial class MainWindow : Window
    {
        private ViewModel viewModel;
        private BoardRenderer boardRenderer;

        public MainWindow()
        {
            InitializeComponent();

            viewModel = new ViewModel();
            boardRenderer = new BoardRenderer();
            boardRenderer.CellShape = CellShape.HEXAGON;

            DrawBoard();
        }

        private void DrawBoard()
        {
            BoardImage.Source =
                boardRenderer.Render(viewModel.Engine.Board);
        }

        private const double ZoomStep = 1.1;
        private const double MinZoom = 0.2;
        private const double MaxZoom = 8.0;

        private void BoardImage_MouseWheel(object sender, MouseWheelEventArgs e)
        {
            double oldZoom = BoardScale.ScaleX;

            double factor = e.Delta > 0 ? ZoomStep : 1.0 / ZoomStep;
            double newZoom = Math.Clamp(oldZoom * factor, MinZoom, MaxZoom);

            if (Math.Abs(newZoom - oldZoom) < 0.0001)
                return;

            // Pozycja kursora względem Image.
            Point mousePosition = e.GetPosition(BoardImage);

            // Punkt obrazu pod kursorem przed zmianą skali.
            double imageX = mousePosition.X / oldZoom;
            double imageY = mousePosition.Y / oldZoom;

            BoardScale.ScaleX = newZoom;
            BoardScale.ScaleY = newZoom;

            // Utrzymujemy punkt pod kursorem w tym samym miejscu.
            BoardImage.RenderTransformOrigin = new Point(0, 0);

            e.Handled = true;
        }

        private void BoardScrollViewer_PreviewMouseWheel(
            object sender, MouseWheelEventArgs e)
        {
            double oldZoom = BoardScale.ScaleX;
            double factor = e.Delta > 0 ? ZoomStep : 1.0 / ZoomStep;
            double newZoom = Math.Clamp(oldZoom * factor, MinZoom, MaxZoom);

            if (Math.Abs(newZoom - oldZoom) < 0.0001)
            {
                e.Handled = true;
                return;
            }

            // Pozycja kursora w widoku.
            Point mouse = e.GetPosition(BoardScrollViewer);

            // Współrzędne punktu planszy pod kursorem.
            double contentX = (mouse.X + BoardScrollViewer.HorizontalOffset) / oldZoom;
            double contentY = (mouse.Y + BoardScrollViewer.VerticalOffset) / oldZoom;

            BoardScale.ScaleX = newZoom;
            BoardScale.ScaleY = newZoom;

            // Przesuń widok tak, aby ten sam punkt pozostał pod kursorem.
            BoardScrollViewer.UpdateLayout();
            BoardScrollViewer.ScrollToHorizontalOffset(
                contentX * newZoom - mouse.X);
            BoardScrollViewer.ScrollToVerticalOffset(
                contentY * newZoom - mouse.Y);

            e.Handled = true;
        }


        private bool _isDragging;
        private Point _lastMousePosition;

        private void BoardScrollViewer_MouseLeftButtonDown(
            object sender, MouseButtonEventArgs e)
        {
            _isDragging = true;
            _lastMousePosition = e.GetPosition(BoardScrollViewer);

            BoardScrollViewer.CaptureMouse();
            BoardScrollViewer.Cursor = Cursors.Hand;

            e.Handled = true;
        }

        private void BoardScrollViewer_MouseMove(
            object sender, MouseEventArgs e)
        {
            if (!_isDragging)
                return;

            Point currentPosition = e.GetPosition(BoardScrollViewer);

            Vector delta = currentPosition - _lastMousePosition;

            BoardScrollViewer.ScrollToHorizontalOffset(
                BoardScrollViewer.HorizontalOffset - delta.X);

            BoardScrollViewer.ScrollToVerticalOffset(
                BoardScrollViewer.VerticalOffset - delta.Y);

            _lastMousePosition = currentPosition;
        }

        private void BoardScrollViewer_MouseLeftButtonUp(
            object sender, MouseButtonEventArgs e)
        {
            StopDragging();
            e.Handled = true;
        }

        private void BoardScrollViewer_LostMouseCapture(
            object sender, MouseEventArgs e)
        {
            StopDragging();
        }

        private void StopDragging()
        {
            _isDragging = false;
            BoardScrollViewer.Cursor = Cursors.Arrow;

            if (Mouse.Captured == BoardScrollViewer)
                BoardScrollViewer.ReleaseMouseCapture();
        }

        private void BoardScrollViewer_SizeChanged(
            object sender, SizeChangedEventArgs e)
        {
            BoardContainer.MinWidth = BoardScrollViewer.ViewportWidth;
            BoardContainer.MinHeight = BoardScrollViewer.ViewportHeight;
        }
    }
}