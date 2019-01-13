using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Simbag
{
    [Serializable]
    class AStarSearch
    {
        // Fields and properties
        public double DistanceTraveled { get; set; }
        public Map Map { get; set; }
        public Node Start { get; set; }
        public Node End { get; set; }
        private Bag bag;
        private bool distadded = false;

        public AStarSearch(Map map, Bag bag)
        {
            Map = map;
            End = map.EndNode;
            Start = map.StartNode;
            this.bag = bag;
        }

        // Methods
        public List<Node> Search()
        {
            foreach(var node in Map.Nodes)
            {
                node.StraightLineDistanceToEnd = node.StraightLineDistanceTo(End);
            }
            List<Node> shortestPath = new List<Node>();
            shortestPath.Add(Start);
            Node currentNode = Start;
            while(currentNode != End)
            {
                if (currentNode is SecurityCheckPoint)
                {
                    bool suspicious = bag.Suspicious;
                    if (suspicious)
                    {
                        bag.Sprite = Properties.Resources.Bag_Suspicious;
                        if (NextSecurityNode(currentNode) == null) { End = currentNode; break; }
                        currentNode = NextSecurityNode(currentNode);
                        shortestPath.Add(currentNode);
                    } else
                    {
                        currentNode = NextNonSecurityNode(currentNode);
                        shortestPath.Add(currentNode);
                    }
                }
                else
                {
                    currentNode = NextNode(currentNode);
                    shortestPath.Add(currentNode);
                }
            }
            return shortestPath;
        }
        private Node NextNonSecurityNode(Node currentNode)
        {
            Transport lowestEdge = null;
            foreach (var cnn in currentNode.Connections)
            {
                if (lowestEdge == null)
                {
                    lowestEdge = cnn;
                }
                else
                {
                    if ((cnn.Length + cnn.Cost) < (lowestEdge.Length + lowestEdge.Cost) && cnn.ConnectedNode.StraightLineDistanceToEnd < lowestEdge.ConnectedNode.StraightLineDistanceToEnd)
                    {
                        if (cnn.ConnectedNode is SecurityCheckPoint) { }
                        else { lowestEdge = cnn; }
                    }
                }
            }
            if (lowestEdge != null)
            {

                Node result = lowestEdge.ConnectedNode;
                lowestEdge.visited = true;
                foreach (var edges in result.Connections)
                {
                    edges.Cost = lowestEdge.Length + lowestEdge.Cost;
                }
                DistanceTraveled += lowestEdge.Length;
                return result;
            }
            else { return null; }
        }
        private Node NextSecurityNode(Node currentNode)
        {
            
            Transport lowestEdge = null;
            foreach(Transport cnn in currentNode.Connections)
            {
                //MessageBox.Show(cnn.ConnectedNode.id);
                if (cnn.ConnectedNode is SecurityCheckPoint)
                {
                    lowestEdge = cnn;
                    
                }
            }
            if(lowestEdge == null) { return null; }
            if (!distadded)
            {
                DistanceTraveled += lowestEdge.Length;
                distadded = true;
            }
            return lowestEdge.ConnectedNode;
        }
        private Node NextNode(Node currentNode)
        {
            Transport lowestEdge = null;
            foreach (var cnn in currentNode.Connections)
            {
                if (lowestEdge == null)
                {
                    lowestEdge = cnn;
                }
                else
                {
                    //if (cnn.ConnectedNode is Gate && cnn.ConnectedNode != End) { }
                    //else
                    //{
                        if ((cnn.Length + cnn.Cost) < (lowestEdge.Length + lowestEdge.Cost) && cnn.ConnectedNode.StraightLineDistanceToEnd < lowestEdge.ConnectedNode.StraightLineDistanceToEnd)
                        {
                            lowestEdge = cnn;
                        }
                        else if(cnn.ConnectedNode is Gate && cnn.ConnectedNode == End) { lowestEdge = cnn; break; }
                    //}
                }
            }
            if (lowestEdge != null)
            {
               
                Node result = lowestEdge.ConnectedNode;
                lowestEdge.visited = true;
                foreach(var edges in result.Connections)
                {
                    edges.Cost = lowestEdge.Length + lowestEdge.Cost;
                }
                DistanceTraveled += lowestEdge.Length;
                return result;
            }
            else { return null; }
        }
    }
}
