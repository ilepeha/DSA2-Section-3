using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Media;
using Avalonia.Controls.Shapes;
using System.Collections.Generic;

namespace SpillBucketAppGUI
{
    public partial class MainWindow : Window
    {
        private int[,] _bitmap = new int[10, 10];
        private const int CellSize = 20;

        public MainWindow()
        {
            InitializeComponent();
            InitializeBitmap();
            DrawBitmap();
        }

        private void InitializeBitmap()
        {
            for (int i = 0; i < _bitmap.GetLength(0); i++)
            {
                for (int j = 0; j < _bitmap.GetLength(1); j++)
                {
                    _bitmap[i, j] = (i + j) % 2; // Alternating pattern
                }
            }
        }

        private void DrawBitmap()
        {
            BitmapCanvas.Children.Clear();
            for (int i = 0; i < _bitmap.GetLength(0); i++)
            {
                for (int j = 0; j < _bitmap.GetLength(1); j++)
                {
                    var rect = new Rectangle
                    {
                        Width = CellSize,
                        Height = CellSize,
                        Fill = GetColorBrush(_bitmap[i, j]),
                        Stroke = Brushes.Black,
                        StrokeThickness = 1
                    };

                    Canvas.SetLeft(rect, j * CellSize);
                    Canvas.SetTop(rect, i * CellSize);
                    BitmapCanvas.Children.Add(rect);
                }
            }
        }

        private IBrush GetColorBrush(int color)
        {
            return color switch
            {
                1 => Brushes.Red,
                2 => Brushes.Blue,
                3 => Brushes.Green,
                _ => Brushes.White,
            };
        }

        private void OnFloodFillClicked(object sender, Avalonia.Interactivity.RoutedEventArgs e)
        {
            int selectedColor = ColorSelector.SelectedIndex + 1;

            // Select the algorithm based on the dropdown
            if (AlgorithmSelector.SelectedIndex == 0) // Iterative
            {
                FloodFillIterative(0, 0, _bitmap[0, 0], selectedColor);
            }
            else if (AlgorithmSelector.SelectedIndex == 1) // Recursive
            {
                FloodFillRecursive(0, 0, _bitmap[0, 0], selectedColor);
            }

            DrawBitmap();
        }

        private void FloodFillIterative(int x, int y, int targetColor, int replacementColor)
        {
            if (targetColor == replacementColor) return;

            var queue = new Queue<(int, int)>();
            queue.Enqueue((x, y));

            while (queue.Count > 0)
            {
                var (currX, currY) = queue.Dequeue();

                if (currX < 0 || currX >= _bitmap.GetLength(0) || currY < 0 || currY >= _bitmap.GetLength(1))
                    continue;
                if (_bitmap[currX, currY] != targetColor || _bitmap[currX, currY] == replacementColor)
                    continue;

                _bitmap[currX, currY] = replacementColor;

                queue.Enqueue((currX + 1, currY));
                queue.Enqueue((currX - 1, currY));
                queue.Enqueue((currX, currY + 1));
                queue.Enqueue((currX, currY - 1));
            }
        }

        private void FloodFillRecursive(int x, int y, int targetColor, int replacementColor)
        {
            if (x < 0 || x >= _bitmap.GetLength(0) || y < 0 || y >= _bitmap.GetLength(1)) return;
            if (_bitmap[x, y] != targetColor || _bitmap[x, y] == replacementColor) return;

            _bitmap[x, y] = replacementColor;

            FloodFillRecursive(x + 1, y, targetColor, replacementColor);
            FloodFillRecursive(x - 1, y, targetColor, replacementColor);
            FloodFillRecursive(x, y + 1, targetColor, replacementColor);
            FloodFillRecursive(x, y - 1, targetColor, replacementColor);
        }

        private void OnResizeBitmapClicked(object sender, Avalonia.Interactivity.RoutedEventArgs e)
        {
            if (int.TryParse(BitmapWidthInput.Text, out int width) && int.TryParse(BitmapHeightInput.Text, out int height))
            {
                _bitmap = new int[height, width];
                InitializeBitmap();
                DrawBitmap();
            }
        }

        private void OnBitmapCanvasClicked(object sender, PointerPressedEventArgs e)
        {
            var point = e.GetPosition(BitmapCanvas);
            int cellX = (int)(point.X / CellSize);
            int cellY = (int)(point.Y / CellSize);

            if (cellX >= 0 && cellX < _bitmap.GetLength(1) && cellY >= 0 && cellY < _bitmap.GetLength(0))
            {
                int selectedColor = ColorSelector.SelectedIndex + 1;

                if (AlgorithmSelector.SelectedIndex == 0) // Iterative
                {
                    FloodFillIterative(cellY, cellX, _bitmap[cellY, cellX], selectedColor);
                }
                else if (AlgorithmSelector.SelectedIndex == 1) // Recursive
                {
                    FloodFillRecursive(cellY, cellX, _bitmap[cellY, cellX], selectedColor);
                }

                DrawBitmap();
            }
        }
    }
}
