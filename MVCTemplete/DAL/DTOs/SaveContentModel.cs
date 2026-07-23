using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MVCTemplete.DAL.DTOs
{
    public class SaveContentModel
    {
        public int Id { get; set; }      // 0 for new
        public string Title { get; set; }
        public string HtmlContent { get; set; }
    }
}