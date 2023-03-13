using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DbDiver.Core
{
    public interface IDatabaseItemsExtractor
    {
        IEnumerable<string> FindValues(DbSearchParameter parameter);
    }
}
