using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Profiler2
{
    public class Scope : IScope
    {
        public int idScope;
        public string name;
        public List<DateTime> beginTime;
        public List<DateTime> endTime;
        public IScope papa;
        private object lockItem;

        public Scope(int idScope, string name, int idPapa)
        {
            this.idScope = idScope;
            this.name = name;
            this.beginTime = new List<DateTime>();
            this.endTime = new List<DateTime>();
            this.papa = Profiler.Get(idPapa);
            this.lockItem = new object();
        }

        public void Start()
        {
            lock (lockItem)
            {
                if (beginTime.Count - endTime.Count == 0)
                {
                    DateTime rightNow = DateTime.Now;
                    beginTime.Add(rightNow);

                    Profiler.order += this.idScope + ";";
                }
            }
        }

        public void Stop()
        {
            lock (lockItem)
            {
                if (beginTime.Count - endTime.Count == 1)
                {
                    DateTime rightNow = DateTime.Now;
                    endTime.Add(rightNow);

                    Profiler.order += this.idScope + ";";
                }
            }
        }
    }
}
