using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;
using System.Windows.Forms;

namespace Simbag
{
    [Serializable]
    public class Builder
    {
        // Fields
        public List<Node> nodeList = new List<Node>();
        public Map mymap = new Map();

        // Methods
        public bool AddToList(string type, string id, Point point)
        {
            Node node = CheckNode(point);
            {
                if (node != null)
                {
                    MessageBox.Show("Please select an empty place");
                    return false;
                }
                else
                {
                    switch (type)
                    {
                        case "CHECKIN":
                            mymap.Nodes.Add(new CheckIn(id, point));
                            return true;
                        case "GATE":
                            mymap.Nodes.Add(new Gate(id, point));
                            return true;
                        case "SECURITY":
                            mymap.Nodes.Add(new SecurityCheckPoint(id, point));
                            return true;
                        case "CONVEYORSPLIT":
                            mymap.Nodes.Add(new ConveyorSplit(id, point));
                            return true;
                        case "CONVEYORMERGE":
                            mymap.Nodes.Add(new ConveyorMerge(id, point));
                            return true;
                    }
                    return true;
                }
            }
        }
        public Node CheckNode(Point p)
        {
            foreach (Node e in mymap.Nodes)
            {
                if (e.CheckNodeLocation(p))
                {
                    return e;
                }
            }
            return null;
        }
        public void DeleteAnything(Point p)
        {
            Transport transport = null;
            Node node = CheckNode(p);
            if (node != null)
            {
                foreach (Node n in mymap.Nodes)
                {
                    foreach (Transport cnn in n.Connections)
                    {
                        if (cnn.ConnectedNode == node)
                        {
                            transport = cnn;
                        }
                    }
                    if (transport != null) { n.Connections.Remove(transport); }
                }
                mymap.Nodes.Remove(node);
            }
            else
            {
                MessageBox.Show("Please select Component you want to remove");
            }

        }
    }
}
