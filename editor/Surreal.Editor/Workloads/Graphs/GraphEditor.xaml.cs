using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Shapes;

namespace Surreal.Editor.Workloads.Graphs
{
  public partial class GraphEditor
  {
    private bool  isDragging;
    private Point startPoint;

    public GraphEditor()
    {
      InitializeComponent();
    }

    private void OnAddRectangleClicked(object sender, RoutedEventArgs e)
    {
      var rectangle = new Rectangle();

      rectangle.Width  = 100;
      rectangle.Height = 50;
      rectangle.Fill   = new SolidColorBrush(Colors.RoyalBlue);

      rectangle.MouseDown += OnCanvasMouseDown;
      rectangle.MouseMove += OnCanvasMouseMove;
      rectangle.MouseUp   += OnCanvasMouseUp;

      System.Windows.Controls.Canvas.SetLeft(rectangle, 0);
      System.Windows.Controls.Canvas.SetTop(rectangle, 0);

      Canvas.Children.Add(rectangle);
    }

    private void OnCanvasMouseDown(object sender, MouseButtonEventArgs e)
    {
      isDragging = true;
      startPoint = Mouse.GetPosition(Canvas);
    }

    private void OnCanvasMouseMove(object sender, MouseEventArgs e)
    {
      if (isDragging)
      {
        var draggedRectangle = sender as Rectangle;
        var newPoint         = Mouse.GetPosition(Canvas);

        var left = System.Windows.Controls.Canvas.GetLeft(draggedRectangle);
        var top  = System.Windows.Controls.Canvas.GetTop(draggedRectangle);

        System.Windows.Controls.Canvas.SetLeft(draggedRectangle, left + (newPoint.X - startPoint.X));
        System.Windows.Controls.Canvas.SetTop(draggedRectangle, top + (newPoint.Y - startPoint.Y));

        startPoint = newPoint;
      }
    }

    private void OnCanvasMouseUp(object sender, MouseButtonEventArgs e)
    {
      isDragging = false;
    }
  }
}