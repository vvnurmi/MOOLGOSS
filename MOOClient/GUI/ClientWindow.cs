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
        private Canvas _canvas;
        private Planet[] _planets = new Planet[0];
        private List<Ellipse> _planetEllipses = new List<Ellipse>();
        private Point _origin = new Point(400, 300);

        public ClientWindow(MOOServiceClient service)
        {
            _service = service;
            SetupControls();
        }

        protected override void OnKeyDown(KeyEventArgs e)
        {
            if (e.Key == Key.Space)
            {
                _planets = _service.GetPlanets();
                foreach (var ellipse in _planetEllipses) _canvas.Children.Remove(ellipse);
                _planetEllipses.Clear();
                foreach (var planet in _planets)
                {
                    var ellipse = CreatePlanetEllipse(planet);
                    _planetEllipses.Add(ellipse);
                    _canvas.Children.Add(ellipse);
                }
            }
            base.OnKeyDown(e);
        }

        private void SetupControls()
        {
            Background = Brushes.Black;
            Width = 800;
            Height = 600;

            var panel = new DockPanel();
            Content = panel;

            _canvas = new Canvas();
            DockPanel.SetDock(_canvas, Dock.Bottom);
            panel.Children.Add(_canvas);

            var star = CreateStarEllipse();
            _canvas.Children.Add(star);
        }

        private Ellipse CreateStarEllipse()
        {
            var starRadius = 35;
            var star = new Ellipse
            {
                Width = starRadius * 2,
                Height = starRadius * 2,
                Fill = Brushes.Yellow,
            };
            Canvas.SetLeft(star, _origin.X - starRadius);
            Canvas.SetTop(star, _origin.Y - starRadius);
            return star;
        }

        private Ellipse CreatePlanetEllipse(Planet planet)
        {
            var planetRadius = 20;
            var orbitRadius = 70 * planet.Orbit;
            var orbitSize = new Size(orbitRadius, orbitRadius);
            var startPoint = _origin - new Vector(planetRadius, planetRadius + orbitRadius);
            var midPoint = _origin - new Vector(planetRadius, planetRadius - orbitRadius);
            var sphere = new Ellipse
            {
                Width = planetRadius * 2,
                Height = planetRadius * 2,
                Fill = Brushes.Green,
            };

            var orbitFigure = new PathFigure { StartPoint = startPoint };
            var orbitArc1 = new ArcSegment(midPoint, orbitSize, 0, false, SweepDirection.Clockwise, false);
            var orbitArc2 = new ArcSegment(startPoint, orbitSize, 0, false, SweepDirection.Clockwise, false);
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
            Storyboard.SetTarget(xAnim, sphere);
            Storyboard.SetTarget(yAnim, sphere);
            Storyboard.SetTargetProperty(xAnim, new PropertyPath(Canvas.LeftProperty));
            Storyboard.SetTargetProperty(yAnim, new PropertyPath(Canvas.TopProperty));
            sphere.Loaded += (sender, args) => storyboard.Begin();
            return sphere;
        }
    }
}
