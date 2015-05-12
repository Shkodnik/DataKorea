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
    public class SaveCategoryTree : BaseEntity
    {

        [Display(Name = "Категория в кэше", Order = 100)]
        [Index]
        public virtual Guid IdCach { get; set; }

        [Display(Name = "Категория в программе", Order = 100)]
        [Index]
        public virtual Guid IdProgCat { get; set; }

        [Display(Name = "Родительская категория(Сверка id по программе)", Order = 200)]
        [Index]
        public virtual Guid? IdParent { get; set; }

        [Display(Name = "Родительская категория родительской (для удаления ошибки в последних узлах категории)", Order = 200)]
        [Index]
        public virtual Guid? IdProgParent { get; set; }

        [Display(Name = "Категория на сайте", Order = 300)]
        [Index]
        public virtual int IdSiteCat { get; set; }
	}
}