using LinqToExcel.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Korea.Models.Xls
{
    public class ModelXls
    {
        [ExcelColumn("Сокращение")]
        public string Abriviatura { get; set; }

        [ExcelColumn("Марка")]
        public string Brand { get; set; }

        [ExcelColumn("Модель")]
        public string Model { get; set; }
    }
}