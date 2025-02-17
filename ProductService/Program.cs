using MassTransit;
using Microsoft.EntityFrameworkCore;
using OrderService.Data;
using ProductService.Consumers;

var builder = WebApplication.CreateBuilder(args);

// Add Database Connection
builder.Services.AddDbContext<ProductDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

// Add services to the container.
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// MassTransit Configuration
builder.Services.AddMassTransit(x =>
{
    // Register Consumers
    x.AddConsumer<OrderCreatedConsumer>();
    x.AddConsumer<OrderRequestConsumer>();

    // RabbitMQ Configuration
    x.UsingRabbitMq((context, cfg) =>
    {
        cfg.Host("rabbitmq://localhost");

        // Order Created Queue
        cfg.ReceiveEndpoint("order-created-queue", e =>
        {
            e.ConfigureConsumer<OrderCreatedConsumer>(context);
        });

        // Order Request Queue
        cfg.ReceiveEndpoint("order-request-queue", e =>
        {
            e.ConfigureConsumer<OrderRequestConsumer>(context);
        });
    });

    // Request Client for Orders
    x.AddRequestClient<EventContracts.IGetOrdersRequest>();
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
