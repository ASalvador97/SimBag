using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;
using System.Text.RegularExpressions;

namespace Simbag
{
    [Serializable]
    public class Transport
    {
        // Fields and properties
        public string id;
        public int speed;
        private double length;
        public bool visited;
        public Node startingNode;
        public Node endingNode;
        public Node ConnectedNode { get; set; }
        public double Cost { get; set; }
        public double Length
        {
            get { return length; }
            set { length = Math.Round(value, 0); }
        }

        public Transport(string id, double length, Node connectedNode)
        {
            this.visited = false;
            this.ConnectedNode = connectedNode;
            this.Cost = 0;
            this.Length = Math.Round(length,0);
            this.id = id;
        }
    }
}
