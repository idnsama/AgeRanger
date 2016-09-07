using AgeRanger.Web.Application.DataLayer;
using AgeRanger.Web.Models;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Http;

namespace AgeRanger.Web.Controllers.Tests
{
    [TestClass()]
    public class AgeRangerApiControllerTests
    {
        public TestContext TestContext { get; set; }

        private static int _numberOfTestRecords = 10;
        private static Random _randomGenerator = new Random(DateTime.Today.Millisecond);
        private static List<PersonItemModel> _testDataList;
        private static int _totalCountBeforeTest = 0;


        [ClassInitialize]
        public static void SetUp(TestContext context)
        {
            _totalCountBeforeTest = AgeRangerDataFactory.Instance.GetTotalCount();

            // Setup database for testing. Add a bunch of unique records.
            _testDataList = new List<PersonItemModel>();

            for (int i = 0; i < _numberOfTestRecords; i++)
            {
                PersonItemModel model = GenerateUniquePerson();
                _testDataList.Add(model);

                AgeRangerDataFactory.Instance.AddPersonEntry(model);
            }

            int currentCount = AgeRangerDataFactory.Instance.GetTotalCount();
            if ((_totalCountBeforeTest + _numberOfTestRecords) != currentCount)
                Assert.Fail("Insert count mismatch");
        }

        private static PersonItemModel GenerateUniquePerson()
        {
            return new PersonItemModel()
            {
                FirstName = Guid.NewGuid().ToString(),
                LastName = Guid.NewGuid().ToString(),
                Age = _randomGenerator.Next(0, 10000)
            };
        }


        [TestMethod()]
        public void GetFilteredListTest()
        {
            AgeRangerApiController test = new AgeRangerApiController();

            //First retrieval with default values.
            var list = test.GetFilteredList();
            if (list.Count < 1) Assert.Fail("Unfiltered list retrieval error");

            var temp = list.Where(r => string.IsNullOrWhiteSpace(r.AgeGroup));
            if (temp.Count() > 0) Assert.Fail("Empty AgeGroup field found");

            var listFiltered = test.GetFilteredList(0, 10, _testDataList[0].FirstName);
            if (listFiltered.Count != 1) Assert.Fail("Filter error. One record expected.");

            var listFiltered2 = test.GetFilteredList(0, 10, _testDataList[1].LastName);
            if (listFiltered2.Count != 1) Assert.Fail("Filter error. One record expected.");
        }

        [TestMethod()]
        public void GetTotalCountTest()
        {
            AgeRangerApiController test = new AgeRangerApiController();

            int currentCount = test.GetTotalCount();
            if ((_totalCountBeforeTest + _numberOfTestRecords) != currentCount)
                Assert.Fail("Total count mismatch");

            int currentCountFiltered = test.GetTotalCount(_testDataList[1].FirstName);
            if (currentCountFiltered != 1) Assert.Fail("Filtered count error");
        }

        [TestMethod()]
        public void PostAddPersonTest()
        {
            AgeRangerApiController test = new AgeRangerApiController();
            int countBeforeTest = test.GetTotalCount();

            //Test duplicate check. This call should fail because person record should already be there.
            try
            {
                test.PostAddPerson(_testDataList[2]);
                Assert.Fail("PostAddPerson allowed duplicate.");
            }
            catch (HttpResponseException ex)
            {
                //Expected
            }


            //Try and add unique person.
            PersonItemModel model = GenerateUniquePerson();

            if (!test.PostAddPerson(model)) Assert.Fail("PostAddPerson returned false on adding unique record.");

            int countAfterAdding = test.GetTotalCount();
            if (countBeforeTest + 1 != countAfterAdding) Assert.Fail("wrong total count after adding.");


            //Test adding a model with defined id.
            PersonItemModel model2 = GenerateUniquePerson();
            model2.Id = 1;
            try
            {
                if (test.PostAddPerson(model2)) Assert.Fail("PostAddPerson added person from model with defined Id.");
            }
            catch (ApplicationException ex2)
            {
                //Expected.
            }

        }

        [TestMethod()]
        public void PostUpdatePersonTest()
        {
            AgeRangerApiController test = new AgeRangerApiController();
            int countBeforeTest = test.GetTotalCount();

            PersonItemModel referenceModel = _testDataList[0];

            //Get known person from DB to obtain Id.
            List<PersonItemModel> list = AgeRangerDataFactory.Instance.GetListFiltered(0, 10, referenceModel.FirstName);
            if (list.Count != 1) Assert.Fail("Single record was expected");

            //This one should have Id defined.
            PersonItemModel model = list[0];
            model.FirstName += "zzzzzzzz";
            model.LastName += "xxxxxxxx";
            model.Age += 1;
            if (!test.PostUpdatePerson(model)) Assert.Fail("PostUpdatePerson returned false on updating a record.");

            //Get updated record.
            List<PersonItemModel> listUpdated = AgeRangerDataFactory.Instance.GetListFiltered(0, 10, model.FirstName);
            if (listUpdated.Count != 1) Assert.Fail("Single record was expected");
            PersonItemModel modelAfterUpdate = listUpdated[0];

            if (modelAfterUpdate.Id != model.Id) Assert.Fail("Id mismatch after update.");
            if (modelAfterUpdate.FirstName != model.FirstName) Assert.Fail("Id mismatch after update.");
            if (modelAfterUpdate.LastName != model.LastName) Assert.Fail("Id mismatch after update.");
            if (modelAfterUpdate.Age != model.Age) Assert.Fail("Id mismatch after update.");


            //Test updating a model with undefined id.
            PersonItemModel model2 = GenerateUniquePerson();
            try
            {
                if (test.PostUpdatePerson(model2)) Assert.Fail("PostUpdatePerson updated record with undefined Id.");
            }
            catch (ApplicationException ex2)
            {
                //Expected.
            }

            int countAfterTest = test.GetTotalCount();
            if (countBeforeTest != countAfterTest) Assert.Fail("total count changed after updating.");
        }

        [TestMethod()]
        public void GetDeletePersonTest()
        {
            AgeRangerApiController test = new AgeRangerApiController();
            int countBeforeTest = test.GetTotalCount();


            PersonItemModel referenceModel = _testDataList[3];

            //Get known person from DB to obtain Id.
            List<PersonItemModel> list = AgeRangerDataFactory.Instance.GetListFiltered(0, 10, referenceModel.FirstName);
            if (list.Count != 1) Assert.Fail("Single record was expected");

            if (!test.GetDeletePerson(list[0].Id)) Assert.Fail("GetDeletePerson returned false.");

            //Get known person from DB to obtain Id.
            List<PersonItemModel> list2 = AgeRangerDataFactory.Instance.GetListFiltered(0, 10, referenceModel.FirstName);
            if (list2.Count != 0) Assert.Fail("No records were expected");


            int countAfterTest = test.GetTotalCount();
            if (countBeforeTest - 1 != countAfterTest) Assert.Fail("Unexpected totalcount difference after deleting.");
        }
    }
}