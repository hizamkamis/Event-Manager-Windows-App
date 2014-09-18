using EventManagerPro.DBLayer.DomainModels;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using EventManagerPro.DBLayer;
using System.Collections.Generic;

namespace Test
{
    
    
    /// <summary>
    ///This is a test class for StudentModelTest and is intended
    ///to contain all StudentModelTest Unit Tests
    ///</summary>
    [TestClass()]
    public class StudentModelTest
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
        ///A test for authenticate
        ///</summary>
        [TestMethod()]
        // authenticate with correct matricId and password
        public void authenticateTest()
        {
            string matricId = "test";
            string password = "test";

            bool expected = true;
            bool actual;
            actual = StudentModel.authenticate(matricId, password);

            Assert.AreEqual(expected, actual);
        }

        [TestMethod()]
        // authenticate with correct matricId but wrong password
        public void authenticateTest2()
        {
            string matricId = "test";
            string password = "test123";

            bool expected = false;
            bool actual;
            actual = StudentModel.authenticate(matricId, password);

            Assert.AreEqual(expected, actual);
        }

        [TestMethod()]
        // authenticate with wrong matricId but correct password
        public void authenticateTest3()
        {
            string matricId = "test123";
            string password = "test";

            bool expected = false;
            bool actual;
            actual = StudentModel.authenticate(matricId, password);

            Assert.AreEqual(expected, actual);
        }

        /// <summary>
        ///A test for create
        ///</summary>
        [TestMethod()]
        public void createTest()
        {
            string matricId = "test3";
            string password = "test3";
            string name = "test3";

            Student expected = new Student();
            expected.Name = name;
            expected.Password = password;
            expected.MatricId = matricId;

            Student actual;
            actual = StudentModel.create(matricId, password, name);

            Assert.AreEqual(expected.Name, actual.Name);
            Assert.AreEqual(expected.Password, actual.Password);
            Assert.AreEqual(expected.MatricId, actual.MatricId);
        }

        /// <summary>
        ///A test for getAll
        ///</summary>
        [TestMethod()]
        public void getAllTest()
        {
            List<string> matricId = new List<string>();
            matricId.Add("test");
            matricId.Add("test2");
            matricId.Add("test3");

            Dictionary<string, Student> expected = new Dictionary<string, Student>();

            for (int i = 0; i < matricId.Count; i++)
            {
                Student temp = new Student();
                temp = StudentModel.getByMatricId(matricId[i]);
                expected.Add(matricId[i], temp);
            }

            Dictionary<string, Student> actual;
            actual = StudentModel.getAll();

            Assert.AreEqual(expected.Count, actual.Count);
            bool isEqual = true;
            foreach (KeyValuePair<string, Student> pair in expected)
            {
                isEqual = actual.ContainsKey(pair.Key);
            }
            Assert.IsTrue(isEqual);
        }

        /// <summary>
        ///A test for getByMatricId
        ///</summary>
        [TestMethod()]
        // Method should return the object or null
        public void getByMatricIdTest()
        {
            string matricId = "test";

            Student expected = new Student();
            expected.MatricId = matricId;
            expected.Password = "test";
            expected.Name = "test";

            Student actual;
            actual = StudentModel.getByMatricId(matricId);

            Assert.AreEqual(expected.MatricId, actual.MatricId);
            Assert.AreEqual(expected.Password, actual.Password);
            Assert.AreEqual(expected.Name, actual.Name);
        }

        [TestMethod()]
        // Object is not in database and method should return null
        public void getByMatricIdTest2()
        {
            string matricId = "test123";
            Student expected = null;

            Student actual;
            actual = StudentModel.getByMatricId(matricId);

            Assert.AreEqual(expected, actual);
        }
    }
}
