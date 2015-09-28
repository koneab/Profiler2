using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Threading;

namespace UnitTestProfiler2
{
    [TestClass]
    public class UnitTestProfiler
    {
        [TestMethod]
        public void BasicTest()
        {
            Profiler2.Profiler.Start("test");
            Profiler2.Profiler.Close();

            Profiler2.IScope s = Profiler2.Profiler.Get("test");
            Assert.IsNotNull(s);
        }

        [TestMethod]
        public void BasicFail()
        {
            Profiler2.IScope s = Profiler2.Profiler.Get("fail");
            Assert.IsNull(s);
        }

        [TestMethod]
        public void OneOpen()
        {
            Profiler2.Profiler.Start("OneOpen");
            Profiler2.Profiler.Close();

            Profiler2.Scope s = (Profiler2.Scope)Profiler2.Profiler.Get("OneOpen");
            Assert.IsTrue(s.beginTime.Count == 1);
        }

        [TestMethod]
        public void OneClose()
        {
            Profiler2.Profiler.Start("OneClose");
            Profiler2.Profiler.Close();

            Profiler2.Scope s = (Profiler2.Scope)Profiler2.Profiler.Get("OneClose");
            Assert.IsTrue(s.endTime.Count == 1);
        }

        [TestMethod]
        public void GotParents()
        {
            Profiler2.Profiler.Start("Father");
            Profiler2.Profiler.Start("Son");
            Profiler2.Profiler.Close();
            Profiler2.Scope s = (Profiler2.Scope)Profiler2.Profiler.Get("Son");
            Profiler2.Profiler.Close();

            Assert.IsNotNull(s.papa);
        }

        [TestMethod]
        public void DontGotParents()
        {
            Profiler2.Profiler.Start("Orphee");
            Profiler2.Profiler.Close();

            Profiler2.Scope s = (Profiler2.Scope)Profiler2.Profiler.Get("Orphee");
            Assert.IsNull(s.papa);
        }


        [TestMethod]
        public void OpenButNotClose()
        {
            Profiler2.Profiler.Start("NotClose");
            Profiler2.Profiler.Close("NotClose");
            Profiler2.Scope s = (Profiler2.Scope)Profiler2.Profiler.Get("NotClose");
            Profiler2.Profiler.Start("NotClose");

            Assert.IsTrue(s.endTime.Count == 1 && s.beginTime.Count == 2);
        }

        [TestMethod]
        public void QuickScope()
        {
            Profiler2.Profiler.Start("Quick");
            Thread.Sleep(100);
            Profiler2.Profiler.Close();

            Profiler2.Scope s = (Profiler2.Scope)Profiler2.Profiler.Get("Quick");
            TimeSpan elapsedTime = s.endTime[0] - s.beginTime[0];
            int ms = (int)elapsedTime.TotalMilliseconds;

            Assert.IsTrue(ms < 110);
        }

        [TestMethod]
        public void LongScope()
        {
            Profiler2.Profiler.Start("Long");
            Thread.Sleep(500);
            Profiler2.Profiler.Close();

            Profiler2.Scope s = (Profiler2.Scope)Profiler2.Profiler.Get("Long");
            TimeSpan elapsedTime = s.endTime[0] - s.beginTime[0];
            int ms = (int)elapsedTime.TotalMilliseconds;

            Assert.IsTrue(ms > 490);
        }

        [TestMethod]
        public void DoubleChrono()
        {
            Profiler2.Profiler.Start("Double");
            Thread.Sleep(50);
            Profiler2.Profiler.Close();

            Profiler2.Profiler.Start("Double");
            Thread.Sleep(250);
            Profiler2.Profiler.Close();

            Profiler2.Scope s = (Profiler2.Scope)Profiler2.Profiler.Get("Double");
            TimeSpan elapsedTime = s.endTime[0] - s.beginTime[0];
            int ms = (int)elapsedTime.TotalMilliseconds;
            elapsedTime = s.endTime[1] - s.beginTime[1];
            ms += (int)elapsedTime.TotalMilliseconds;

            Assert.IsTrue(ms > 290);
        }

        [TestMethod]
        public void SameSon()
        {
            Profiler2.Profiler.Start("BigFather");
            Profiler2.Profiler.Start("SameSon");
            Profiler2.Profiler.Close();
            Profiler2.Scope s1 = (Profiler2.Scope)Profiler2.Profiler.Get("SameSon");
            Profiler2.Profiler.Close();

            Profiler2.Profiler.Start("BigFather");
            Profiler2.Profiler.Start("SameSon");
            Profiler2.Profiler.Close();
            Profiler2.Scope s2 = (Profiler2.Scope)Profiler2.Profiler.Get("SameSon");
            Profiler2.Profiler.Close();

            Assert.AreEqual(s1, s2);
        }

        [TestMethod]
        public void DifferentSon()
        {
            Profiler2.Profiler.Start("BigFather");
            Profiler2.Profiler.Start("SameSon");
            Profiler2.Profiler.Close();
            Profiler2.Scope s1 = (Profiler2.Scope)Profiler2.Profiler.Get("SameSon");
            Profiler2.Profiler.Close();

            Profiler2.Profiler.Start("SameSon");
            Profiler2.Profiler.Close();
            Profiler2.Scope s2 = (Profiler2.Scope)Profiler2.Profiler.Get("SameSon");

            Assert.AreNotEqual(s1, s2);
        }

        [TestMethod]
        public void TwoInTheSameScope()
        {
            Profiler2.Profiler.Start("TwoSons");
            Profiler2.Profiler.Start("One");
            Profiler2.Profiler.Close();
            Profiler2.Profiler.Start("Two");
            Profiler2.Profiler.Close();
            Profiler2.Profiler.Close();

            int idScope = ((Profiler2.Scope)Profiler2.Profiler.Get("TwoSons")).idScope;

            Assert.IsTrue(Profiler2.Profiler.allId[idScope].Count == 2);
        }

        [TestMethod]
        public void SameNameButDifferentScope()
        {
            Profiler2.Profiler.Start("NotTwoSons");
            Profiler2.Profiler.Start("One");
            Profiler2.Profiler.Close();
            Profiler2.Profiler.Close();

            Profiler2.Profiler.Start("FakeParent");
            Profiler2.Profiler.Start("NotTwoSons");
            Profiler2.Profiler.Start("One");
            Profiler2.Profiler.Close();
            Profiler2.Profiler.Close();
            Profiler2.Profiler.Close();

            int idScope = ((Profiler2.Scope)Profiler2.Profiler.Get("NotTwoSons")).idScope;

            Assert.IsFalse(Profiler2.Profiler.allId[idScope].Count == 2);
        }

        [TestMethod]
        public void ExportCSV()
        {
            try
            {
                Profiler2.Profiler.ExportCsv();
                Assert.IsTrue(true);
            }
            catch (Exception e)
            {
                Assert.Fail("Error : " + e.Message);
            }
        }
    }
}
