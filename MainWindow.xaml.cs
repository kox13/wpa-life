using System.Windows;

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

            DrawBoard();
        }

        private void DrawBoard()
        {
            BoardImage.Source =
                boardRenderer.Render(viewModel.Engine.Board);
        }
    }
}