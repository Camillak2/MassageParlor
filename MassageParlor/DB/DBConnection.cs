using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MassageParlor.DB
{
    internal class DBConnection
    {
        public static MassageParlorEntities massageParlor = new MassageParlorEntities();
        
        public static Worker loginedWorker;
    }
}
