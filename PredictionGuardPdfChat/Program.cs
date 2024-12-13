using Microsoft.AspNetCore.DataProtection.KeyManagement;
using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.Data;
using PredictionGuard.Extensions;
using PredictionGuardPdfChat;
using PredictionGuardPdfChat.Components;
using PredictionGuardPdfChat.Services;
using Syncfusion.Blazor;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();
builder.Configuration.AddUserSecrets<Program>();

// Read PredictionGuard configuration from user secrets
var predictionGuardChatConfig = builder.Configuration.GetSection("PredictionGuard:Chat").Get<PredictionGuardConfig>();
var predictionGuardEmbeddingsConfig = builder.Configuration.GetSection("PredictionGuard:Embeddings").Get<PredictionGuardConfig>();

builder.Services.AddPredictionGuardChatClient(predictionGuardChatConfig.ApiKey, options => {
    options.Endpoint = new Uri(predictionGuardChatConfig.Endpoint);
    options.Model = predictionGuardChatConfig.Model;
});

builder.Services.AddPredictionGuardEmbeddingsClient(predictionGuardEmbeddingsConfig.ApiKey, options =>
{
    options.Endpoint = new Uri(predictionGuardEmbeddingsConfig.Endpoint);
    options.Model = predictionGuardEmbeddingsConfig.Model;
});

builder.Services.AddInMemoryVectorStore();
builder.Services.AddControllers();
builder.Services.AddSyncfusionBlazor();
builder.Services.AddHttpClient<VectorsService>(client =>
{
    client.BaseAddress = new Uri(builder.Configuration["BaseAddress"]);
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error", createScopeForErrors: true);
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();

app.UseAntiforgery();

app.MapControllers();
app.MapStaticAssets();
app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode();

app.Run();
