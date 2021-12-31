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

var commandMode = false;
var currentCommandMode = "";
var garageStatus = "";

app.Urls.Add("https://*:3000");
app.Urls.Add("http://*:3001");


app.UseHttpsRedirection();

void RunCommand(string command)
{
    commandMode = true;
    if (command == "OPEN")
    {
        garageStatus = "closed";
        Thread.Sleep(5000);
        garageStatus = "opening";
        Thread.Sleep(10000);
        garageStatus = "open";
        Thread.Sleep(10000);
    }
    if (command == "CLOSE")
    {
        garageStatus = "open";
        Thread.Sleep(5000);
        garageStatus = "closing";
        Thread.Sleep(10000);
        garageStatus = "closed";
        Thread.Sleep(10000);
    }
    commandMode = false;
}

var statuses = new[]
{
        "closed", "opening", "open", "closing"
    };

app.MapPost("/v1/devices/{garageAccessKey}/commands", (string garageAccessKey, Command[] commands) =>
{
    // Thread.Sleep(4000);

    // { "results":[{ "id":"e2649c22-ccd7-4a72-81a4-fad8aa79632f","status":"ACCEPTED"}]}

    if (commands[0].command == "OPEN" || commands[0].command == "CLOSE")
    {
        commandMode = true;
        currentCommandMode = commands[0].command;
        Thread thread = new Thread(() => { RunCommand(currentCommandMode); });
        thread.Start();
    }
    var status = new { results = new[] { new { status = "ACCEPTED" } } };
    return status;
})
    .WithName("GarageCommands");

app.MapGet("/v1/devices/{garageAccessKey}/status", (string garageAccessKey) =>
{
    var time = (int)((float)DateTime.Now.Second / (float)60 * 4);

    Object status;

    // Thread.Sleep(4000);

    if (commandMode)
    {
        status = new { components = new { main = new { doorControl = new { door = new { value = garageStatus } } } } };
        return status;
    }

    status = new { components = new { main = new { doorControl = new { door = new { value = statuses[time], time = time } } } } };
    return status;
})
    .WithName("GarageStatus");

app.Run();

class Command
{
    public string command { get; set; }
}
