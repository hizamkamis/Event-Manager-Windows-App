using System.Reflection;
using EventManagerPro.DBLayer.DomainModels;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using EventManagerPro.DBLayer;
using System.Collections.Generic;

namespace Test
{


    /// <summary>
    ///This is a test class for EventModelTest and is intended
    ///to contain all EventModelTest Unit Tests
    ///</summary>
    [TestClass()]
    public class EventModelTest
    {


        private TestContext testContextInstance;

        /// <summary>
        ///Gets or sets the test context which provides
        ///information about and functionality for the current test run.
        ///</summary>
        public TestContext TestContext
        {
            get { return testContextInstance; }
            set { testContextInstance = value; }
        }

        public bool PropertiesEqual(object object1, object object2)
        {

            Type sourceType = object1.GetType();
            Type destinationType = object2.GetType();

            PropertyInfo[] sourceProperties = sourceType.GetProperties();
            foreach (PropertyInfo pi in sourceProperties)
            {
                if (
                    !(sourceType.GetProperty(pi.Name).GetValue(object1, null).ToString() ==
                      destinationType.GetProperty(pi.Name).GetValue(object2, null).ToString()) && pi.Name != "Guests" &&
                    pi.Name != "SubEvents")
                {
                    Console.Out.WriteLine(pi.Name);
                    Console.Out.WriteLine((sourceType.GetProperty(pi.Name).GetValue(object1, null).ToString()));
                    Console.Out.WriteLine(destinationType.GetProperty(pi.Name).GetValue(object2, null).ToString());
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
            int id = 20;
            Event expected = new Event();
            expected.Id = id;
            expected.Budget = BudgetModel.getByID(37);
            expected.Capacity = 50;
            expected.Description = "asdasd";
            expected.End = new DateTime(2012, 4, 4, 2, 33, 3);
            expected.Start = new DateTime(2012, 4, 4, 2, 33, 3);
            expected.Guests.Add(StudentModel.getByMatricId("test2"));
            expected.Name = "Hello World";
            expected.Owner = StudentModel.getByMatricId("test");
            expected.StudentMatricId = "test";
            expected.SubEvents = SubEventModel.getAllByEventID(20);
            expected.TimeCreated = new DateTime(2012, 4, 4, 2, 33, 3);
            expected.ViewAtLoginPage = 1;

            Event actual = EventModel.getByID(id);
            Assert.IsTrue(PropertiesEqual(actual, expected));
        }

        /// <summary>
        ///A test for registerGuest.
        ///</summary>
        [TestMethod()]
        public void registerGuestTest()
        {
            string matricId = "test2";
            int eventId = 2;
            bool actual = EventModel.registerGuest(matricId, eventId);
            bool result = false;
            foreach (Student s in EventModel.getByID(2).Guests)
                if (s.MatricId == "test2")
                    result = true;
            Assert.IsTrue(result);
        }

        /// <summary>
        ///A test for registerGuest. Try to register with invalid matricID
        ///</summary>
        [TestMethod()]
        public void registerGuestInvalidMatricTest()
        {
            string matricId = "test100";
            int eventId = 2;
            bool actual = EventModel.registerGuest(matricId, eventId);
            Assert.IsFalse(actual);
        }

        /// <summary>
        ///A test for registerGuest. Try to register with invalid eventID
        ///</summary>
        [TestMethod()]
        public void registerGuestInvalidEventTest()
        {
            string matricId = "test";
            int eventId = 2009;
            bool actual = EventModel.registerGuest(matricId, eventId);
            Assert.IsFalse(actual);
        }

        /// <summary>
        ///A test for unregisterGuest
        ///</summary>
        [TestMethod()]
        public void unregisterGuestTest()
        {
            string matricId = "test2";
            int eventId = 2;
            bool actual = EventModel.unregisterGuest(matricId, eventId);
            bool result = false;
            foreach (Student s in EventModel.getByID(2).Guests)
                if (s.MatricId == "test2")
                    result = true;
            Assert.IsFalse(result);
        }

        /// <summary>
        ///A test for getByOwner
        ///</summary>
        [TestMethod()]
        public void getByOwnerTest()
        {
            string matricId = "test";
            List<Event> expected = new List<Event>();
            expected.Add(EventModel.getByID(26));
            expected.Add(EventModel.getByID(20));
            expected.Add(EventModel.getByID(2));
            List<Event> actual;
            actual = EventModel.getByOwner(matricId);
            Assert.AreEqual(expected.Count, actual.Count);
            for (int i = 0; i < actual.Count; i++)
            {
                Console.Out.WriteLine("Actual is " + actual[i].Name);
                Console.Out.WriteLine("Expected is " + expected[i].Name);
                Assert.IsTrue(this.PropertiesEqual(actual[i], expected[i]));
            }
        }

        /// <summary>
        ///A test for getNotByOwner
        ///</summary>
        [TestMethod()]
        public void getNotByOwnerTest()
        {
            string matricId = "test2"; // TODO: Initialize to an appropriate value
            List<Event> expected = new List<Event>();
            expected.Add(EventModel.getByID(26));
            expected.Add(EventModel.getByID(20));
            expected.Add(EventModel.getByID(2));
            List<Event> actual;
            actual = EventModel.getNotByOwner(matricId);
            Assert.AreEqual(expected.Count, actual.Count);
            for (int i = 0; i < actual.Count; i++)
            {
                Console.Out.WriteLine("Actual is " + actual[i].Name);
                Console.Out.WriteLine("Expected is " + expected[i].Name);
                Assert.IsTrue(this.PropertiesEqual(actual[i], expected[i]));
            }
        }

        /// <summary>
        ///A test for getAllByYearMonth
        ///</summary>
        [TestMethod()]
        public void getAllByYearMonthTest()
        {
            DateTime date = new DateTime(2012, 4, 12);
            List<Event> expected = new List<Event>();
            expected.Add(EventModel.getByID(26));
            expected.Add(EventModel.getByID(20));
            List<Event> actual;
            actual = EventModel.getAllByYearMonth(date);
            Assert.AreEqual(expected.Count, actual.Count);
            for (int i = 0; i < actual.Count; i++)
            {
                Console.Out.WriteLine("Actual is " + actual[i].Name);
                Console.Out.WriteLine("Expected is " + expected[i].Name);
                Assert.IsTrue(this.PropertiesEqual(actual[i], expected[i]));
            }
        }

        /// <summary>
        ///A test for getAllByMonth
        ///</summary>
        [TestMethod()]
        public void getAllByMonthTest()
        {
            int month = 4;
            List<Event> expected = new List<Event>();
            expected.Add(EventModel.getByID(26));
            expected.Add(EventModel.getByID(20));
            List<Event> actual;
            actual = EventModel.getAllByMonth(month);
            Assert.AreEqual(expected.Count, actual.Count);
            for (int i = 0; i < actual.Count; i++)
            {
                Console.Out.WriteLine("Actual is " + actual[i].Name);
                Console.Out.WriteLine("Expected is " + expected[i].Name);
                Assert.IsTrue(this.PropertiesEqual(actual[i], expected[i]));
            }
        }

        /// <summary>
        ///A test for getAllForLoginPage
        ///</summary>
        [TestMethod()]
        public void getAllForLoginPageTest()
        {
            List<Event> expected = new List<Event>();
            expected.Add(EventModel.getByID(26));
            expected.Add(EventModel.getByID(20));
            expected.Add(EventModel.getByID(2));
            List<Event> actual = EventModel.getAllForLoginPage();
            Assert.AreEqual(expected.Count, actual.Count);
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
            List<Event> expected = new List<Event>();
            expected.Add(EventModel.getByID(26));
            expected.Add(EventModel.getByID(20));
            expected.Add(EventModel.getByID(2));
            List<Event> actual = EventModel.getAll();
            Assert.AreEqual(expected.Count, actual.Count);
            for (int i = 0; i < actual.Count; i++)
            {
                Console.Out.WriteLine("Actual is " + actual[i].Name);
                Console.Out.WriteLine("Expected is " + expected[i].Name);
                Assert.IsTrue(this.PropertiesEqual(actual[i], expected[i]));
            }
        }
    }
}
