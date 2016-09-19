using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Datapecker.Agent
{
    internal class TypeFinder<TInterface, TAttribute> : TypeFinder<TInterface>
        where TAttribute : StateProviderAttribute
    {
        //методы
        public override Type Find(string typeName)
        {
            if (_implementingTypes == null)
            {
                _implementingTypes = FindAllByInterface();
            }

            return FindByAttribute(typeName) ?? FindByFullName(typeName);
        }

        private Type FindByAttribute(string typeName)
        {
            Type attributeType = typeof(TAttribute);

            foreach (Type implementingType in _implementingTypes)
            {
                TAttribute[] attributes = (TAttribute[])implementingType
                    .GetCustomAttributes(attributeType, true);

                if (attributes.Any(p => p.Name == typeName))
                {
                    return implementingType;
                }
            }

            return null;
        }

        private Type FindByFullName(string typeName)
        {
            return _implementingTypes.FirstOrDefault(p => p.FullName == typeName);
        }
    }
}
