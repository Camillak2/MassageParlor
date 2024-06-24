using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MassageParlor.DB
{
    internal class DBConnection
    {
        public static MassageSalonEntities massageSalon = new MassageSalonEntities();
        
        public static Worker loginedWorker;

        public static Client loginedClient;

        public static Worker selectedForEditWorker;

        public static Client selectedForEditClient;

        public static Service selectedForEditService;

        public static Appeals selectedForEditAppeal;
    }
}
