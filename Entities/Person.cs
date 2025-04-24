using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entities
{
    public class Person
    {
        [Key]
        public Guid PersonID{ get; set; }
        
        [StringLength(20)]
        public string? PersonName{ get; set; }
        
        [StringLength(30)]
        public string? Email{ get; set; }
        
        public DateTime? DateOfBirth { get; set; }
        
        [StringLength(10)]
        public string? Gender{ get; set; }
        
        public Guid? CountryID{ get; set; }
        
        [StringLength(100)]
        public string? Address{ get; set; }
        
        public bool RecievesNewsLetters{ get; set; }
        
        [ForeignKey("CountryID")]
        public Country? Country{ get; set; }
    }
}
