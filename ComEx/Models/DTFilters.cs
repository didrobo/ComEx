using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ComEx.Models
{
    public class DTFilters
    {
        public string ColumnField { get; set; }
        public string LogicOperator { get; set; }
        public string Value { get; set; }
        public string Operator { get; set; }
        public string OpenParen { get; set; }
        public string CloseParen { get; set; }
    }
}