using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MVCTemplete.DAL.DTOs
{
    public class PaginationDto
    {
        public int CurrentPage { get; set; }

        public int PageSize { get; set; }

        public int TotalRecords { get; set; }

        public int TotalPages
        {
            get
            {
                return (int)Math.Ceiling((double)TotalRecords / PageSize);
            }
        }
    }
}