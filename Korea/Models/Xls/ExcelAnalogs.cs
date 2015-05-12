using LinqToExcel.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Korea.Models.Xls
{
    public class Analogs
    {
        [ExcelColumn("Код")]
        public string Id1 { get; set; }

        [ExcelColumn("Код аналога")]
        public string Id2 { get; set; }
    }
}