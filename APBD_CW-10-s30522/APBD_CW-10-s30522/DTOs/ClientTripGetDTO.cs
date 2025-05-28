namespace APBD_CW_10_s30522.DTOs;

public class ClientTripGetDTO
{
    public int IdClient { get; set; }
    public int IdTrip { get; set; }
    public DateTime? PaymentDate { get; set; }
    public DateTime RegisteredAt { get; set; }
}