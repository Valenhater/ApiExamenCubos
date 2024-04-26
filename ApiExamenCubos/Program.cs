using Azure.Security.KeyVault.Secrets;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Azure;
using NSwag.Generation.Processors.Security;
using NSwag;
using ApiExamenCubos.Helpers;
using ApiExamenCubos.Repositories;
using ApiExamenCubos.Data;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddAzureClients(factory =>
{
    factory.AddSecretClient
    (builder.Configuration.GetSection("KeyVault"));
});

SecretClient secretClient = builder.Services.BuildServiceProvider().GetService<SecretClient>();
KeyVaultSecret audienceKey = await secretClient.GetSecretAsync("Audience");
KeyVaultSecret issuerKey = await secretClient.GetSecretAsync("Issuer");
KeyVaultSecret secretKey = await secretClient.GetSecretAsync("SecretKey"); //Aqui ponemos el nombre del secret
KeyVaultSecret connectionKey = await secretClient.GetSecretAsync("ConnectionString");
string audience = audienceKey.Value;
string issuer = issuerKey.Value;
string secret = secretKey.Value;
string connectionkey = connectionKey.Value;


HelperActionServicesOAuth helper = new HelperActionServicesOAuth(issuer, audience, secret, connectionkey);

builder.Services.AddSingleton<HelperActionServicesOAuth>(helper);

//habilitamos los servicios de auth que hemos creado 
//en el helper con action<>
builder.Services.AddAuthentication
    (helper.GetAuthenticateSchema())
    .AddJwtBearer(helper.GetJwtBearerOptions());

// Add services to the container.
string connectionString = connectionKey.Value;

builder.Services.AddTransient<RepositoryCubos>();
builder.Services.AddDbContext<CubosContext> (options => options.UseSqlServer(connectionString));


builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
// REGISTRAMOS SWAGGER COMO SERVICIO
builder.Services.AddOpenApiDocument(document =>
{
    document.Title = "Api OAuth Cubos";
    document.Description = "Api con seguridad 2024";
    document.AddSecurity("JWT", Enumerable.Empty<string>(),
        new NSwag.OpenApiSecurityScheme
        {
            Type = OpenApiSecuritySchemeType.ApiKey,
            Name = "Authorization",
            In = OpenApiSecurityApiKeyLocation.Header,
            Description = "Copia y pega el Token en el campo 'Value:' así: Bearer {Token JWT}."
        }
    );
    document.OperationProcessors.Add(
    new AspNetCoreOperationSecurityScopeProcessor("JWT"));
});

var app = builder.Build();
app.UseOpenApi();
//app.UseSwagger();
app.UseSwaggerUI(options =>
{
    options.InjectStylesheet("/css/monokai_theme.css");
    //options.InjectStylesheet("/css/material3x.css");
    options.SwaggerEndpoint(url: "/swagger/v1/swagger.json"
        , name: "Api OAuth Cubos");
    options.RoutePrefix = "";
});
// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{

}

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();