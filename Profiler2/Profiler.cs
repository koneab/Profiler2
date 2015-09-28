using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Profiler2
{
    public static class Profiler
    {
        private static int idScope;
        public static Dictionary<int, Dictionary<string, IScope>> allId;
        public static string order;
        private static object itemLock;

        static Profiler()
        {
            allId = new Dictionary<int, Dictionary<string, IScope>>();
            order = "";
            idScope = 0;
            itemLock = new object();
        }

        private static IScope addScope(string name)
        {
            lock (itemLock)
            {
                allId[getCurrentContext()][name] = new Scope(idScope, name, getCurrentContext());
                idScope += 1;

                return allId[getCurrentContext()][name];
            }
        }

        public static IScope Get(string name)
        {
            Dictionary<string, IScope> allScope = allId[getCurrentContext()];
            if (allScope.ContainsKey(name))
                return allScope[name];

            return null;
        }

        public static IScope Get(int id)
        {
            foreach (var anId in allId.Keys)
            {
                foreach (var aGroupScope in allId[anId])
                {
                    int currentId = ((Scope)aGroupScope.Value).idScope;
                    if (currentId == id)
                        return aGroupScope.Value;
                }
            }

            return null;
        }

        public static IScope Start(string name)
        {
            if (!allId.ContainsKey(getCurrentContext()))
                allId[getCurrentContext()] = new Dictionary<string, IScope>();

            Dictionary<string, IScope> allScope = allId[getCurrentContext()];

            if (!allScope.ContainsKey(name))
                addScope(name);
            allScope[name].Start();

            return allScope[name];
        }

        public static void Close(IScope scope)
        {
            scope.Stop();
        }

        public static void Close(string name)
        {
            int idToClose = getCurrentContext();
            if (idToClose != -1)
                Get(idToClose).Stop();
        }

        private static int getCurrentContext()
        {
            int i = 0;
            List<string> elem = order.Split(';').ToList();
            if (elem.Count > 0)
            {
                elem.RemoveAt(elem.Count - 1);
            }

            while (i < elem.Count - 1)
            {
                if (elem[i] == elem[i + 1])
                {
                    elem.RemoveAt(i);
                    elem.RemoveAt(i);
                    i = 0;
                }
                else
                    i += 1;
            }

            if (elem.Count > 0)
            {
                int id = int.Parse(elem[elem.Count - 1]);
                return id;
            }
            else
                return -1;
        }

        public static void Close()
        {
            int idToClose = getCurrentContext();
            if (idToClose != -1)
                Get(idToClose).Stop();
        }

        public static void ExportCsv()
        {
            List<string> elem = order.Split(';').ToList();
            List<string> temp = new List<string>();
            List<string> lines = new List<string>();
            int i = 0;
            List<string> idDone = new List<string>();

            lines.Add("IdScope;IdParentScope;TimeFromParent(ms);Name;Time(ms);");

            while (elem[i] != "")
            {
                if (!temp.Contains(elem[i]))
                {
                    temp.Add(elem[i]);
                    if (!idDone.Contains(elem[i]))
                    {
                        lines.Add(Get(int.Parse(elem[i])).ToString());
                        idDone.Add(elem[i]);
                    }
                }
                else
                {
                    temp.Remove(elem[i]);
                }
                i += 1;
            }

            System.IO.File.WriteAllLines(@"export.csv", lines);
        }
    }
}   
