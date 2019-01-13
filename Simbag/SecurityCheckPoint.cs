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
    class SecurityCheckPoint : Node
    {
        public SecurityCheckPoint(string id, Point location) : base( id, location)
        {
            this.Sprite = Properties.Resources.SecurityCheckpoint2;
        }
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
            if (Connections.Count >= 2)
            {
                return false;
            }
            else
            {
                return true;
            }
        }
    }
}
