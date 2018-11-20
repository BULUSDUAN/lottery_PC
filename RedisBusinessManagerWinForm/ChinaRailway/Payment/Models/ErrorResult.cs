using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChinaRailway.Payment.Models
{
    public class ErrorResult
    {
        public string Message { get; set; }
        public string StackTrace { get; set; }
    }
}
