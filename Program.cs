using ExomineAPIbzxnks.Models.DTOs;
using ExomineAPIbzxnks.Models;

var builder = WebApplication.CreateBuilder(args);

List<Governor> governors = new List<Governor>
{
    new Governor { Id = 1, Name = "Edward Wong Hau Pepelu Tivrusky IV", Active = true, ColonyId = 5 },
    new Governor { Id = 2, Name = "Sivaji Ganesan", Active = false, ColonyId = 3 },
    new Governor { Id = 3, Name = "Zhaan", Active = true, ColonyId = 2 },
    new Governor { Id = 4, Name = "Kane", Active = true, ColonyId = 1 },
    new Governor { Id = 5, Name = "Bester", Active = false, ColonyId = 4 }
};

List<Colony> colonies = new List<Colony>
{
    new Colony { Id = 1, Name = "Europa" },
    new Colony { Id = 2, Name = "Tatooine" },
    new Colony { Id = 3, Name = "Xandar" },
    new Colony { Id = 4, Name = "Arrakis" },
    new Colony { Id = 5, Name = "Oklahoma" }
};

List<ColonyMineral> colonyMinerals = new List<ColonyMineral>
{
    new ColonyMineral { Id = 1, ColonyId = 4, MineralId = 3, ColonyTons = 1 },
    new ColonyMineral { Id = 2, ColonyId = 5, MineralId = 5, ColonyTons = 5 },
    new ColonyMineral { Id = 3, ColonyId = 1, MineralId = 1, ColonyTons = 5 },
    new ColonyMineral { Id = 4, ColonyId = 2, MineralId = 4, ColonyTons = 6 },
    new ColonyMineral { Id = 5, ColonyId = 5, MineralId = 2, ColonyTons = 13 }
};

List<Mineral> minerals = new List<Mineral>
{
    new Mineral { Id = 1, Name = "Red Mercury" },
    new Mineral { Id = 2, Name = "Kyber Crystals" },
    new Mineral { Id = 3, Name = "Unobtainium" },
    new Mineral { Id = 4, Name = "Spice Melange" },
    new Mineral { Id = 5, Name = "Vibranium" },
    new Mineral { Id = 6, Name = "Pyroxene" }
};

List<FacilityMineral> facilityMinerals = new List<FacilityMineral>
{
    new FacilityMineral { Id = 1, MineralId = 1, FacilitiesId = 3, FacilityTons = 25 },
    new FacilityMineral { Id = 2, MineralId = 4, FacilitiesId = 5, FacilityTons = 40 },
    new FacilityMineral { Id = 3, MineralId = 2, FacilitiesId = 1, FacilityTons = 30 },
    new FacilityMineral { Id = 4, MineralId = 3, FacilitiesId = 4, FacilityTons = 30 },
    new FacilityMineral { Id = 5, MineralId = 5, FacilitiesId = 2, FacilityTons = 40 }
};

List<Facility> facilities = new List<Facility>
{
    new Facility { Id = 1, Name = "Ganymede", Active = true },
    new Facility { Id = 2, Name = "The Death Star", Active = false },
    new Facility { Id = 3, Name = "S.H.I.E.L.D. Helicarrier", Active = true },
    new Facility { Id = 4, Name = "Imperial Star Destroyer", Active = true },
    new Facility { Id = 5, Name = "Pandora", Active = false }
};


// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

//fetch ALL governors
app.MapGet("/api/governors", () =>
{
    return governors.Select(g => new GovernorDTO
    {
        Id = g.Id,
        Name = g.Name,
        Active = g.Active,
        ColonyId = g.ColonyId
    });
});

//fetch ALL facilities
app.MapGet("/api/facilities", () =>
{
    return facilities.Select(f => new FacilityDTO
    {
        Id = f.Id,
        Name = f.Name,
        Active = f.Active
    });
});


//fetch ALL colonyMinerals
app.MapGet("/api/colonyminerals", () =>
{
    return colonyMinerals.Select(cm => new ColonyMineralDTO
    {
        Id = cm.Id,
        ColonyId = cm.ColonyId,
        MineralId = cm.MineralId,
        ColonyTons = cm.ColonyTons
    });
});

//fetch ALL facilityMinerals
app.MapGet("/api/facilityminerals", () =>
{
    return facilityMinerals.Select(fm => new FacilityMineralDTO
    {
        Id = fm.Id,
        MineralId = fm.MineralId,
        FacilitiesId = fm.FacilitiesId,
        FacilityTons = fm.FacilityTons
    });
});

app.Run();