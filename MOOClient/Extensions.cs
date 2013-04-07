using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;

namespace MOO.Client
{
    public static class Extensions
    {
        private enum DragFilter { MustClick, MustMove, Ok };

        public static Size MeasureString(this TextBlock textBlock, string str)
        {
            var formattedText = new FormattedText(
                str,
                CultureInfo.CurrentUICulture,
                FlowDirection.LeftToRight,
                new Typeface(textBlock.FontFamily, textBlock.FontStyle, textBlock.FontWeight, textBlock.FontStretch),
                textBlock.FontSize,
                Brushes.Black);
            return new Size(formattedText.Width, formattedText.Height);
        }

        /// <summary>
        /// Returns a function that tells if drag'n' drop is allowed to start.
        /// </summary>
        public static Func<bool> PrepareForDragDrop(this Window window)
        {
            var state = DragFilter.MustClick;
            var clickPos = new Point();
            window.MouseLeftButtonDown += (sender, args) =>
            {
                if (state != DragFilter.MustClick) return;
                clickPos = Mouse.GetPosition(window);
                state = DragFilter.MustMove;
            };
            window.MouseLeftButtonUp += (sender, args) => state = DragFilter.MustClick;
            window.PreviewMouseMove += (sender, args) =>
            {
                if (state == DragFilter.MustMove && Mouse.GetPosition(window) != clickPos)
                    state = DragFilter.Ok;
            };
            window.MouseMove += (sender, args) =>
            {
                if (state == DragFilter.Ok)
                    state = DragFilter.MustClick;
            };
            return () => state == DragFilter.Ok;
        }
    }
}
