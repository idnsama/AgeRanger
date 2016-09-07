using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using AgeRanger.Web.Models;

namespace AgeRanger.Web.Application.Interfaces
{
    public interface IAgeRangerDataValidator
    {
        void ValidateFilterString(string name);
        void ValidatePersonId(int personId);
        void ValidateModelForUpdatePerson(PersonItemModel model);
        void ValidateModelForAddPerson(PersonItemModel model);
        void ValidateModel(PersonItemModel model);
    }
}