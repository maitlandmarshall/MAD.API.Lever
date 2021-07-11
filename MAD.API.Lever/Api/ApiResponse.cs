using System;
using System.Collections.Generic;
using System.Text;

namespace MAD.API.Lever.Api
{
    public class ApiResponse <T>
    {
        public T[] Data { get; set; }

        public bool? HasNext { get; set; }
        public string Next { get; set; }
    }
}
