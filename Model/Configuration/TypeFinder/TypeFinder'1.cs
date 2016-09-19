using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Datapecker.Agent
{
    internal class TypeFinder<TInterface> : TypeFinder
    {
        //поля
        protected List<Type> _implementingTypes;


        //методы
        public override Type Find(string typeName)
        {
            if (_implementingTypes == null)
            {
                _implementingTypes = FindAllByInterface();
            }

            return _implementingTypes.FirstOrDefault(p => p.Name == typeName);
        }

        public override List<Type> FindAllByInterface()
        {
            if (_allTypes == null)
            {
                _allTypes = AppDomain.CurrentDomain.GetAssemblies()
                    .SelectMany(s => s.GetTypes())
                    .ToList();
            }

            Type interfaceType = typeof(TInterface);

            List<Type> list = _allTypes
                .Where(mytype => mytype.GetInterfaces().Contains(interfaceType))
                .ToList();

            return list;
        }

    }
}
