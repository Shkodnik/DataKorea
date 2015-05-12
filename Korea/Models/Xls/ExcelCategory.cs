using LinqToExcel.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Korea.Models.Xls
{
    public class CategoryXLS
    {
        [ExcelColumn("Наименование")]
        public string Lv3 { get; set; }

        [ExcelColumn("Группа")]
        public string Lv1 { get; set; }
    }
}