using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using JP.DataHub.Com.Extensions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using JP.DataHub.UnitTest.Com;

namespace UnitTest.JP.DataHub.Com.Extensions
{
    [TestClass]
    public class UnitTest_ObjectExtensions_FindObjectPath : UnitTestBase
    {
        internal class TestParent
        {
            public string Prop1 { get; set; }
            public int Prop2 { get; set; }
            public TestChild Child { get; set; } = new TestChild();
            public List<TestChild> Children { get; set; } = new List<TestChild>();
            public TestParent()
            {
                Prop1 = Guid.NewGuid().ToString();
                Prop2 = new Random().Next();
            }
        }
        internal class TestChild
        {
            public string ChildProp1 { get; set; }
            public int ChildProp2 { get; set; }
            public TestChild()
            {
                ChildProp1 = Guid.NewGuid().ToString();
                ChildProp2 = new Random().Next();
            }
        }

        [TestInitialize]
        public override void TestInitialize()
        {
            base.TestInitialize();
        }

        private TestParent RandomValToTestClass()
        {
            var result = new TestParent();
            result.Children.Add(new TestChild());
            result.Children.Add(new TestChild());
            result.Children.Add(new TestChild());
            return result;
        }

        [TestMethod]
        public void FindObjectPath()
        {
            var target = RandomValToTestClass();
            target.FindObjectPath("Prop1").Is(target.Prop1);
            target.FindObjectPath("Prop2").Is(target.Prop2);
            target.FindObjectPath("Child.ChildProp1").Is(target.Child.ChildProp1);
            target.FindObjectPath("Child.ChildProp2").Is(target.Child.ChildProp2);
            target.FindObjectPath("Children[0].ChildProp1").Is(target.Children[0].ChildProp1);
            target.FindObjectPath("Children[1].ChildProp2").Is(target.Children[1].ChildProp2);
        }

        [TestMethod]
        public void FindObjectPathList()
        {
            var target = RandomValToTestClass();
            target.FindObjectPathList("Children[*].ChildProp1").Is(target.Children.Select(x => x.ChildProp1).ToList());
            var expect = target.Children.Select(x => x.ChildProp2).ToList();
            var actual = target.FindObjectPathList("Children[*].ChildProp2");
            // FindObjectPathListはobjectで返すためにintに変換してから比較する
            actual.Select(x => (int)x).ToList().Is(expect);
        }
    }
}
