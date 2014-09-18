using EventManagerPro.DBLayer.DomainModels;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using EventManagerPro.DBLayer;
using System.Collections.Generic;

namespace Test
{
    
    
    /// <summary>
    ///This is a test class for VenueModelTest and is intended
    ///to contain all VenueModelTest Unit Tests
    ///</summary>
    [TestClass()]
    public class VenueModelTest
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
        ///A test for create, createObj and updateObj
        ///</summary>
        [TestMethod()]
        public void createUpdateObjTest()
        {
            // test for create
            string name = "Lecture Theatre 40";
            int capacity = 500;

            Venue e;
            e = VenueModel.create(name, capacity);

            Venue expected = new Venue();
            expected.Name = name;
            expected.Capacity = capacity;

            Assert.AreEqual(expected.Name, e.Name);
            Assert.AreEqual(expected.Capacity, e.Capacity);

            // test for create obj
            Venue actual;
            actual = VenueModel.createObj(e);

            Assert.AreEqual(expected.Name, actual.Name);
            Assert.AreEqual(expected.Capacity, actual.Capacity);

            // test for update obj
            name = "New Lecture Theatre";
            capacity = 600;

            expected.Name = name;
            expected.Capacity = capacity;

            e.Name = name;
            e.Capacity = capacity;
            actual = VenueModel.updateObj(e);

            Assert.AreEqual(expected.Name, actual.Name);
            Assert.AreEqual(expected.Capacity, actual.Capacity);
        }

        /// <summary>
        ///A test for getByID
        ///</summary>
        [TestMethod()]
        // Method should return the object and not null
        public void getByIDTest()
        {
            int id = 1;

            Venue expected = new Venue();
            expected.Id = id;
            expected.Name = "Lecture Theatre 15";
            expected.Capacity = 100;

            Venue actual;
            actual = VenueModel.getByID(id);

            Assert.AreEqual(expected.Id, actual.Id);
            Assert.AreEqual(expected.Name, actual.Name);
            Assert.AreEqual(expected.Capacity, actual.Capacity);
        }

        [TestMethod()]
        // Object is not in database and method should return null
        public void getByIDTest2()
        {
            int id = 90;

            Venue expected = null;

            Venue actual;
            actual = VenueModel.getByID(id);

            Assert.AreEqual(expected, actual);
        }

    }
}
