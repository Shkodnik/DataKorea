using Korea.Migrations;
using Korea.Models.Domain;
using System;

namespace Korea.Models
{
    public class CategoresTree : BaseEntity
    {
        public Guid IdProdram { get; set; }
        public Guid IdTree { get; set; }
        public Category category { get; set; }
	}
}