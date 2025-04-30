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
    new Colony { Id = 1, Name = "Europa", Currency = 100 },
    new Colony { Id = 2, Name = "Tatooine", Currency = 75 },
    new Colony { Id = 3, Name = "Xandar", Currency = 165 },
    new Colony { Id = 4, Name = "Arrakis", Currency = 110 },
    new Colony { Id = 5, Name = "Oklahoma", Currency = 190 }
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
    new FacilityMineral { Id = 1, MineralId = 1, FacilityId = 3, FacilityTons = 25, HourlyRate = 3, MineralPrice = 50 },
    new FacilityMineral { Id = 2, MineralId = 4, FacilityId = 5, FacilityTons = 40, HourlyRate = 5, MineralPrice = 70 },
    new FacilityMineral { Id = 3, MineralId = 2, FacilityId = 1, FacilityTons = 30, HourlyRate = 7, MineralPrice = 85 },
    new FacilityMineral { Id = 4, MineralId = 3, FacilityId = 4, FacilityTons = 30, HourlyRate = 2, MineralPrice = 40 },
    new FacilityMineral { Id = 5, MineralId = 5, FacilityId = 2, FacilityTons = 40, HourlyRate = 10, MineralPrice = 90 }
};

List<Facility> facilities = new List<Facility>
{
    new Facility { Id = 1, Name = "Ganymede", Active = true, Currency = 500 },
    new Facility { Id = 2, Name = "The Death Star", Active = false, Currency = 800 },
    new Facility { Id = 3, Name = "S.H.I.E.L.D. Helicarrier", Active = true, Currency = 300 },
    new Facility { Id = 4, Name = "Imperial Star Destroyer", Active = true, Currency = 200 },
    new Facility { Id = 5, Name = "Pandora", Active = false, Currency = 375 }
};


// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll",
        policy =>
        {
            policy.AllowAnyOrigin()
                  .AllowAnyMethod()
                  .AllowAnyHeader();
        });
});

var app = builder.Build();

app.UseCors("AllowAll");

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseCors("AllowMyOrigin");

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
        Active = f.Active,
        Currency = f.Currency
    });
});

//fetch ALL facilityMinerals
app.MapGet("/api/facilityMinerals", (int? facilityId, int? mineralId, string? expand) =>
{
    List<FacilityMineral> joinTables = facilityMinerals.ToList();

    if (facilityId != null || mineralId != null)
    {
        joinTables = joinTables.Where(jt =>
        (facilityId == null || jt.FacilityId == facilityId) &&
        (mineralId == null || jt.MineralId == mineralId)
        ).ToList();
    }

    if (expand == null)
    {
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
            HourlyRate = jt.HourlyRate,
            MineralPrice = jt.MineralPrice,
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
        FacilityTons = facilityMineral.FacilityTons,
        HourlyRate = facilityMineral.HourlyRate,
        MineralPrice = facilityMineral.MineralPrice,
    });
});

// Fetch a single governor with their colony
app.MapGet("/api/governors/{id}", (int id) =>
{
    Governor governor = governors.FirstOrDefault(g => g.Id == id);
    if (governor == null)
    {
        return Results.NotFound();
    }


    Colony colony = colonies.FirstOrDefault(c => c.Id == governor.ColonyId);
    var colonyDTO = colony != null ? new ColonyDTO { Id = colony.Id, Name = colony.Name, Currency = colony.Currency } : null;


    var governorDTO = new GovernorDTO
    {
        Id = governor.Id,
        Name = governor.Name,
        Active = governor.Active,
        ColonyId = governor.ColonyId,
        Colonies = colonyDTO != null ? new List<ColonyDTO> { colonyDTO } : new List<ColonyDTO>()
    };


    return Results.Ok(governorDTO);
});

//PUT to a colonyMineral by Id and body
app.MapPut("/api/colonyMinerals/{id}", (int id, ColonyMineral colonyMineral) =>
{

    ColonyMineral cm = colonyMinerals.FirstOrDefault(cm => cm.Id == id);
    Colony colony = colonies.FirstOrDefault(c => c.Id == colonyMineral.ColonyId);
    Mineral mineral = minerals.FirstOrDefault(m => m.Id == colonyMineral.MineralId);

    if (cm == null)
    {
        return Results.NotFound($"ColonyMineral id: {id} not found");
    }

    if (colony == null)
    {
        return Results.BadRequest($"ColonyId: {colonyMineral.ColonyId} must be a valid colony id");
    }

    if (mineral == null)
    {
        return Results.BadRequest($"MineralId: {colonyMineral.MineralId} must be a valid mineral id");
    }


    cm.ColonyId = colonyMineral.ColonyId;
    cm.MineralId = colonyMineral.MineralId;
    cm.ColonyTons = colonyMineral.ColonyTons;

    return Results.Accepted($"/colonyMinerals/{id}", new ColonyMineralDTO
    {
        Id = cm.Id,
        ColonyId = cm.ColonyId,
        MineralId = cm.MineralId,
        ColonyTons = cm.ColonyTons
    });

});

