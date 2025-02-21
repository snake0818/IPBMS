var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();
builder.Services.AddHttpClient();
builder.Services.AddCors(options =>
{
    options.AddPolicy(name: "AllowPolicy",
    builder =>
    {
        builder
                // .WithOrigins("http://140.137.41.136:15000/", "http://140.137.41.136:1380/IPBMS/OinkAPI/api")
                .AllowAnyOrigin()
                .AllowAnyHeader()
                .AllowAnyMethod();
    });
});

var config = builder.Configuration;

// 註冊各服務
builder.Services.AddSingleton<IVariableService, VariableService>();
builder.Services.AddSingleton<IValidatorService, ValidatorService>();

var app = builder.Build();

app.UseCors("AllowPolicy");

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Page/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Page}/{action=Index}/{id?}");

app.Run();
