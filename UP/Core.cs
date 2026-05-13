using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UP.Models;


namespace UP
{
    internal class Core
    {
        public static UPEntities Context = new UPEntities();
        public static User CurrentUser {  get; set; }
    }
}

