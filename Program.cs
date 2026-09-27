var builder = WebApplication.CreateBuilder(args);

// Razor Pages
builder.Services.AddRazorPages();

// SignalR
builder.Services.AddSignalR();

var app = builder.Build();

// Development mode
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
}

// IMPORTANT:
// LAN testing-এর সময় HTTPS redirection বন্ধ রাখা হয়েছে.
// app.UseHttpsRedirection();

app.UseStaticFiles();

app.UseRouting();

app.UseAuthorization();

// Razor Pages
app.MapRazorPages();

// SignalR Hub
app.MapHub<ChatHub>("/chathub");

app.Run();