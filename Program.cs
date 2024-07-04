using webchat.Models;
using webchat.Service;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// mongodb+srv://lisacope:Rivrfish1!@cluster0.mirxmih.mongodb.net/?retryWrites=true&w=majority&appName=Cluster0
builder.Services.AddControllers();

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.Configure<WsDatabaseSettingsClass>(
    builder.Configuration.GetSection("MessageDatabase"));
builder.Services.AddSingleton<MessageService>();
builder.Services.AddSingleton<WebSocketService>();
builder.Services.AddSingleton<UserService>();
builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(
        builder =>
        {
            builder.WithOrigins("http://localhost:4200")
                                .AllowAnyHeader()
                                .AllowAnyMethod();
        });
});

var dbConnectionString = builder.Configuration["Messages:ConnectionString"];

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

var webSocketOptions = new WebSocketOptions { KeepAliveInterval = TimeSpan.FromMinutes(2) };

app.UseWebSockets(webSocketOptions);

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();
app.UseCors();

app.Run();
