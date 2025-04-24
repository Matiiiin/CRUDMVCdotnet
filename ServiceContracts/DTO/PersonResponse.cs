using Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ServiceContracts.Enums;

namespace ServiceContracts.DTO
{
    public class PersonResponse
    {
        public Guid? PersonID { get; set; }
        public string? PersonName { get; set; }
        public string? Email { get; set; }
        public DateTime? DateOfBirth { get; set; }
        public string? Gender { get; set; }
        public Guid? CountryID { get; set; }
        public string? Country { get; set; }
        public string? Address { get; set; }
        public bool RecievesNewsLetters { get; set; }
        public int? Age { get; set; }

        public override bool Equals(object? obj)
        {
            if (obj == null)
            {
                return false;
            }
            if (obj.GetType() != typeof(PersonResponse))
            {
                return false;
            }
            var otherPersonResponse = obj as PersonResponse;
            return
                PersonID == otherPersonResponse.PersonID &&
                PersonName == otherPersonResponse.PersonName &&
                Email == otherPersonResponse.Email &&
                DateOfBirth == otherPersonResponse.DateOfBirth &&
                CountryID == otherPersonResponse.CountryID &&
                Gender == otherPersonResponse.Gender &&
                Country == otherPersonResponse.Country &&
                Address == otherPersonResponse.Address &&
                RecievesNewsLetters == otherPersonResponse.RecievesNewsLetters &&
                Age == otherPersonResponse.Age;
        }

        public override string ToString()
        {
            return $"PersonID: {PersonID} , PersonName: {PersonName}";
        }

        public PersonUpdateRequest ToPersonUpdateRequest()
        {
            return new PersonUpdateRequest()
            {
                PersonID = PersonID,
                PersonName = PersonName,
                Email = Email,
                DateOfBirth = DateOfBirth,
                Gender = (PersonGenderEnum)Enum.Parse(typeof(PersonGenderEnum), Gender , true),
                CountryID = CountryID,
                Address = Address,
                RecievesNewsLetters = RecievesNewsLetters,
            };
        }
    }
        
    public static class PersonExtensions
    {
        public static PersonResponse ToPersonResponse(this Person person)
        {
            return new PersonResponse()
            {
                PersonID = person.PersonID,
                PersonName = person.PersonName,
                Email = person.Email,
                DateOfBirth = person.DateOfBirth,
                Gender = person.Gender,
                CountryID = person.CountryID,
                Address = person.Address,
                RecievesNewsLetters = person.RecievesNewsLetters,
                Country = person.Country?.CountryName,
                Age = (person.DateOfBirth != null) ? (int)Math.Round((DateTime.Now - person.DateOfBirth.Value).TotalDays / 365.25) : null

            };
        }
    }
}
