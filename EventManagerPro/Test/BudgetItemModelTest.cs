using EventManagerPro.DBLayer.DomainModels;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Reflection;
using EventManagerPro.DBLayer;
using System.Collections.Generic;

namespace Test
{


    /// <summary>
    ///This is a test class for BudgetItemModelTest and is intended
    ///to contain all BudgetItemModelTest Unit Tests
    ///</summary>
    [TestClass()]
    public class BudgetItemModelTest
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

        /// <summary>
        /// Compare property values (as strings)
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public bool PropertiesEqual(object object1, object object2)
        {

            Type sourceType = object1.GetType();
            Type destinationType = object2.GetType();

            if (sourceType == destinationType)
            {
                PropertyInfo[] sourceProperties = sourceType.GetProperties();
                foreach (PropertyInfo pi in sourceProperties)
                {
                    if (!(sourceType.GetProperty(pi.Name).GetValue(object1, null).ToString() == destinationType.GetProperty(pi.Name).GetValue(object2, null).ToString()))
                    {
                        // only need one property to be different to fail Equals.
                        return false;
                    }
                }
            }
            else
            {
                throw new ArgumentException("Comparison object must be of the same type.", "comparisonObject");
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
            int id = 6;
            BudgetItem expected = new BudgetItem();
            expected.Id = 6;
            expected.BudgetId = 2;
            expected.Cost = 432;
            expected.Name = "New Item";
            BudgetItem actual;
            actual = BudgetItemModel.getByID(id);
            Assert.IsTrue(this.PropertiesEqual(actual, expected));
        }

        /// <summary>
        ///A test for getByBudgetId
        ///</summary>
        [TestMethod()]
        public void getByBudgetIdTest()
        {
            int id = 37;
            List<BudgetItem> expected = new List<BudgetItem>();
            BudgetItem c = new BudgetItem();
            c.BudgetId = 37;
            c.Cost = 50;
            c.Id = 22;
            c.Name = "watermelon";
            expected.Add(c);
            BudgetItem b = new BudgetItem();
            b.BudgetId = 37;
            b.Cost = 60;
            b.Id = 23;
            b.Name = "papaya";
            expected.Add(b);

            List<BudgetItem> actual = BudgetItemModel.getByBudgetId(id);
            Assert.AreEqual(expected.Count, actual.Count);
            Console.Out.Write("Length is " + actual.Count);
            for (int i = 0; i < actual.Count; i++)
            {
                Console.Out.WriteLine("Actual is " + actual[i].Name);
                Console.Out.WriteLine("Expected is " + expected[i].Name);
                Assert.IsTrue(this.PropertiesEqual(actual[i], expected[i]));
            }

        }

        /// <summary>
        ///A test for getAll
        ///</summary>
        [TestMethod()]
        public void getAllTest()
        {
            List<BudgetItem> expected = new List<BudgetItem>();

            BudgetItem a = new BudgetItem();
            a.BudgetId = 2;
            a.Cost = 432;
            a.Id = 6;
            a.Name = "New Item";
            expected.Add(a);
            BudgetItem c = new BudgetItem();
            c.BudgetId = 37;
            c.Cost = 50;
            c.Id = 22;
            c.Name = "watermelon";
            expected.Add(c);
            BudgetItem b = new BudgetItem();
            b.BudgetId = 37;
            b.Cost = 60;
            b.Id = 23;
            b.Name = "papaya";
            expected.Add(b);

            List<BudgetItem> actual = BudgetItemModel.getAll();
            Assert.AreEqual(expected.Count, actual.Count);
            Console.Out.WriteLine("Length is " + actual.Count);
            for (int i = 0; i < actual.Count; i++)
            {
                Console.Out.WriteLine("Actual is " + actual[i].Name);
                Console.Out.WriteLine("Expected is " + expected[i].Name);
                Assert.IsTrue(this.PropertiesEqual(actual[i], expected[i]));
            }

        }

        /// <summary>
        ///A test for createObj, UpdateObj and DeleteById
        ///</summary>
        [TestMethod()]
        public void createUpdateDeleteObjTest()
        {
            //test for createObj
            BudgetItem expected = new BudgetItem();
            expected.BudgetId = 2;
            expected.Cost = 10;
            expected.Name = "test";
            BudgetItemModel.createObj(expected);
            int count = BudgetItemModel.getByBudgetId(2).Count;
            BudgetItem actual = BudgetItemModel.getByBudgetId(2)[count - 1];
            Assert.IsTrue(PropertiesEqual(expected, actual));

            //test for updateObj
            expected.Name = "new name";
            BudgetItemModel.updateObj(expected);
            actual = BudgetItemModel.getByBudgetId(2)[count - 1];
            Assert.IsTrue(PropertiesEqual(expected, actual));

            //test for deleteById
            BudgetItemModel.deleteById(actual.Id);
            Assert.AreEqual(1, BudgetItemModel.getByBudgetId(2).Count);
        }

        /// <summary>
        ///A test for create
        ///</summary>
        [TestMethod()]
        public void createTest()
        {
            //test for create
            string name = "mango";
            int cost = 20;
            int budgetId = 43;
            BudgetItem expected = BudgetItemModel.create(name, cost, budgetId);
            BudgetItem actual = BudgetItemModel.getByBudgetId(43)[0];
            Assert.IsTrue(PropertiesEqual(expected, actual));

            //reverting database back to original
            BudgetItemModel.deleteById(actual.Id);

        }

    }
}
