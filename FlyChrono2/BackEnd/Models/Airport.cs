using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FlyChrono2.BackEnd.Models
{
    public class Airport
    {
        public Airport()
        {

        }

        public Airport(double _lat, double _long, string _name)
        {
            latitude = _lat;
            longitude = _long;
            airportName = _name;
        }

        public double latitude { get; set; }
        public double longitude { get; set; }
        public string airportName { get; set; }
    }
}
