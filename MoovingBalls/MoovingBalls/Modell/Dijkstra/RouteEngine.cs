using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RouteEngine
{
    public class RouteEngine
    {
        List<Connection> _connections;
        List<Location> _locations;

        public List<Location> Locations
        {
            get { return _locations; }
            set { _locations = value; }
        }
        public List<Connection> Connections
        {
            get { return _connections; }
            set { _connections = value; }
        }

        public RouteEngine()
        {
            _connections = new List<Connection>();
            _locations = new List<Location>();
        }



        /// <summary>
        /// Calculates the shortest route to all the other locations
        /// </summary>
        /// <param name="_startLocation"></param>
        /// <returns>List of all locations and their shortest route</returns>
        public Dictionary<Location, Route> CalculateMinCost(Location _startLocation)
        {
            //Initialise a new empty route list
            Dictionary<Location, Route> _shortestPaths = new Dictionary<Location, Route>();            
            //Initialise a new empty handled locations list
            List<Location> _handledLocations = new List<Location>();
            
            //Initialise the new routes. the constructor will set the route weight to in.max
            foreach (Location location in _locations)
            {
                _shortestPaths.Add(location, new Route(location.Identifier));
            }

            //The startPosition has a weight 0. 
            _shortestPaths[_startLocation].Cost = 0;
            

            //If all locations are handled, stop the engine and return the result
            while (_handledLocations.Count != _locations.Count)
            {
                //Order the locations
                List<Location> _shortestLocations = (List < Location > )(from s in _shortestPaths
                                                        orderby s.Value.Cost                                       
                                                        select s.Key).ToList();

                
                Location _locationToProcess = null;

                //Search for the nearest location that isn't handled
                foreach (Location _location in _shortestLocations)
                {
                    if (!_handledLocations.Contains(_location))
                    {
                        //If the cost equals int.max, there are no more possible connections to the remaining locations
                        if (_shortestPaths[_location].Cost == int.MaxValue)
                            return _shortestPaths;
                        _locationToProcess = _location;
                        break;
                    }
                }

                //Select all connections where the startposition is the location to Process
                var _selectedConnections = from c in _connections
                                           where c.A == _locationToProcess
                                           select c;

                //Iterate through all connections and search for a connection which is shorter
                foreach (Connection conn in _selectedConnections)
                {
                    if (_shortestPaths[conn.B].Cost > conn.Weight + _shortestPaths[conn.A].Cost)
                    {
                        _shortestPaths[conn.B].Connections = _shortestPaths[conn.A].Connections.ToList();
                        _shortestPaths[conn.B].Connections.Add(conn);
                        _shortestPaths[conn.B].Cost = conn.Weight + _shortestPaths[conn.A].Cost;
                    }
                }
                //Add the location to the list of processed locations
                _handledLocations.Add(_locationToProcess);
            }


            return _shortestPaths;
        }
    }
}
