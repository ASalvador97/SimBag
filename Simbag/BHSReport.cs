using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Simbag
{
    class BHSReport
    {
        // Fields
        public int NormalBags;
        public int SuspiciousBags;
        public int SuspiciousPrc;
        public double AvgTime;
        public double AvgDistance;
        public double SuspiciosTime;
        public double NormalTime;
        public double SuspiciosDistance;
        public double NormalDistance;
        public List<string> GateInfo;

        public BHSReport(int nb,int sb,double avgtime,double avgdistance,List<string>gateinfo, int susprc,double st,double nt,double sd, double nd)
        {
            this.NormalBags = nb;
            this.SuspiciousBags = sb;
            this.AvgTime = avgtime;
            this.AvgDistance = avgdistance;
            this.GateInfo = gateinfo;
            this.SuspiciousPrc = susprc;
            this.SuspiciosTime = st;
            this.NormalTime = nt;
            this.SuspiciosDistance = sd;
            this.NormalDistance = nd;
        }
    }
}
