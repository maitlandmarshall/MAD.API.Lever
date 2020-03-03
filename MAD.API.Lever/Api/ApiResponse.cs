using System;
using System.Collections.Generic;
using System.Text;

namespace MAD.API.Lever.Api
{
    internal class ApiResponse <T>
    {
        public T[] Data { get; set; }

        public bool? HasNext { get; set; }
        public string Next { get; set; }
    }
}
