using AgeRanger.Web.Application.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using AgeRanger.Web.Models;
using System.Data.SqlClient;
using Dapper;

namespace AgeRanger.Web.Application.DataLayer.Implementations
{
    public class AgeRangerDataSQLServer : IAgeRangerData
    {
        private string _connectionString = null;
        private IAgeRangerDataValidator _validator = null;


        internal AgeRangerDataSQLServer(string ConnectionString, IAgeRangerDataValidator validator)
        {
            _connectionString = ConnectionString;
            _validator = validator;
        }

        public List<PersonItemModel> GetList(int Start, int Length)
        {
            using (SqlConnection conn = new SqlConnection(_connectionString))
            {
                List<PersonItemModel> temp = conn.Query<PersonItemModel>(@"
select 
p.Id,
p.FirstName,
p.LastName,
p.Age,
ag.Description as AgeGroup
from Person p 
join AgeGroup ag on ((ag.MinAge is null or ag.MinAge = '') or ag.MinAge <= p.Age) and ((ag.MaxAge is null or ag.MaxAge = '') or ag.MaxAge > p.Age) 
order by p.Id 
limit @start, @length;",
                    new { start = Start, length = Length }).ToList();

                return temp;
            }
        }

        public int GetTotalCount()
        {
            using (SqlConnection conn = new SqlConnection(_connectionString))
            {
                int temp = conn.Query<int>(@"select count(1) from Person;").FirstOrDefault();

                return temp;
            }
        }


        public List<PersonItemModel> GetListFiltered(int Start, int Length, string Name)
        {
            _validator.ValidateFilterString(Name);

            using (SqlConnection conn = new SqlConnection(_connectionString))
            {
                List<PersonItemModel> temp = conn.Query<PersonItemModel>(@"
select 
p.Id,
p.FirstName,
p.LastName,
p.Age,
ag.Description as AgeGroup
from Person p 
join AgeGroup ag on ((ag.MinAge is null or ag.MinAge = '') or ag.MinAge <= p.Age) and ((ag.MaxAge is null or ag.MaxAge = '') or ag.MaxAge > p.Age) 
where p.FirstName = @name COLLATE NOCASE or p.LastName = @name COLLATE NOCASE
order by p.Id
limit @start, @length;",
                    new { name = Name, start = Start, length = Length }).ToList();

                return temp;
            }
        }

        public int GetTotalCountFiltered(string Name)
        {
            _validator.ValidateFilterString(Name);

            using (SqlConnection conn = new SqlConnection(_connectionString))
            {
                int temp = conn.Query<int>(@"
select count(1) 
from Person p
where p.FirstName = @name COLLATE NOCASE or p.LastName = @name COLLATE NOCASE;",
                    new { name = Name }).FirstOrDefault();

                return temp;
            }
        }


        public bool CheckIfPersonAlreadyExists(PersonItemModel model)
        {
            _validator.ValidateModel(model);

            using (SqlConnection conn = new SqlConnection(_connectionString))
            {
                PersonItemModel temp = conn.Query<PersonItemModel>(@"
select 
p.Id,
p.FirstName,
p.LastName,
p.Age
from Person p 
where p.FirstName = @firstName and p.LastName = @lastName and p.Age = @age limit 1;",
                    new { firstName = model.FirstName, lastName = model.LastName, age = model.Age }).FirstOrDefault();

                return temp != null;
            }
        }


        public bool AddPersonEntry(PersonItemModel model)
        {
            _validator.ValidateModelForAddPerson(model);

            using (SqlConnection conn = new SqlConnection(_connectionString))
            {
                int temp = conn.Execute(@"
insert into Person (FirstName, LastName, Age)
values (@FirstName, @LastName, @Age);"
                    , new { FirstName = model.FirstName, LastName = model.LastName, Age = model.Age });

                if (temp > 0) return true;
                else return false;
            }
        }

        public bool UpdatePersonEntry(PersonItemModel model)
        {
            _validator.ValidateModelForUpdatePerson(model);

            using (SqlConnection conn = new SqlConnection(_connectionString))
            {
                int temp = conn.Execute(@"
update Person 
set 
FirstName = @FirstName,
LastName = @LastName,
Age = @Age
where Id = @Id;"
                    , new { FirstName = model.FirstName, LastName = model.LastName, Age = model.Age, Id = model.Id });

                if (temp > 0) return true;
                else return false;
            }
        }

        public bool DeletePersonEntry(int PersonId)
        {
            _validator.ValidatePersonId(PersonId);

            using (SqlConnection conn = new SqlConnection(_connectionString))
            {
                int temp = conn.Execute(@"delete from Person where Id = @Id;", new { Id = PersonId });

                if (temp > 0) return true;
                else return false;
            }
        }
    }
}