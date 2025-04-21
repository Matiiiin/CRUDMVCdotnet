using System.ComponentModel.DataAnnotations;
using Entities;
using ServiceContracts.Enums;


namespace ServiceContracts.DTO;

public class PersonUpdateRequest
{
    [Required(ErrorMessage = "{0} is required")]
    public Guid? PersonID { get; set; }
    
    [Required(ErrorMessage = "{0} is required")]
    [Display(Name = "Person Name")]
    public string? PersonName { get; set; }
    
    [Required(ErrorMessage = "{0} is required")]
    [Display(Name ="Email")]
    [EmailAddress(ErrorMessage = "Please write {0} in a proper format")]
    public string? Email { get; set; }

    [Required(ErrorMessage = "{0} is required")]
    [Display(Name = "Date of Birth")]
    [DataType(DataType.Date)]
    [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
    public DateTime? DateOfBirth { get; set; }

    [Required(ErrorMessage = "{0} is required")]
    [Display(Name = "Gender")]
    [EnumDataType(typeof(PersonGenderEnum), ErrorMessage = "Please select a {0}")]
    [DisplayFormat(ConvertEmptyStringToNull = true)]
    public PersonGenderEnum? Gender { get; set; }

    [Required(ErrorMessage = "{0} is required")]
    [Display(Name = "CountryID")]
    [DataType(DataType.Text)]
    [DisplayFormat(ConvertEmptyStringToNull = true)]
    public Guid? CountryID { get; set; }

    [Required(ErrorMessage = "{0} is required")]
    [Display(Name = "Address")]
    [DataType(DataType.MultilineText)]
    [StringLength(100, ErrorMessage = "Address cannot be more than {1} characters")]
    [DisplayFormat(ConvertEmptyStringToNull = true)]
    public string? Address{ get; set; }

    [Required(ErrorMessage = "{0} is required")]
    [Display(Name = "Recieves News Letters")]
    public bool RecievesNewsLetters { get; set; }


    public Person ToPerson()
    {
        return new Person()
        {
            PersonID = (Guid)PersonID,
            PersonName = PersonName,
            Email = Email,
            DateOfBirth = DateOfBirth,
            Gender = Gender.ToString(),
            CountryID = CountryID,
            Address = Address,
            RecievesNewsLetters = RecievesNewsLetters

        };
    }
}