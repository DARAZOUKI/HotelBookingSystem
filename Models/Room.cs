using System.ComponentModel.DataAnnotations;

namespace HotelBookingSystem.Models
{
    public class Room
    {
        public int Id { get; set; }
        [Required]
        public int HotelId { get; set; }
        public Hotel? Hotel { get; set; }

        [Required]
        public  string RoomType { get; set; }  = string.Empty;

        [Required]
        public decimal PricePerNight { get; set; }

        public bool IsAvailable { get; set; } = true;
    }
}
