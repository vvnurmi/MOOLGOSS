using MOO.Client.MOOService;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Media;
using System.Windows.Shapes;

namespace MOO.Client.GUI
{
    public class CommandGraphics
    {
        private ClientWindow.GraphicsData _gfx;
        private List<CommandCBase> _issuedCommands = new List<CommandCBase>();

        public CommandGraphics(ClientWindow.GraphicsData gfx)
        {
            _gfx = gfx;
        }

        public void Clear()
        {
            RemoveAll<CommandCBase>(c => true);
        }

        public void Add(CommandCMoveFormation command)
        {
            RemoveAll<CommandCMoveFormation>(c => c.ID == command.ID);
            _issuedCommands.Add(command);
            var line = new Line
            {
                Name = "Move",
                Stroke = Brushes.LightBlue,
                StrokeThickness = 3,
                StrokeDashArray = new DoubleCollection(new[] { 1.5, 2.5 }),
                X1 = 0,
                Y1 = 0,
            };
            var x2Binding = new GenericBinding<double>(() =>
                {
                    var transform = _gfx.Formations[command.ID].TransformToVisual(_gfx.Canvas);
                    var formationPos = transform.Transform(new Point(0, 0));
                    return Canvas.GetLeft(_gfx.Planets[command.Location.item]) - formationPos.X;
                },
                new Binding("(Canvas.Left)") { Source = _gfx.Planets[command.Location.item] },
                new Binding("(Canvas.Left)") { Source = _gfx.Formations[command.ID] });
            var y2Binding = new GenericBinding<double>(() =>
                {
                    var transform = _gfx.Formations[command.ID].TransformToVisual(_gfx.Canvas);
                    var formationPos = transform.Transform(new Point(0, 0));
                    return Canvas.GetTop(_gfx.Planets[command.Location.item]) - formationPos.Y;
                },
                new Binding("(Canvas.Top)") { Source = _gfx.Planets[command.Location.item] },
                new Binding("(Canvas.Top)") { Source = _gfx.Formations[command.ID] });
            line.SetBinding(Line.X2Property, x2Binding);
            line.SetBinding(Line.Y2Property, y2Binding);
            _gfx.Formations[command.ID].Children.Add(line);
        }

        private void RemoveAll<T>(Func<T, bool> filter) where T : CommandCBase
        {
            foreach (var command in _issuedCommands.OfType<T>().Where(filter).ToArray())
            {
                _issuedCommands.Remove(command);
                var moveFormation = command as CommandCMoveFormation;
                if (moveFormation != null) Remove(moveFormation);
            }
        }

        private void Remove(CommandCMoveFormation command)
        {
            var canvas = _gfx.Formations[command.ID];
            var oldLine = canvas.Children.OfType<Line>().First(l => l.Name == "Move");
            canvas.Children.Remove(oldLine);
        }
    }
}
