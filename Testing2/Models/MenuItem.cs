using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Testing2.Models
{
    public class MenuItem
    {
        public string LinkText { get; set; }
        public string ActionName { get; set; }
        public string ControllerName { get; set; }
    }

    public class Menu
    {
        public List<MenuItem> Items { get; set; }
    }
}
