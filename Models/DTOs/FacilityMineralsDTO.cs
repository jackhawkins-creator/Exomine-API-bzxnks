 namespace ExomineAPIbzxnks.Models.DTOs;

 
public class FacilityMineralDTO {

    public int Id {get; set;}

    public int? MineralId {get; set;}

    public int? FacilityId {get; set;}

    public int? FacilityTons {get; set;}

    public MineralDTO? Mineral {get; set;}

    public FacilityDTO? Facility {get; set;}
}