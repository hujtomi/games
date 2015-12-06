using System;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;

namespace Torpedo.Modell
{
    public class Ship
    {
        public Point StartPoint { get; set; }
        public short Direction { get; set; }
        public short Length { get; set; }
    }
}
