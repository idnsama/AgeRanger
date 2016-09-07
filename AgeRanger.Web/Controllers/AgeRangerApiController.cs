using AgeRanger.Web.Application.DataLayer;
using AgeRanger.Web.Application.Filters;
using AgeRanger.Web.Models;
using log4net;
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace AgeRanger.Web.Controllers
{
    [ApiErrorHandler]
    public class AgeRangerApiController : ApiController
    {
        private static ILog logger = LogManager.GetLogger(typeof(AgeRangerApiController));

        public List<PersonItemModel> GetFilteredList(int position = 0, int length = 10, string filterString = null)
        {
            if (string.IsNullOrWhiteSpace(filterString))
                return AgeRangerDataFactory.Instance.GetList(position, length);
            else
                return AgeRangerDataFactory.Instance.GetListFiltered(position, length, filterString);
        }

        public int GetTotalCount(string filterString = null)
        {
            if (string.IsNullOrWhiteSpace(filterString))
                return AgeRangerDataFactory.Instance.GetTotalCount();
            else
                return AgeRangerDataFactory.Instance.GetTotalCountFiltered(filterString);
        }

        public bool PostAddPerson(PersonItemModel model)
        {
            if(AgeRangerDataFactory.Instance.CheckIfPersonAlreadyExists(model))
            {
                var resp = new HttpResponseMessage()
                {
                    StatusCode = HttpStatusCode.Conflict,
                    Content = new StringContent("Person with selected properties already exists in the application database."),
                    ReasonPhrase = "Person already exists"
                };
                throw new HttpResponseException(resp);
            }

            return AgeRangerDataFactory.Instance.AddPersonEntry(model);
        }

        public bool PostUpdatePerson(PersonItemModel model)
        {
            return AgeRangerDataFactory.Instance.UpdatePersonEntry(model);
        }

        public bool GetDeletePerson(int? Id = null)
        {
            if (Id == null) throw new ApplicationException("GetDeletePerson: null Id received");

            logger.Info(string.Format("Got request to delete Person row '{0}'", Id.Value));

            return AgeRangerDataFactory.Instance.DeletePersonEntry(Id.Value);
        }


    }
}
