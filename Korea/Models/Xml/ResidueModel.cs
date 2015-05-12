using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Korea.Models.Xml
{
    public class ResidueModel
    {
        public virtual string Article { get; set; }

        public virtual int Number { get; set; }

        public virtual double Cost { get; set; }
	}
}