using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;

namespace Simbag
{
    [Serializable]
    public class Bag
    {
        // Fields and properties
        public string id;   
        private int x;
        private int y;
        private int nextx;
        private int nexty;
        private int listIndex;
        private int deltax;
        private int deltay;
        private int stepx;
        private int stepy;
        private int countx =0;
        private int county=0;
        public double traveltime = 0;
        private bool started = false;
        public bool finished = false;
        private DateTime start;
        private DateTime finish;
        private List<Point> pointList;
        public Node StartingPoint { get; set; }
        public Node EndPoint { get; set; }
        public bool Suspicious { get; set; }
        public Image Sprite { get; set; }

        public Bag(string id, Node startingPoint, Node endPoint, bool suspicious)
        {
            this.id = id;
            this.Sprite = Properties.Resources.Bag;
            this.StartingPoint = startingPoint;
            this.EndPoint = endPoint;

            pointList = new List<Point>();
            listIndex = 1;
            Suspicious = suspicious;

            x = startingPoint.Location.X;
            y = startingPoint.Location.Y-15;
            
        }

        // Methods
        public void Draw(Graphics gr,int state)
        {
            if(!started)
            {
                start = DateTime.Now;
                started = true;
            }
            if (state == 0)
            {
                nextx = this.pointList[listIndex].X;
                nexty = this.pointList[listIndex].Y-15;
                deltax = nextx - x;
                deltay = nexty - y;

                if (deltax == 0)
                {
                    stepx = 0;
                }
                else
                {
                    if (deltay != 0)
                    {
                        stepx = deltax / deltay;
                    }
                }
                if (deltay == 0)
                {
                    stepy = 0;
                }
                else
                {
                    if (deltax != 0)
                    {
                        stepy = deltay / deltax;
                    }
                }
                if (countx == Math.Abs(stepx) && county == Math.Abs(stepy))
                {
                    countx = 0;
                    county = 0;

                }
                if (countx < Math.Abs(stepx))
                {
                    if (x < nextx)
                    {
                        x += 1;
                    }
                    else if (x > nextx)
                    {
                        x -= 1;
                    }
                    countx++;
                }
                if (county < Math.Abs(stepy))
                {
                    if (y < nexty)
                    {
                        y += 1;
                    }
                    else if (y > nexty)
                    {
                        y -= 1;
                    }
                    county++;
                }
                if (y == nexty && x == nextx)
                {
                    if (listIndex < pointList.Count - 1)
                    {
                        listIndex++;
                    }
                    nextx = pointList[listIndex].X;
                    nexty = pointList[listIndex].Y-15;
                    countx = 0;
                    county = 0;
                }
                if (countx == Math.Abs(stepx) && county == Math.Abs(stepy) && y != nexty && x != nextx || nextx - x == 1 || nexty - y == 1)
                {
                    if (x < nextx)
                    {
                        x += 1;
                    }
                    else if (x > nextx)
                    {
                        x -= 1;
                    }
                    else if (x == nextx && y != nexty)
                    {
                        if (y < nexty)
                        {
                            y += 1;
                        }
                        else if (y > nexty)
                        {
                            y -= 1;
                        }
                    }
                    if (y < nexty)
                    {
                        y += 1;
                    }
                    else if (y > nexty)
                    {
                        y -= 1;
                    }
                    else if (y == nexty && x != nextx)
                    {
                        if (x < nextx)
                        {
                            x += 1;
                        }
                        else if (x > nextx)
                        {
                            x -= 1;
                        }
                    }
                    countx = 0;
                    county = 0;
                }
            }
            if (state == 2)
            {
                listIndex = 1;
                x = pointList[0].X;
                y = pointList[0].Y-15;
                countx = 0;
                county = 0;
            }
            if(x==pointList[pointList.Count-1].X && y==pointList[pointList.Count - 1].Y-15 && !finished)
            {
                finish = DateTime.Now;
                finished = true;
                traveltime = (finish - start).TotalSeconds;
            }
            Size size = new Size(Sprite.Width, Sprite.Height);
            Rectangle rect = new Rectangle(new Point(x, y), size);
            gr.DrawImage(Sprite, rect);           
        }
        public void GetNextLocation(List<Node> nodes)
        {  
            foreach(Node n in nodes)
            {
                pointList.Add(n.Location);
            }    
        }   
    }
}
