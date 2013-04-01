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
        private TextBox _stardateBox;
        private Canvas _canvas;
        public ToggleButton ServerButton { get; private set; }
        private Dictionary<int, Planet> _planets = new Dictionary<int, Planet>();
        private Dictionary<int, Formation> _formations = new Dictionary<int, Formation>();
        private List<Canvas> _planetCanvases = new List<Canvas>();
        private Dictionary<int, Canvas> _formationCanvases = new Dictionary<int, Canvas>();
        private Point _origin = new Point(400, 300);

        public ClientWindow(Func<IMOOServiceCallback, MOOServiceClient> createService, State state)
        {
            Loaded += (sender, args) => ServerButton.IsChecked = true;
            _createService = () =>
            {
                var callbackHandler = new MOOCallbackHandler(_state);
                callbackHandler.Updated += () => Dispatcher.InvokeAsync(UpdateState);
                return createService(callbackHandler);
            };
            _state = state;
            SetupWindow();
        }

        private void UpdateState()
        {
            UpdatePlanets();
            UpdateFormations();
        }

        private void UpdateFormations()
        {
            var newFormations = _service.GetFormations();
            var missingFormations = newFormations.Where(f => !_formations.ContainsKey(f.id));
            var changedFormations = newFormations.Where(f => _formations.ContainsKey(f.id) && !_formations[f.id].Equals(f));
            foreach (var formation in changedFormations)
            {
                var planetCanvas = _planetCanvases[formation.location.item];
                planetCanvas.Children.Remove(_formationCanvases[formation.id]);
                _formationCanvases.Remove(formation.id);
                _formations.Remove(formation.id);
            }
            foreach (var formation in missingFormations.Union(changedFormations))
            {
                var formationCanvas = CreateFormationCanvas(formation);
                _planetCanvases[formation.location.item].Children.Add(formationCanvas);
                _formationCanvases[formation.id] = formationCanvas;
                _formations[formation.id] = formation;
            }
        }

        private void UpdatePlanets()
        {
            _planets = _service.GetPlanets().ToDictionary(p => p.id);
            foreach (var c in _planetCanvases) _canvas.Children.Remove(c);
            _planetCanvases.Clear();
            foreach (var planet in _planets.Values)
            {
                var canvas = CreatePlanetCanvas(planet);
                _planetCanvases.Add(canvas);
                _canvas.Children.Add(canvas);
            }
        }

        private void ConnectToServer()
        {
            ServerButton.Content = "Server OK";
            ServerButton.IsEnabled = false;
            _service = _createService();
            _service.Authenticate(Environment.UserName);
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

            _stardateBox = CreateDateTimeBox();
            DockPanel.SetDock(_stardateBox, Dock.Left);
            topPanel.Children.Add(_stardateBox);

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
            var dateTimeBinding = new Binding("Stardate") { Source = _state, StringFormat = "yyyy-MM-dd HH:mm" };
            dateTimeBox.SetBinding(TextBox.TextProperty, dateTimeBinding);
            return dateTimeBox;
        }

        private Canvas CreateCanvas()
        {
            var canvas = new Canvas { ClipToBounds = true };
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

        private Shape CreatePlanetShape(Planet planet)
        {
            var planetRadius = 20;
            var shape = new Ellipse
            {
                Width = planetRadius * 2,
                Height = planetRadius * 2,
                Fill = Brushes.Green,
            };
            Canvas.SetTop(shape, -planetRadius);
            Canvas.SetLeft(shape, -planetRadius);
            return shape;
        }

        private Canvas CreateFormationCanvas(Formation formation)
        {
            var planet = _planets[formation.location.item];
            var polygon = new Polygon { Fill = Brushes.Gray };
            polygon.Points.Add(new Point(0, 0));
            polygon.Points.Add(new Point(20, 10));
            polygon.Points.Add(new Point(20, -10));
            Canvas.SetLeft(polygon, 10);
            Canvas.SetTop(polygon, -25);
            var text = new TextBlock(new Run(formation.ships.ToString())) { Foreground = Brushes.White };
            Canvas.SetLeft(text, 20);
            Canvas.SetTop(text, -34);
            var canvas = new Canvas { Width = 0, Height = 0, ClipToBounds = false };
            canvas.Children.Add(polygon);
            canvas.Children.Add(text);
            return canvas;
        }

        private Canvas CreatePlanetCanvas(Planet planet)
        {
            var canvas = new Canvas
            {
                Width = 0,
                Height = 0,
                ClipToBounds = false,
            };
            var planetShape = CreatePlanetShape(planet);
            canvas.Children.Add(planetShape);

            var storyboard = CreatePlanetOrbit(planet, canvas);
            canvas.Loaded += (sender, args) => storyboard.Begin();
            return canvas;
        }

        private Storyboard CreatePlanetOrbit(Planet planet, DependencyObject canvas)
        {
            var orbitRadius = 70 * planet.orbit;
            var orbitSize = new Size(orbitRadius, orbitRadius);
            var startPoint = _origin - new Vector(0, orbitRadius);
            var midPoint = _origin + new Vector(0, orbitRadius);
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
            Storyboard.SetTarget(xAnim, canvas);
            Storyboard.SetTarget(yAnim, canvas);
            Storyboard.SetTargetProperty(xAnim, new PropertyPath(Canvas.LeftProperty));
            Storyboard.SetTargetProperty(yAnim, new PropertyPath(Canvas.TopProperty));
            return storyboard;
        }
    }
}
