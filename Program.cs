var builder = WebApplication.CreateBuilder(args);

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

app.Urls.Add("https://*:3000");
app.Urls.Add("http://*:3001");


app.UseHttpsRedirection();

var statuses = new[]
{
        "closed", "opening", "open", "closing"
    };

app.MapPost("/v1/devices/{garageAccessKey}/commands", (string garageAccessKey) =>
{
// Thread.Sleep(4000);

// { "results":[{ "id":"e2649c22-ccd7-4a72-81a4-fad8aa79632f","status":"ACCEPTED"}]}

    var status = new { results = new [] { new { status = "ACCEPTED" } } };
    return status;
})
    .WithName("GarageCommands");

app.MapGet("/v1/devices/{garageAccessKey}/status", (string garageAccessKey) =>
{

    var time = (int)((float) DateTime.Now.Second / (float) 60 * 4); 

    // Thread.Sleep(4000);

    var status = new { components = new { main = new { doorControl = new { door = new { value = statuses[time], time=time } } } } };
    return status;
})
    .WithName("GarageStatus");

app.Run();
