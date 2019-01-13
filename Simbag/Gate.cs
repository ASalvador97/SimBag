using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Simbag
{
    [Serializable]
    class Gate : Node
    {
        // Properties
        public int CountBag { get; set; }

        public Gate(string id, Point location) : base( id, location)
        {
            this.Sprite = Properties.Resources.Endpoint2;
        }

        // Methods
        public override void MakeConnection(Node node)
        {
            if (CheckConnections())
            {

            }
        }
        public override bool CheckConnections()
        {
            return true;
        }
    }
}
