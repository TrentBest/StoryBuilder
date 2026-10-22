using StoryBuilder.MicroBundle;
using StoryBuilder.Core.Services;
using StoryBuilder.FSM;
using StoryBuilder.WebDemo;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddRazorComponents().AddInteractiveServerComponents();
builder.Services.AddSingleton<StoryBuilderAuthoringBundle>();
builder.Services.AddScoped<StoryLifecycleRegistry>();
builder.Services.AddScoped<StoryBuilderPersistenceCoordinator>();
builder.Services.AddScoped<StoryBuilderSession>(services =>
    services.GetRequiredService<StoryBuilderAuthoringBundle>().CreateSession("StoryBuilder Web Demo"));

var app = builder.Build();

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    app.UseHsts();
}

app.UseStaticFiles();
app.UseAntiforgery();
app.MapRazorComponents<StoryBuilder.WebDemo.Components.App>()
    .AddInteractiveServerRenderMode();
app.Run();

public partial class Program { }
