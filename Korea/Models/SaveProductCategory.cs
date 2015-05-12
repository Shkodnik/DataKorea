using Korea.Models.Domain;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Korea.Models
{
    [Serializable]
    public class SaveProductCategory : BaseEntity
    {
        [Display(Name = "Продукт", Order = 100)]
        [Index]
        public virtual int ProductId { get; set; }

        [Display(Name = "Категория", Order = 110)]
        [Index]
        public virtual int CategoryId { get; set; }

        [Display(Name = "Число для сортировки", Order = 100)]
        [Index]
        public virtual int SortOrder { get; set; }

        [Display(Name = "", Order = 100)]
        [Index]
        public bool Main { get; set; }
	}
}