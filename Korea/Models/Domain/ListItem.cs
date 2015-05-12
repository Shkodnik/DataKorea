using Korea.Models.Domain;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Korea.Models.Domain
{
    public abstract class ListItem :BaseEntity
    {
        [Display(Name = "Название", Order = 100)]
        [Required]
        public virtual string Title { get; set; }

        [Display(Name = "Вес", Order = 200)]
        [DefaultValue(0)]
        public virtual int Weight { get; set; }
    }
}