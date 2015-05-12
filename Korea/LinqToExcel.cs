using LinqToExcel.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Korea.Models.Xls
{
    public class Price
    {
        [ExcelColumn("Код")]
        public string Codes { get; set; }

        [ExcelColumn("Производитель")]
        public string Suppliers { get; set; }

        [ExcelColumn("Наименование")]
        public string Names { get; set; }

        [ExcelColumn("Артикул")]
        public string Article { get; set; }
	}
}