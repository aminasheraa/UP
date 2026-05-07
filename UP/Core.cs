using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UP
{
    internal class Core
    {
        public static UPEntities Context = new UPEntities();
        public static User CurrentUser {  get; set; }
    }
}

