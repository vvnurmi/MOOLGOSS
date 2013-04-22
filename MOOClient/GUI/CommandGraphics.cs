using MOO.Service;
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
        private List<Command> _issuedCommands = new List<Command>();

        public CommandGraphics(ClientWindow.GraphicsData gfx)
        {
            _gfx = gfx;
        }

        public void Clear()
        {
            RemoveAll(c => true);
        }

        public void Add(MoveFormationData command)
        {
            RemoveAll(c => c.Type == CommandType.MoveFormation && c.MoveFormationData.Formation == command.Formation);
            _issuedCommands.Add(new Command { Type = CommandType.MoveFormation, MoveFormationData = command });
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
                    var transform = _gfx.Formations[command.Formation].TransformToVisual(_gfx.Canvas);
                    var formationPos = transform.Transform(new Point(0, 0));
                    return Canvas.GetLeft(_gfx.Planets[command.Destination.Planet]) - formationPos.X;
                },
                new Binding("(Canvas.Left)") { Source = _gfx.Planets[command.Destination.Planet] },
                new Binding("(Canvas.Left)") { Source = _gfx.Formations[command.Formation] });
            var y2Binding = new GenericBinding<double>(() =>
                {
                    var transform = _gfx.Formations[command.Formation].TransformToVisual(_gfx.Canvas);
                    var formationPos = transform.Transform(new Point(0, 0));
                    return Canvas.GetTop(_gfx.Planets[command.Destination.Planet]) - formationPos.Y;
                },
                new Binding("(Canvas.Top)") { Source = _gfx.Planets[command.Destination.Planet] },
                new Binding("(Canvas.Top)") { Source = _gfx.Formations[command.Formation] });
            line.SetBinding(Line.X2Property, x2Binding);
            line.SetBinding(Line.Y2Property, y2Binding);
            _gfx.Formations[command.Formation].Children.Add(line);
        }

        private void RemoveAll(Func<Command, bool> filter)
        {
            foreach (var command in _issuedCommands.Where(filter).ToArray())
            {
                _issuedCommands.Remove(command);
                switch (command.Type)
                {
                    case CommandType.MoveFormation: Remove(command.MoveFormationData); break;
                    default: throw new InvalidOperationException();
                }
            }
        }

        private void Remove(MoveFormationData command)
        {
            var canvas = _gfx.Formations[command.Formation];
            var oldLine = canvas.Children.OfType<Line>().First(l => l.Name == "Move");
            canvas.Children.Remove(oldLine);
        }
    }
}
