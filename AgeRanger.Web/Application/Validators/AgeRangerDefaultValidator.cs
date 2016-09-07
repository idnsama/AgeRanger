using AgeRanger.Web.Application.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using AgeRanger.Web.Models;
using AgeRanger.Web.Application.Exceptions;

namespace AgeRanger.Web.Application.Validators
{
    public class AgeRangerDefaultValidator : IAgeRangerDataValidator
    {
        public void ValidateFilterString(string name)
        {
            if (string.IsNullOrWhiteSpace(name)) throw new UserFriendlyException("Filter string is empty");
        }

        public void ValidatePersonId(int personId)
        {
            if (personId <= 0) throw new ApplicationException("Person Id was incorrect.");
        }

        public void ValidateModel(PersonItemModel model)
        {
            if (model == null) throw new ApplicationException("Person model is null");

            if (string.IsNullOrWhiteSpace(model.FirstName)) throw new UserFriendlyException("First name cannot be empty");
            if (string.IsNullOrWhiteSpace(model.LastName)) throw new UserFriendlyException("Last name cannot be empty");
            if (!model.Age.HasValue) throw new UserFriendlyException("Age is missing from the input");
            if (model.Age < 0) throw new UserFriendlyException("Age should be zero or positive");
        }

        public void ValidateModelForAddPerson(PersonItemModel model)
        {
            this.ValidateModel(model);

            //Please note that we are checking Id to prevent possible misuse of the WEB API method, as well as unlikely but possible case of data mishandling on front-end.
            if (model.Id.HasValue && model.Id != 0) throw new ApplicationException("Person model Id should not be defined for insert operation.");
        }

        public void ValidateModelForUpdatePerson(PersonItemModel model)
        {
            this.ValidateModel(model);

            if (!model.Id.HasValue) throw new ApplicationException("Person model Id should be defined for insert operation.");
            if (model.Id <= 0) throw new ApplicationException("Person model Id should be positive for update operation.");
        }

    }
}