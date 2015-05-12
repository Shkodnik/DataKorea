using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace Korea.Models.Domain
{
    [Serializable]
    public class BaseEntity
    {
        
        [Display(Name = "Индификационный номер", Order = 0)]
        public virtual Guid Id { get; set; }
    }
}