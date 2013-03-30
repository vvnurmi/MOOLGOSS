using MOO.Client.MOOService;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
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
    public class ClientWindow : Window
    {
        private Func<MOOServiceClient> _createService;
        private MOOServiceClient _service;
        private State _state;
        private TextBox _dateTimeBox;
        private Canvas _canvas;
        public ToggleButton ServerButton { get; private set; }
        private Planet[] _planets = new Planet[0];
        private List<Ellipse> _planetEllipses = new List<Ellipse>();
        private Point _origin = new Point(400, 300);

        public ClientWindow(Func<MOOServiceClient> createService, State state)
        {
            Loaded += (sender, args) => ServerButton.IsChecked = true;
            _createService = createService;
            _service = createService();
            _state = state;
            SetupWindow();
        }

        private void UpdatePlanets()
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

        private void ConnectToServer()
        {
            ServerButton.Content = "Server OK";
            ServerButton.IsEnabled = false;
            _service = _createService();
            _service.Authenticate(Environment.UserName);
            UpdatePlanets();
        }

        private void SetupWindow()
        {
            Background = Brushes.Black;
            Width = 800;
            Height = 600;
            Content = CreateMainPanel();
        }

        private DockPanel CreateMainPanel()
        {
            var panel = new DockPanel();
            var topPanel = CreateTopPanel();
            DockPanel.SetDock(topPanel, Dock.Top);
            panel.Children.Add(topPanel);
            _canvas = CreateCanvas();
            DockPanel.SetDock(_canvas, Dock.Bottom);
            panel.Children.Add(_canvas);
            return panel;
        }

        private DockPanel CreateTopPanel()
        {
            var topPanel = new DockPanel();

            ServerButton = new ToggleButton { Background = Brushes.Red };
            ServerButton.Checked += (sender, args) => ConnectToServer();
            ServerButton.Unchecked += (sender, args) => { ServerButton.Content = "No server"; ServerButton.IsEnabled = true; };
            DockPanel.SetDock(ServerButton, Dock.Right);
            topPanel.Children.Add(ServerButton);

            _dateTimeBox = CreateDateTimeBox();
            DockPanel.SetDock(_dateTimeBox, Dock.Left);
            topPanel.Children.Add(_dateTimeBox);

            return topPanel;
        }

        private TextBox CreateDateTimeBox()
        {
            var dateTimeBox = new TextBox
            {
                Background = Brushes.DarkGray,
                Foreground = Brushes.White,
                IsReadOnly = true,
            };
            var dateTimeBinding = new Binding("Now") { Source = _state };
            dateTimeBox.SetBinding(TextBox.TextProperty, dateTimeBinding);
            return dateTimeBox;
        }

        private Canvas CreateCanvas()
        {
            var canvas = new Canvas();
            var star = CreateStarEllipse();
            canvas.Children.Add(star);
            return canvas;
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
            var orbitRadius = 70 * planet.orbit;
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
