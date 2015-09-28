using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Profiler2
{
    public interface IScope
    {
        void Start();
        void Stop();
    }
}
