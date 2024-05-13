using ebooks_dotnet7_api;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddDbContext<DataContext>(opt => opt.UseInMemoryDatabase("ebooks"));
builder.Services.AddDatabaseDeveloperPageExceptionFilter();
var app = builder.Build();
var ebooks = app.MapGroup("api/ebook");

// TODO: Add more routes
ebooks.MapPost("/", CreateEBookAsync);



app.MapPost("/ebook", async (EBook ebook, DataContext db) => 
{ 
    var existingEBook = await db.EBooks.AnyAsync(e => e.Title == ebook.Title && e.Author == ebook.Author);
     if (existingEBook) 
        { 
            return Results.BadRequest("Ya existe un libro con el mismo nombre"); 
        } 
     db.EBooks.Add(ebook); await db.SaveChangesAsync(); 
     return 
        Results.Created($"/ebooks/{ebook.Id}", ebook);
});
app.MapGet("/ebook?genre={genre}&author={author}&format={format}", async (DataContext db) => 
    await db.EBooks.ToListAsync());

app.MapPut("/ebooks/{id}", async (int id, EBook inputEbook, DataContext db) => 
{ 
    var ebook = await db.EBooks.FindAsync(id); 
    if (ebook is null) 
    return 
        Results.NotFound(); 
    ebook.Title = inputEbook.Title; 
    ebook.Author = inputEbook.Author; 
    ebook.Genre = inputEbook.Genre; 
    ebook.Format = inputEbook.Format; 
    ebook.Price = inputEbook.Price; 
    await db.SaveChangesAsync(); 
    return Results.NoContent(); }); 
    // Endpoint para eliminar un libro electrónico 
    app.MapDelete("/ebooks/{id}", async (int id, DataContext db) => 
    {
        if (await db.EBooks.FindAsync(id) is EBook ebook) 
        { 
            db.EBooks.Remove(ebook); 
            await db.SaveChangesAsync(); 
            return 
                Results.NoContent(); 
        } 
            return Results.NotFound(); });

// TODO: Add more methods
async Task<IResult> CreateEBookAsync(DataContext context)
{
    return Results.Ok();
}

app.Run();