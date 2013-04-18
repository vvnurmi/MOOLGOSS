using MOO.Client.MOOService;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Timers;
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
        public class GraphicsData
        {
            public Canvas Canvas;
            public Point Origin = new Point(400, 300);
            public Dictionary<int, PlanetCanvas> Planets = new Dictionary<int, PlanetCanvas>();
            public Dictionary<int, Canvas> Formations = new Dictionary<int, Canvas>();
        }

        public class PlanetCanvas : Canvas
        {
            public Storyboard Storyboard { get; set; }
        }

        private Func<string, MOOServiceClient> _createService;
        private MOOServiceClient _service;
        private GraphicsData _gfx = new GraphicsData();
        private CommandGraphics _commandGraphics;
        private State _state;
        private Func<bool> _dragAllowed;
        private TextBox _stardateBox;
        public ToggleButton ServerButton { get; private set; }
        private Dictionary<int, Planet> _planets = new Dictionary<int, Planet>();
        private Dictionary<int, Formation> _formations = new Dictionary<int, Formation>();
        private Timer _updateTimer;

        public ClientWindow(Func<string, MOOServiceClient> createService, State state)
        {
            _commandGraphics = new CommandGraphics(_gfx);
            Loaded += (sender, args) => ServerButton.IsChecked = true;
            _createService = createService;
            _state = state;
            SetupWindow();
            _dragAllowed = this.PrepareForDragDrop();
            _updateTimer = new Timer(1000);
            _updateTimer.Elapsed += (sender, args) => Dispatcher.Invoke(UpdateState);
            _updateTimer.Start();
        }

        private void UpdateState()
        {
            var update = _service.GetUpdate();
            if (update.Stardate != _state.Stardate)
            {
                _updateTimer.Interval = (update.NextUpdate + TimeSpan.FromSeconds(1)).TotalMilliseconds;
                _state.Stardate = update.Stardate;
                _commandGraphics.Clear();
                UpdatePlanets();
                UpdateFormations();
            }
        }

        private void UpdateFormations()
        {
            var newFormations = _service.GetFormations();
            var missingFormations = newFormations.Where(f => !_formations.ContainsKey(f.ID)).ToArray();
            var changedFormations = newFormations.Where(f => _formations.ContainsKey(f.ID) && !_formations[f.ID].Equals(f)).ToArray();
            foreach (var formation in changedFormations)
            {
                var planetCanvas = _gfx.Planets[_formations[formation.ID].Location.item];
                planetCanvas.Children.Remove(_gfx.Formations[formation.ID]);
                _gfx.Formations.Remove(formation.ID);
                _formations.Remove(formation.ID);
            }
            foreach (var formation in missingFormations.Union(changedFormations))
            {
                var formationCanvas = CreateFormationCanvas(formation);
                var planetCanvas = _gfx.Planets[formation.Location.item];
                planetCanvas.Children.Add(formationCanvas);
                _gfx.Formations[formation.ID] = formationCanvas;
                _formations[formation.ID] = formation;
            }
        }

        private void UpdatePlanets()
        {
            var newPlanets = _service.GetPlanets();
            var missingPlanets = newPlanets.Where(p => !_planets.ContainsKey(p.ID));
            var changedPlanets = newPlanets.Where(p => _planets.ContainsKey(p.ID) && !_planets[p.ID].Equals(p));
            var animationTimes = changedPlanets.ToDictionary(p => p.ID,
                p => _gfx.Planets[p.ID].Storyboard.GetCurrentTime(_gfx.Planets[p.ID]));
            foreach (var planet in changedPlanets)
            {
                var planetCanvas = _gfx.Planets[planet.ID];
                _gfx.Canvas.Children.Remove(planetCanvas);
                _gfx.Planets.Remove(planet.ID);
                _planets.Remove(planet.ID);
            }
            foreach (var planet in missingPlanets.Union(changedPlanets))
            {
                var planetCanvas = CreatePlanetCanvas(planet);
                if (animationTimes.ContainsKey(planet.ID))
                    planetCanvas.Loaded += (sender, args) =>
                        planetCanvas.Storyboard.Seek(planetCanvas, animationTimes[planet.ID].Value, TimeSeekOrigin.BeginTime);
                _gfx.Planets[planet.ID] = planetCanvas;
                _gfx.Canvas.Children.Add(planetCanvas);
                _planets[planet.ID] = planet;
            }
        }

        private void ConnectToServer()
        {
            ServerButton.Content = "Server OK";
            ServerButton.IsEnabled = false;
            _service = _createService(Environment.UserName);
            if (_service == null) AbandonServer();
        }

        public void AbandonServer()
        {
            _service = null;
            ServerButton.Content = "No server";
            ServerButton.IsEnabled = true;
            MessageBox.Show("There was a communication error. Perhaps the server is offline?", "MOO Communication Error");
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
            _gfx.Canvas = CreateCanvas();
            DockPanel.SetDock(_gfx.Canvas, Dock.Bottom);
            panel.Children.Add(_gfx.Canvas);
            return panel;
        }

        private DockPanel CreateTopPanel()
        {
            var topPanel = new DockPanel();

            ServerButton = new ToggleButton { Background = Brushes.Red };
            ServerButton.Checked += (sender, args) => ConnectToServer();
            ServerButton.Unchecked += (sender, args) => AbandonServer();
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
            Canvas.SetLeft(star, _gfx.Origin.X - starRadius);
            Canvas.SetTop(star, _gfx.Origin.Y - starRadius);
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
                AllowDrop = true,
            };
            shape.Drop += (sender, args) =>
            {
                if (!args.Data.GetDataPresent(typeof(Formation))) return;
                var formation = (Formation)args.Data.GetData(typeof(Formation));
                Command_MoveFormation(formation, planet);
            };
            Canvas.SetTop(shape, -planetRadius);
            Canvas.SetLeft(shape, -planetRadius);
            return shape;
        }

        private void Command_MoveFormation(Formation formation, Planet target)
        {
            var command = new CommandCMoveFormation
            {
                ID = formation.ID,
                Location = new Location { item = target.ID }
            };
            _service.IssueCommand(command);
            _commandGraphics.Add(command);
        }

        private TextBlock CreatePlanetText(Planet planet)
        {
            var textBlock = new TextBlock
            {
                Text = string.Format("{0}\n{1}/{2}", planet.Player, planet.Population, planet.MaxPopulation),
                Foreground = Brushes.White,
                IsHitTestVisible = false,
            };
            var textSize = textBlock.MeasureString(textBlock.Text);
            Canvas.SetLeft(textBlock, -textSize.Width / 2);
            Canvas.SetTop(textBlock, -textSize.Height / 2);
            return textBlock;
        }

        private Canvas CreateFormationCanvas(Formation formation)
        {
            var planet = _planets[formation.Location.item];
            var polygon = new Polygon { Fill = Brushes.Gray };
            polygon.Points.Add(new Point(-10, 0));
            polygon.Points.Add(new Point(10, 10));
            polygon.Points.Add(new Point(10, -10));
            polygon.MouseMove += (sender, args) =>
            {
                if (_dragAllowed()) DragDrop.DoDragDrop(polygon, formation, DragDropEffects.Move);
            };
            var text = new TextBlock
            {
                Text = formation.Ships.ToString(),
                Foreground = Brushes.White,
                IsHitTestVisible = false,
            };
            Canvas.SetLeft(text, 0);
            Canvas.SetTop(text, -9);
            var canvas = new Canvas { Width = 0, Height = 0, ClipToBounds = false };
            canvas.Children.Add(polygon);
            canvas.Children.Add(text);
            Canvas.SetLeft(canvas, 20);
            Canvas.SetTop(canvas, -25);
            return canvas;
        }

        private PlanetCanvas CreatePlanetCanvas(Planet planet)
        {
            var canvas = new PlanetCanvas
            {
                Width = 0,
                Height = 0,
                ClipToBounds = false,
            };
            var planetShape = CreatePlanetShape(planet);
            canvas.Children.Add(planetShape);
            var planetText = CreatePlanetText(planet);
            canvas.Children.Add(planetText);

            var storyboard = CreatePlanetOrbit(planet);
            canvas.Storyboard = storyboard;
            canvas.Loaded += (sender, args) => storyboard.Begin(canvas, true);
            return canvas;
        }

        private Storyboard CreatePlanetOrbit(Planet planet)
        {
            var orbitRadius = 70 * planet.Orbit;
            var orbitSize = new Size(orbitRadius, orbitRadius);
            var startPoint = _gfx.Origin - new Vector(0, orbitRadius);
            var midPoint = _gfx.Origin + new Vector(0, orbitRadius);
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
                Name = "X",
                RepeatBehavior = RepeatBehavior.Forever,
                Duration = TimeSpan.FromSeconds(15),
                Source = PathAnimationSource.X,
                PathGeometry = orbitPath,
            };
            var yAnim = new DoubleAnimationUsingPath()
            {
                Name = "Y",
                RepeatBehavior = RepeatBehavior.Forever,
                Duration = TimeSpan.FromSeconds(15),
                Source = PathAnimationSource.Y,
                PathGeometry = orbitPath,
            };
            var storyboard = new Storyboard();
            storyboard.Children.Add(xAnim);
            storyboard.Children.Add(yAnim);
            Storyboard.SetTargetProperty(xAnim, new PropertyPath(Canvas.LeftProperty));
            Storyboard.SetTargetProperty(yAnim, new PropertyPath(Canvas.TopProperty));
            return storyboard;
        }
    }
}
