using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace HyperStoreService.Models
{
    public class Tag
    {
        public Guid TagId { get; set; }
        public string TagName { get; set; }

        public Tag()
        {
            TagId = Guid.NewGuid();
            TagName = "";
        }

        public List<ProductTag> ProductTags { get; set; }
    }
}