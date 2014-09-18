using EventManagerPro.DBLayer.DomainModels;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using EventManagerPro.DBLayer;
using System.Collections.Generic;

namespace Test
{
    
    
    /// <summary>
    ///This is a test class for SubEventModelTest and is intended
    ///to contain all SubEventModelTest Unit Tests
    ///</summary>
    [TestClass()]
    public class SubEventModelTest
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
        ///A test for createObj
        ///</summary>
        [TestMethod()]
        public void createObjTest()
        {
            SubEvent e = new SubEvent(); // TODO: Initialize to an appropriate value
            e.Id = 98;
            e.Name = "Test 3";
            e.EventId = 2;
            e.VenueId = 1;
            e.Start = new DateTime(2012, 4, 20, 7, 0, 0);
            e.End = new DateTime(2012, 4, 20, 8, 0, 0);
            
            SubEvent expected = e; // TODO: Initialize to an appropriate value
            
            SubEvent actual;
            actual = SubEventModel.createObj(e);
            Assert.AreEqual(expected.Name, actual.Name);
            Assert.AreEqual(expected.EventId, actual.EventId);
            Assert.AreEqual(expected.VenueId, actual.VenueId);
        }

        /// <summary>
        ///A test for getByID
        ///</summary>
        [TestMethod()]
        // Method should return the object and not null
        public void getByIDTest()
        {
            int id = 9;

            SubEvent expected = new SubEvent();
            expected.Name = "Test Event";
            expected.EventId = 2;
            expected.VenueId = 1;

            SubEvent actual;
            actual = SubEventModel.getByID(id);

            Assert.AreEqual(expected.Name, actual.Name);
            Assert.AreEqual(expected.EventId, actual.EventId);
            Assert.AreEqual(expected.VenueId, actual.VenueId);
        }

        [TestMethod()]
        // Object is not in database and method should return null
        public void getByIDTest2()
        {
            int id = 99;

            SubEvent expected = null;

            SubEvent actual;
            actual = SubEventModel.getByID(id);

            Assert.AreEqual(expected, actual);
        }

    }
}
