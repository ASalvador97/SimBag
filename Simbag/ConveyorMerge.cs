using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Simbag
{
    [Serializable]
    class ConveyorMerge : Node
    {
        public ConveyorMerge(string id, Point location) : base(id, location)
        {
            this.Sprite = Properties.Resources.ConveyorMerge2;
        }

        // Methods
        public override void MakeConnection(Node node)
        {
            if (Connections.Count < 3)
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
