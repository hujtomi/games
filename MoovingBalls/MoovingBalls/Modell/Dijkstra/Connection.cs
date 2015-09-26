using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RouteEngine
{
    public class Connection
    {
        Location _a, _b;
        int _weight;
        bool selected=false;

        public bool Selected
        {
            get { return selected; }
            set { selected = value; }
        }

        public Connection(Location a, Location b, int weight)
        {
            this._a = a;
            this._b = b;
            this._weight = weight;
        }
        public Location B
        {
            get { return _b; }
            set { _b = value; }
        }

        public Location A
        {
            get { return _a; }
            set { _a = value; }
        }       

        public int Weight
        {
            get { return _weight; }
            set { _weight = value; }
        }


    }
}
