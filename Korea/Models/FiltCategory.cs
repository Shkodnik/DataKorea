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
    public class FiltCategory : BaseEntity
    {
        [Display(Name = "Товар", Order = 90)]
        [Required]
        [Index]
        public virtual Guid Product { get; set; }

        [Display(Name = "Родительская категория", Order = 100)]
        [Required]
        [Index]
        public virtual Guid CategoryParent { get; set; }

        [Display(Name = "Брэнд", Order = 110)]
        [Required]
        [Index]
        public virtual Guid Brand { get; set; }

        [Display(Name = "Категория", Order = 200)]
        [Required]
        [Index]
        public virtual Guid Category { get; set; }

        [Display(Name = "Модель", Order = 300)]
        [Required]
        [Index]
        public virtual Guid Model { get; set; }

        [Display(Name = "Поколение", Order = 400)]
        [Required]
        [Index]
        public virtual Guid Generation { get; set; }
	}
}