using Microsoft.EntityFrameworkCore;
using bancoApiGame;

var builder = WebApplication.CreateBuilder(args);

// Configura o DbContext com a string de conexão
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlServer(connectionString));

// Add services to the container.
builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(builder =>
    {
        builder.AllowAnyOrigin().AllowAnyMethod().AllowAnyHeader();
    });
});

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseCors();

app.MapPost("/api/pergunta", async (AppDbContext db, Pergunta pergunta) =>
{
    db.Perguntas.Add(pergunta);
    await db.SaveChangesAsync();
    return Results.Created($"/api/pergunta/{pergunta.Id}", pergunta);
});

app.MapPut("/api/pergunta/{id}", async (AppDbContext db, int id, Pergunta inputPergunta) =>
{
    var pergunta = await db.Perguntas.FindAsync(id);

    if (pergunta == null)
    {
        return Results.NotFound("Pergunta não encontrada.");
    }

    pergunta.Question = inputPergunta.Question;
    pergunta.ChemicalElement = inputPergunta.ChemicalElement;

    await db.SaveChangesAsync();

    return Results.Ok(pergunta);
});

app.MapDelete("/api/pergunta/{id}", async (AppDbContext db, int id) =>
{
    var pergunta = await db.Perguntas.FindAsync(id);

    if (pergunta == null)
    {
        return Results.NotFound("Pergunta não encontrada.");
    }

    db.Perguntas.Remove(pergunta);
    await db.SaveChangesAsync();

    return Results.Ok("Pergunta excluída com sucesso.");
});

app.MapGet("/api/pergunta", async (AppDbContext db) =>
{
    return Results.Ok(await db.Perguntas.ToListAsync());
});

app.MapGet("/api/pergunta/{id}", async (AppDbContext db, int id) =>
{
    var pergunta = await db.Perguntas.FindAsync(id);

    if (pergunta == null)
    {
        return Results.NotFound("Pergunta não encontrada.");
    }

    return Results.Ok(pergunta);
});

app.MapPost("/api/resposta", async (AppDbContext db, Resposta resposta) =>
{
    db.Respostas.Add(resposta);
    await db.SaveChangesAsync();
    return Results.Created($"/api/resposta/{resposta.Id}", resposta);
});

app.MapGet("/api/resposta", async (AppDbContext db) =>
{
    return Results.Ok(await db.Respostas.ToListAsync());
});

app.MapGet("/api/resposta/{id}", async (AppDbContext db, int id) =>
{
    var resposta = await db.Respostas.FindAsync(id);

    if (resposta == null)
    {
        return Results.NotFound("Resposta não encontrada.");
    }

    return Results.Ok(resposta);
});

app.MapDelete("/api/pergunta", async (AppDbContext db) =>
{
    db.Perguntas.RemoveRange(db.Perguntas);
    await db.SaveChangesAsync();
    return Results.NoContent();
});

app.MapDelete("/api/resposta", async (AppDbContext db) =>
{
    db.Respostas.RemoveRange(db.Respostas);
    await db.SaveChangesAsync();
    return Results.NoContent();
});

app.MapGet("/api/pergunta/maxid", async (AppDbContext db) =>
{
    int maxId = await db.Perguntas.AnyAsync() ? await db.Perguntas.MaxAsync(p => p.Id) : 1;
    return Results.Ok(maxId);
});

app.MapGet("/api/pergunta/minid", async (AppDbContext db) =>
{
    int minId = await db.Perguntas.AnyAsync() ? await db.Perguntas.MinAsync(p => p.Id) : 1;
    return Results.Ok(minId);
});


app.MapGet("/api/resposta/maxid/{alunoId}", async (int alunoId, AppDbContext db) =>
{
    var maxId = await db.Respostas
                   .Where(r => r.AlunoId == alunoId)
                   .Select(r => r.Id)
                   .DefaultIfEmpty()
                   .MaxAsync();
    return Results.Ok(maxId);
});

app.Run();

