using CW10_s27161.Models;

namespace CW10_s27161.DTOs
{
    public class CountryTripGetDTO
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public DateTime DateFrom { get; set; }
        public DateTime DateTo { get; set; }
        public int MaxPeople { get; set; }
        
        public virtual ICollection<CountryGetDTO> Countries { get; set; }
        public virtual ICollection<ClientGetDTO> Clients { get; set; }
    }
}