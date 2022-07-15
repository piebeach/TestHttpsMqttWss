using MQTTnet.AspNetCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddRazorPages();

builder.Services.AddHostedMqttServer(
                optionsBuilder =>
                {
                   optionsBuilder.WithDefaultEndpoint();
                });

builder.Services.AddMqttConnectionHandler();
builder.Services.AddConnections();



var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
   app.UseExceptionHandler("/Error");
   // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
   app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();


app.UseEndpoints(endpoints =>
{
   endpoints.MapConnectionHandler<MqttConnectionHandler>(
                        "/mqtt",
                        httpConnectionDispatcherOptions => httpConnectionDispatcherOptions.WebSockets.SubProtocolSelector =
                            protocolList => protocolList.FirstOrDefault() ?? string.Empty);
});

app.UseMqttServer(server =>
{
   /*
      * Attach event handlers etc. if required.
      */

   server.ValidatingConnectionAsync += e =>
   {
      Console.WriteLine($"Client '{e.ClientId}' wants to connect. Accepting!");
      return Task.CompletedTask;
   };

   server.ClientConnectedAsync += e =>
   {
      Console.WriteLine($"Client '{e.ClientId}' connected.");
      return Task.CompletedTask;
   };
});

app.UseAuthorization();

app.MapRazorPages();

app.Run();
