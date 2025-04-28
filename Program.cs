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
    new FacilityMineral { Id = 1, MineralId = 1, FacilityId = 3, FacilityTons = 25 },
    new FacilityMineral { Id = 2, MineralId = 4, FacilityId = 5, FacilityTons = 40 },
    new FacilityMineral { Id = 3, MineralId = 2, FacilityId = 1, FacilityTons = 30 },
    new FacilityMineral { Id = 4, MineralId = 3, FacilityId = 4, FacilityTons = 30 },
    new FacilityMineral { Id = 5, MineralId = 5, FacilityId = 2, FacilityTons = 40 }
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

app.MapGet("/api/facilityminerals", (int? facilityId, int? mineralId, string? expand) =>
{
    List<FacilityMineral> joinTables = facilityMinerals.ToList();

    if (facilityId != null || mineralId != null)
    {
        joinTables = joinTables.Where(jt =>
        (facilityId == null || jt.FacilityId == facilityId) &&
        (mineralId == null || jt.MineralId == mineralId)
        ).ToList();
    }

    if (expand == null) {
        expand = "";
    }

    return joinTables.Select(jt =>
    {
        Mineral m = minerals.FirstOrDefault(m => m.Id == jt.MineralId);
        Facility f = facilities.FirstOrDefault(f => f.Id == jt.FacilityId);

        return new FacilityMineralDTO
        {
            Id = jt.Id,
            MineralId = jt.MineralId,
            FacilityId = jt.FacilityId,
            FacilityTons = jt.FacilityTons,
            Mineral = expand.Contains("mineral") && m != null ? new MineralDTO
            {
                Id = m.Id,
                Name = m.Name
            } : null,
            Facility = expand.Contains("facility") && m != null ? new FacilityDTO
            {
                Id = f.Id,
                Name = f.Name
            } : null
        };
    });
});

//Fetches a mineral by id
app.MapGet("api/minerals/{id}", (int id) =>
{
    Mineral mineral = minerals.FirstOrDefault(m => m.Id == id);
    if (mineral == null)
    {
        return Results.NotFound();
    }

    return Results.Ok(new MineralDTO
    {
        Id = mineral.Id,
        Name = mineral.Name,
    });
});

//Fetches a facility by id
app.MapGet("api/facilities/{id}", (int id) =>
{
    Facility facility = facilities.FirstOrDefault(f => f.Id == id);
    if (facility == null)
    {
        return Results.NotFound();
    }

    return Results.Ok(new FacilityDTO
    {
        Id = facility.Id,
        Name = facility.Name,
        Active = facility.Active
    });
});

//Fetches ColonyMineral by Id
app.MapGet("api/colonyMinerals/{id}", (int id) =>
{
    ColonyMineral colonyMineral = colonyMinerals.FirstOrDefault(cm => cm.Id == id);
    if (colonyMineral == null)
    {
        return Results.NotFound();
    }

    return Results.Ok(new ColonyMineralDTO
    {
        Id = colonyMineral.Id,
        ColonyId = colonyMineral.ColonyId,
        MineralId = colonyMineral.MineralId,
        ColonyTons = colonyMineral.ColonyTons


    });
});

//Fetches a FacilityMineral by id
app.MapGet("api/facilityMinerals/{id}", (int id) =>
{
    FacilityMineral facilityMineral = facilityMinerals.FirstOrDefault(fm => fm.Id == id);
    if (facilityMineral == null)
    {
        return Results.NotFound();
    }

    return Results.Ok(new FacilityMineralDTO
    {
        Id = facilityMineral.Id,
        MineralId = facilityMineral.MineralId,
        FacilityId = facilityMineral.FacilityId,
        FacilityTons = facilityMineral.FacilityTons


    });
});

//Puts to a colonyMineral by Id and body
app.MapPut("/api/colonyMinerals/{id}", (int id, ColonyMineral colonyMineral) => {

    ColonyMineral cm = colonyMinerals.FirstOrDefault(cm => cm.Id == id);
    Colony colony = colonies.FirstOrDefault(c => c.Id == colonyMineral.ColonyId);
    Mineral mineral = minerals.FirstOrDefault(m => m.Id == colonyMineral.MineralId);

    if (cm == null) {
        return Results.NotFound($"ColonyMineral id: {id} not found");
    }

    if (colonyMineral.ColonyId == null || colony == null) {
        return Results.BadRequest($"ColonyId: {colonyMineral.ColonyId} must be a valid colony id");
    }

    if (colonyMineral.MineralId == null || mineral == null) {
        return Results.BadRequest($"MineralId: {colonyMineral.MineralId} must be a valid colony id");
    }

    if (colonyMineral.ColonyTons == null) {
        return Results.BadRequest("ColonyTons must be a valid integer");
    }

    cm.ColonyId = colonyMineral.ColonyId;
    cm.MineralId = colonyMineral.MineralId;
    cm.ColonyTons = colonyMineral.ColonyTons;

    return Results.Accepted($"/colonyMinerals/{id}", new ColonyMineralDTO {
        Id = cm.Id,
        ColonyId = cm.ColonyId,
        MineralId = cm.MineralId,
        ColonyTons = cm.ColonyTons
    });

});

app.MapPost("/colonyMinerals", (ColonyMineral colonyMineral) => {

});

app.Run();