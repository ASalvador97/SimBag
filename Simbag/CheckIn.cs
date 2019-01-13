using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Simbag
{
    [Serializable]
    class CheckIn : Node
    {
        // Properties
        public Node FinalDestination { get; set; }
        public int NumberOfBags { get; set; }
        public int ChanceSuspicious { get; set; }

        public CheckIn(string id, Point location) : base(id, location)
        {
            this.Sprite = Properties.Resources.Checkin2;
        }

        // Methods
        public override void MakeConnection(Node node)
        {
            if (CheckConnections())
            {
                Random r = new Random();
                double length = StraightLineDistanceTo(node);
                Connections.Add(new Transport(r.Next(0, 1111).ToString(), length, node));
            }
        }
        public override bool CheckConnections()
        {
            if (Connections.Count == 0)
            {
                return true;
            }
            else
            {
                return false;
            }
        }
    }
}
