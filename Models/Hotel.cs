using System.ComponentModel.DataAnnotations;

namespace HotelBookingSystem.Models
{
    public class Hotel
    {
        public int Id { get; set; }

        [Required]
        public string Name { get; set; }

        [Required]
        public string Location { get; set; }


        public List<Room> Rooms { get; set; } = new List<Room>();
    }
}
