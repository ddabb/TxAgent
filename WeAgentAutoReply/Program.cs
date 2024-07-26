using WeAgentAutoReply;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddSingleton<OfficialAccount>(new OfficialAccount("wxe1f3fdcea63ded5d", "da4c70f06bfe2f5ce28f610cfafa4a13", "testtoken", "7A7XMwPjzLQU1eWi23cE4Bi5F3lFopXrNd6EFqq19A2"));

 

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
