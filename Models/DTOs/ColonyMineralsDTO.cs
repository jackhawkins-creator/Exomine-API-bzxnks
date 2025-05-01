 namespace ExomineAPIbzxnks.Models.DTOs;

 public class ColonyMineralDTO {
    public int Id {get; set;}

    public int? ColonyId {get; set;}

    public int? MineralId {get; set;}

    public int? ColonyTons {get; set;}

    public MineralDTO? Mineral { get; set; }
 }