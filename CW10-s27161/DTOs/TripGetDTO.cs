using CW10_s27161.Models;

namespace CW10_s27161.DTOs
{
    public class TripGetDTO
    {
        public int PageNum { get; set; }
        public int PageSize { get; set; } = 10;
        public int AllPages { get; set; } = 20;
        public ICollection<CountryTripGetDTO> Trips { get; set; }
        
    }
}