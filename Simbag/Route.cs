using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Simbag
{
    public class Route
    {
        int _cost;
        List<Transport> _connections;
        string _identifier;

        public Route(string _identifier)
        {
            _cost = int.MaxValue;
            _connections = new List<Transport>();
            this._identifier = _identifier;
        }


        public List<Transport> Transports
        {
            get { return _connections; }
            set { _connections = value; }
        }
        public int Cost
        {
            get { return _cost; }
            set { _cost = value; }
        }

        public override string ToString()
        {
            return "Id:" + _identifier + " Cost:" + Cost;
        }
    }
}
