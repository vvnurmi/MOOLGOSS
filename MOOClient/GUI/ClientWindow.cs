using MOO.Client.MOOService;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace MOO.Client.GUI
{
    public partial class ClientWindow : Window
    {
        private MOOServiceClient _service;
        private Planet[] _planets = new Planet[0];

        public ClientWindow(MOOServiceClient service)
        {
            _service = service;
            SetupControls();
        }

        protected override void OnKeyDown(KeyEventArgs e)
        {
            if (e.Key == Key.Space)
                _planets = _service.GetPlanets();
            base.OnKeyDown(e);
        }

        private void SetupControls()
        {
            Background = Brushes.Black;
            Width = 800;
            Height = 600;

            var panel = new DockPanel();
            Content = panel;

            var canvas = new Canvas();
            DockPanel.SetDock(canvas, Dock.Bottom);
            panel.Children.Add(canvas);

            var star = new Ellipse { Width = 50, Height = 50, Fill = Brushes.Yellow };
            Canvas.SetLeft(star, 400);
            Canvas.SetTop(star, 300);
            canvas.Children.Add(star);

            var planet = new Ellipse { Width = 20, Height = 20, Fill = Brushes.Green };
            Canvas.SetLeft(planet, 400);
            Canvas.SetTop(planet, 200);
            canvas.Children.Add(planet);

            var orbitFigure = new PathFigure { StartPoint = new Point(400, 200) };
            var orbitArc1 = new ArcSegment(new Point(400, 400), new Size(100, 100), 0, false, SweepDirection.Clockwise, false);
            var orbitArc2 = new ArcSegment(new Point(400, 200), new Size(100, 100), 0, false, SweepDirection.Clockwise, false);
            orbitFigure.Segments.Add(orbitArc1);
            orbitFigure.Segments.Add(orbitArc2);
            var orbitPath = new PathGeometry();
            orbitPath.Figures.Add(orbitFigure);
            orbitPath.Freeze();
            var xAnim = new DoubleAnimationUsingPath()
            {
                RepeatBehavior = RepeatBehavior.Forever,
                Duration = TimeSpan.FromSeconds(5),
                Source = PathAnimationSource.X,
                PathGeometry = orbitPath,
            };
            var yAnim = new DoubleAnimationUsingPath()
            {
                RepeatBehavior = RepeatBehavior.Forever,
                Duration = TimeSpan.FromSeconds(5),
                Source = PathAnimationSource.Y,
                PathGeometry = orbitPath,
            };

            var storyboard = new Storyboard();
            storyboard.Children.Add(xAnim);
            storyboard.Children.Add(yAnim);
            Storyboard.SetTarget(xAnim, planet);
            Storyboard.SetTarget(yAnim, planet);
            Storyboard.SetTargetProperty(xAnim, new PropertyPath(Canvas.LeftProperty));
            Storyboard.SetTargetProperty(yAnim, new PropertyPath(Canvas.TopProperty));
            planet.Loaded += (sender, args) => storyboard.Begin();
        }
    }
}
