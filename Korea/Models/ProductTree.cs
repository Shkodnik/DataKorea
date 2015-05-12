using Korea.Migrations;
using Korea.Models.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Korea.Models
{
    public class ProductTree : BaseEntity
    {
        public Guid IdProdram { get; set; }
        public Product product { get; set; }
	}
}