using System.Collections.Generic;
using AgeRanger.Web.Models;

namespace AgeRanger.Web.Application.Interfaces
{
    public interface IAgeRangerData
    {
        bool AddPersonEntry(PersonItemModel Model);
        bool DeletePersonEntry(int PersonId);
        List<PersonItemModel> GetList(int Position, int Length);
        //PersonItemModel GetPersonByID(int personId);
        List<PersonItemModel> GetListFiltered(int Start, int Length, string Name);
        bool UpdatePersonEntry(PersonItemModel Model);
        int GetTotalCount();
        int GetTotalCountFiltered(string Name);
        bool CheckIfPersonAlreadyExists(PersonItemModel model);
    }
}