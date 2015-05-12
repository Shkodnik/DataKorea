using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Korea.Models.Domain
{
    public class YearForImport : ListItem
    {
        public ProductForImport Product { get; set; }


        [ForeignKey("Product")]
        [Display(Name = "Продукт", Order = 150)]
        public Guid ProductId { get; set; }
	}
}