using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DS_Project.Cars
{
    public class Car
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        public Guid CarUid { get; set; }

        public string Brand { get; set; }

        public string Model { get; set; }

        public string RegistrationNumber { get; set; }

        public int? Power { get; set; }

        public int Price { get; set; }

        public string Type { get; set; }

        public bool Availability { get; set; }
    }
}
