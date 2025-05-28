using System.Collections;

namespace APBD_CW_10_s30522.DTOs;

public class TripDetailGetDTO
{
    public string Name { get; set; } = null!;

    public string Description { get; set; } = null!;

    public DateTime DateFrom { get; set; }

    public DateTime DateTo { get; set; }

    public int MaxPeople { get; set; }

    public ICollection<CountryBasicGetDTO> Countries { get; set; } = null!;

    public ICollection<ClientBasicGetDTO> Clients { get; set; } = null!;
}