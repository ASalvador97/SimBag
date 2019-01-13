using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Simbag
{
    [Serializable]
    public class Map
    {
        // Properties
        public List<Node> Nodes { get; set; } = new List<Node>();
        public Node StartNode { get; set; }
        public Node EndNode { get; set; }

        // Methods
        public void Draw(Graphics gr)
        {
            foreach (var node in Nodes)
            {
                Point point = node.Location;
                foreach (var cnn in node.Connections)
                {
                    Point point1 = node.Location;
                    Point point2 = cnn.ConnectedNode.Location;
                    gr.DrawLine(new Pen(Color.Black, 3), point1, point2);

                    int difX = point2.X - point1.X;
                    int difY = point2.Y - point1.Y;
                    Point p = new Point((point1.X) + (difX / 2) + 10, (point2.Y) + (difY / 2) + 10);

                    gr.DrawString(cnn.Length.ToString(), new Font("Arial", 8, FontStyle.Regular), Brushes.Black, p);
                }
                node.Draw(gr);
            }
        }
    }
}
