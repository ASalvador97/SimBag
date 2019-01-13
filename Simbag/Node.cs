using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Simbag
{
    [Serializable]
    public abstract class Node
    {
        // Fields and properties
        public string id;
        public Point location;
        public bool Connected { get; set; }
        public Image Sprite { get; set; }
        public List<Transport> Connections { get; set; }
        public double StraightLineDistanceToEnd { get; set; }
        public Point Location { get { return location; } set{ location = value; } }

        public Node(string id, Point location)
        {
            this.id = id;
            this.Location = location;
            this.Connections = new List<Transport>();
        }

        // Methods
        public double StraightLineDistanceTo(Node End)
        {
            return Math.Sqrt(Math.Pow(Location.X - End.Location.X, 2) + Math.Pow(Location.Y - End.Location.Y, 2));
        }
        public void Draw(Graphics gr)
        {
            Size size = new Size(Sprite.Width, Sprite.Height);
            Point point = new Point(Location.X - 15, Location.Y - 15);
            Rectangle rect = new Rectangle(point, size);
            gr.DrawImage(Sprite, rect);
            gr.DrawEllipse(Pens.Blue, rect);
        }
        public abstract void MakeConnection(Node node);
        public bool CheckNodeLocation(Point p)
        {
            if (p == Location || ((Location.X + 15 > p.X && Location.X < p.X + 15) && (Location.Y + 15 > p.Y && Location.Y < p.Y + 15)))
            {
                return true;
            }
            return false;
        }
        public virtual bool CheckConnections() { return false; } //checks if the Node had a valid open output connection
    }
}