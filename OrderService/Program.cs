using MassTransit;
using Microsoft.EntityFrameworkCore;
using OrderService.Consumer;
using OrderService.Data;

var builder = WebApplication.CreateBuilder(args);

// Add Database Connection
builder.Services.AddDbContext<OrderDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

// Add services to the container.
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// MassTransit Configuration
builder.Services.AddMassTransit(x =>
{
    // Register Consumer
    x.AddConsumer<GetOrdersConsumer>();

    // RabbitMQ Configuration
    x.UsingRabbitMq((context, cfg) =>
    {
        cfg.Host("rabbitmq://localhost"); // RabbitMQ Host

        // Queue for Get Orders Consumer
        cfg.ReceiveEndpoint("get-orders-queue", e =>
        {
            e.ConfigureConsumer<GetOrdersConsumer>(context);
        });
    });
});

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