//POST a colonyMineral
app.MapPost("/api/colonyMinerals", (ColonyMineral colonyMineral) =>
{
    Colony colony = colonies.FirstOrDefault(c => c.Id == colonyMineral.ColonyId);
    Mineral mineral = minerals.FirstOrDefault(m => m.Id == colonyMineral.MineralId);

    if (colony == null)
    {
        return Results.BadRequest($"ColonyId: {colonyMineral.ColonyId} must be a valid colony id");
    }

    if (mineral == null)
    {
        return Results.BadRequest($"MineralId: {colonyMineral.MineralId} must be a valid mineral id");
    }

    colonyMineral.Id = colonyMinerals.Max(cm => cm.Id) + 1;
    colonyMinerals.Add(colonyMineral);

    return Results.Created($"/api/colonyMinerals/{colonyMineral.Id}", new ColonyMineralDTO
    {
        Id = colonyMineral.Id,
        ColonyId = colonyMineral.ColonyId,
        MineralId = colonyMineral.MineralId,
        ColonyTons = colonyMineral.ColonyTons
    });
});

//PATCH a facilityMineral
app.MapPatch("/api/facilityMinerals/{id}", (int id, FacilityMineralDTO updates) =>
{
    FacilityMineral fm = facilityMinerals.FirstOrDefault(fm => fm.Id == id);

    if (fm == null)
    {
        return Results.NotFound();
    }

    if (updates.MineralId.HasValue)
        fm.MineralId = updates.MineralId.Value;

    if (updates.FacilityId.HasValue)
        fm.FacilityId = updates.FacilityId.Value;

    if (updates.FacilityTons.HasValue)
        fm.FacilityTons = updates.FacilityTons.Value;

    if (updates.HourlyRate.HasValue)
        fm.FacilityTons = updates.HourlyRate.Value;

    if (updates.MineralPrice.HasValue)
        fm.FacilityTons = updates.MineralPrice.Value;

    return Results.Accepted($"/api/facilityMinerals/{id}", new FacilityMineralDTO
    {
        Id = fm.Id,
        MineralId = fm.MineralId,
        FacilityId = fm.FacilityId,
        FacilityTons = fm.FacilityTons,
        HourlyRate = fm.HourlyRate,
        MineralPrice = fm.MineralPrice,
    });
});

app.MapGet("/api/colonyMinerals", (int? colonyId, string? expand) =>
{
    List<ColonyMineral> filtered = colonyMinerals.ToList();

    if (colonyId != null)
    {
        filtered = filtered.Where(cm => cm.ColonyId == colonyId).ToList();
    }

    if (expand == null)
    {
        expand = "";
    }

    return filtered.Select(cm =>
    {
        Mineral m = minerals.FirstOrDefault(m => m.Id == cm.MineralId);

        return new ColonyMineralDTO
        {
            Id = cm.Id,
            ColonyId = cm.ColonyId,
            MineralId = cm.MineralId,
            ColonyTons = cm.ColonyTons,
            Mineral = expand.Contains("mineral") && m != null ? new MineralDTO
            {
                Id = m.Id,
                Name = m.Name
            } : null
        };
    });
});

app.MapPut("/api/facilityMinerals/waitOneHour", () =>
{
    return facilityMinerals.Select(fm =>
    {
        fm.FacilityTons += fm.HourlyRate;

        return new FacilityMineralDTO
        {
            Id = fm.Id,
            MineralId = fm.MineralId,
            FacilityId = fm.FacilityId,
            FacilityTons = fm.FacilityTons,
            HourlyRate = fm.HourlyRate,
            MineralPrice = fm.MineralPrice,
        };
    });
});

app.MapPut("/api/colonies/{id}", (int id, Colony Update) =>
{
    Colony colony = colonies.FirstOrDefault(c => c.Id == id);

    if (colony == null)
    {
        return Results.NotFound();
    }

    colony.Name = Update.Name;
    colony.Currency = Update.Currency;

    return Results.Ok(
        new ColonyDTO
        {
            Id = colony.Id,
            Name = colony.Name,
            Currency = colony.Currency,
        }
    );
});

app.MapPut("/api/facilities/{id}", (int id, Facility Update) =>
{
    Facility facility = facilities.FirstOrDefault(c => c.Id == id);

    if (facility == null)
    {
        return Results.NotFound();
    }

    facility.Name = Update.Name;
    facility.Active = Update.Active;
    facility.Currency = Update.Currency;

    return Results.Ok(
        new FacilityDTO
        {
            Id = facility.Id,
            Name = facility.Name,
            Active = facility.Active,
            Currency = facility.Currency,
        }
    );
});

app.MapGet("/api/facilities/{id}", (int id) =>
{
    Facility facility = facilities.FirstOrDefault(c => c.Id == id);

    if (facility == null)
    {
        return Results.NotFound();
    }
    return Results.Ok(
        new FacilityDTO
        {
            Id = facility.Id,
            Name = facility.Name,
            Active = facility.Active,
            Currency = facility.Currency,
        }
    );
});

app.Run();

