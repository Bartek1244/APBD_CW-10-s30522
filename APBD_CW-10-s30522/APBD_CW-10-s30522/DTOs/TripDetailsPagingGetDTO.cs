namespace APBD_CW_10_s30522.DTOs;

public class TripDetailsPagingGetDTO
{
    public int PageNum { get; set; }
    
    public int PageSize { get; set; }
    
    public int AllPages { get; set; }
    
    public ICollection<TripDetailGetDTO> Trips { get; set; }
}