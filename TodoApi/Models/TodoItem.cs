using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TodoApi.Models
{
    public class TodoItem
    {
        //the unique key in a relational database
        public long Id { get; set; }
        public string Name { get; set; }
        public bool IsComplete { get; set; }
        
        //for DTO
        //this field needs to be hidden from this app, but visible for an administrative app if desired
        public string Secret { get; set; }
    }
}
