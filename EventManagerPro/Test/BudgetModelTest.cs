using System.Reflection;
using EventManagerPro.DBLayer.DomainModels;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using EventManagerPro.DBLayer;
using System.Collections.Generic;

namespace Test
{


    /// <summary>
    ///This is a test class for BudgetModelTest and is intended
    ///to contain all BudgetModelTest Unit Tests
    ///</summary>
    [TestClass()]
    public class BudgetModelTest
    {


        private TestContext testContextInstance;

        /// <summary>
        ///Gets or sets the test context which provides
        ///information about and functionality for the current test run.
        ///</summary>
        public TestContext TestContext
        {
            get
            {
                return testContextInstance;
            }
            set
            {
                testContextInstance = value;
            }
        }

        public bool PropertiesEqual(object object1, object object2)
        {

            Type sourceType = object1.GetType();
            Type destinationType = object2.GetType();

            PropertyInfo[] sourceProperties = sourceType.GetProperties();
            foreach (PropertyInfo pi in sourceProperties)
            {
                if (!(sourceType.GetProperty(pi.Name).GetValue(object1, null).ToString() == destinationType.GetProperty(pi.Name).GetValue(object2, null).ToString()))
                {
                    // only need one property to be different to fail Equals.
                    return false;
                }
            }
            return true;
        }

        #region Additional test attributes
        // 
        //You can use the following additional attributes as you write your tests:
        //
        //Use ClassInitialize to run code before running the first test in the class
        //[ClassInitialize()]
        //public static void MyClassInitialize(TestContext testContext)
        //{
        //}
        //
        //Use ClassCleanup to run code after all tests in a class have run
        //[ClassCleanup()]
        //public static void MyClassCleanup()
        //{
        //}
        //
        //Use TestInitialize to run code before running each test
        //[TestInitialize()]
        //public void MyTestInitialize()
        //{
        //}
        //
        //Use TestCleanup to run code after each test has run
        //[TestCleanup()]
        //public void MyTestCleanup()
        //{
        //}
        //
        #endregion


        /// <summary>
        ///A test for getByID
        ///</summary>
        [TestMethod()]
        public void getByIDTest()
        {
            int id = 2;
            Budget expected = new Budget();
            expected.Id = 2;
            expected.AllocatedBudget = 500;
            expected.BudgetItems.Add(BudgetItemModel.getByBudgetId(2)[0]);
            Budget actual = BudgetModel.getByID(id);
            Assert.IsTrue(this.PropertiesEqual(actual, expected));
        }

        /// <summary>
        ///A test for getAll
        ///</summary>
        [TestMethod()]
        public void getAllTest()
        {
            List<Budget> expected = new List<Budget>();
            Budget b = new Budget();
            b.Id = 2;
            b.AllocatedBudget = 500;
            b.BudgetItems.Add(BudgetItemModel.getByBudgetId(2)[0]);
            expected.Add(b);
            Budget a = new Budget();
            a.Id = 37;
            a.AllocatedBudget = 0;
            a.BudgetItems.Add(BudgetItemModel.getByBudgetId(37)[0]);
            a.BudgetItems.Add(BudgetItemModel.getByBudgetId(37)[1]);
            expected.Add(a);
            Budget c = new Budget();
            c.Id = 43;
            c.AllocatedBudget = 0;
            expected.Add(c);
            List<Budget> actual = BudgetModel.getAll();
            Assert.AreEqual(expected.Count, actual.Count);
            Console.Out.Write("Length is " + actual.Count);
            for (int i = 0; i < actual.Count; i++)
            {
                Console.Out.WriteLine("Actual is " + actual[i].Id);
                Console.Out.WriteLine("Expected is " + expected[i].Id);
                Assert.IsTrue(this.PropertiesEqual(actual[i], expected[i]));
            }
        }

        /// <summary>
        ///A test for updateObj
        ///</summary>
        [TestMethod()]
        public void updateObjTest()
        {
            Budget expected = new Budget();
            expected.Id = 43;
            expected.AllocatedBudget = 30;
            BudgetModel.updateObj(expected);
            Budget actual = BudgetModel.getByID(43);
            Assert.IsTrue(this.PropertiesEqual(actual, expected));

            //revert database back to original state so our getAll() test will pass
            expected.AllocatedBudget = 0;
            BudgetModel.updateObj(expected);
        }

        /// <summary>
        ///A test for update
        ///</summary>
        [TestMethod()]
        public void updateTest()
        {
            int id = 43;
            int allocatedBudget = 30;
            Budget expected = BudgetModel.update(id, allocatedBudget);
            Budget actual = BudgetModel.getByID(43);
            Assert.IsTrue(this.PropertiesEqual(actual, expected));

            //revert back to original state
            allocatedBudget = 0;
            expected = BudgetModel.update(id, allocatedBudget);
        }
    }
}
