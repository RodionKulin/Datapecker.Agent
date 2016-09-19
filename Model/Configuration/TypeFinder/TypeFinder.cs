using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Datapecker.Agent
{
    internal abstract class TypeFinder
    {
        //поля
        protected static List<Type> _allTypes;


        //методы
        public abstract Type Find(string typeName);

        public abstract List<Type> FindAllByInterface();

        public static void ClearAppDomainTypesCache()
        {
            _allTypes = null;
        }
    }
}
