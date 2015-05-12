using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Korea.Models.Domain
{
    public class AnalogsForImport : BaseEntity
    {
        [MaxLength(16)]
        [Index]
        [Required]
        [Display(Name = "Код", Order = 105)]
        public virtual string Сode1 { get; set; }

        [MaxLength(16)]
        [Index]
        [Required]
        [Display(Name = "Код", Order = 105)]
        public virtual string Сode2 { get; set; }
	}
}