using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Datapecker.Agent
{
    internal class DetailsIDProvider
    {
        private int _currentID;

        public int TotalItemsAdded
        {
            get
            {
                return _currentID;
            }
        }

        public int GetNext()
        {
            _currentID++;
            return _currentID;
        }
    }
}
