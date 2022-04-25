using LANMovie.Common;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;

var builder = WebApplication.CreateBuilder(args);


GlobalValues.AppConfig = AppConfig.Parse();


/// <summary>
/// ×¢²á·þÎñ
/// </summary>
builder.Services.AddAntDesign();
builder.Services.AddRazorPages();
builder.Services.AddServerSideBlazor();

var app = builder.Build();

app.UseStaticFiles();
app.UseRouting();
app.MapControllers();
app.MapBlazorHub();
app.MapFallbackToPage("/_Host");
app.Urls.Add("http://0.0.0.0:13145");

app.Run();
