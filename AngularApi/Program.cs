using System.ComponentModel;
using AngularApi.MylogicService_group;
using AngularApi.MylogicService_group.home;
using AngularApi.MylogicService_group.users.Album.album_upload;
using AngularApi.MylogicService_group.users.Album.show_albums;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Options;

var builder = WebApplication.CreateBuilder(args);


// --- 1. 服務註冊區 (Services Configuration) ---
// 註冊 CORS
builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(policy => policy.AllowAnyOrigin().AllowAnyMethod().AllowAnyHeader());
});

// 註冊 MySettings (一定要在這裡註冊，MapGet 才能用)
builder.Services.Configure<MySettings>(builder.Configuration.GetSection("MySettings"));
builder.Services.Configure<FileControl>(builder.Configuration.GetSection("FileControl"));

// 註冊你的自定義 Service (這樣 MySample1 才能注入)
builder.Services.AddScoped<MyLogicService>();
builder.Services.AddScoped<header_class>();
builder.Services.AddScoped<album_upload>();
builder.Services.AddScoped<show_albums>();
var app = builder.Build();

// --- 2. 中間層設定區 (Middleware) ---
app.UseCors();

// 讀取連線字串
string connectionString = builder.Configuration.GetConnectionString("DefaultConnection");

// --- 3. 路由定義區 (Endpoints) ---

// 第一個 API: 資料庫測試
app.MapGet("/api/data", async () =>
{
    var results = new List<object>();
    using var conn = new SqlConnection(connectionString);
    await conn.OpenAsync();
    using var cmd = new SqlCommand("SELECT TOP 10 * FROM YourTable", conn); // 建議加上 TOP 10 測試
    using var reader = await cmd.ExecuteReaderAsync();
    while (await reader.ReadAsync())
    {
        results.Add(new { Id = reader[0], Name = reader[1]?.ToString() });
    }
    return Results.Ok(results);
});

// 第二個 API: 測試讀取 Settings
app.MapGet("/api/ApiTest", (IOptions<MySettings> settings) =>
{
    var name = settings.Value.SystemName;
    return Results.Ok($"API 正常運作 - {name}");
});

// 第三個 API: 呼叫外部 Class
app.MapGet("/api/MySample1", (MyLogicService myService) =>
{
    var result = myService.ExecuteSpecialTask();
    return Results.Ok(new { message = result });
});

// Menu表單
app.MapPost("/api/headMenu", (header_class headerService) =>
{
    var result = headerService.header_MenuAsync();
    return Results.Ok(new { message = result });
});

// 相簿群組表單
app.MapPost("/api/albumGroup", (show_albums albumsService) =>
{
    var result = albumsService.album_Group();
    return Results.Ok(new { message = result });
});

// 照片上傳
app.MapPost("/api/fileUpload", async (HttpRequest request, IConfiguration config, IOptions<FileControl> SettingsBindableAttribute, album_upload albumService) =>
{
    var form = await request.ReadFormAsync();
    string functionName = form["func"].ToString();

    switch (functionName)
    {
        case "AlbumUpload":
            var result_AlbumUpload = albumService.UploadFile(request);
            return Results.Ok(new { message = result_AlbumUpload });
        case "AlbumMultipleUpload":
            var result_AlbumMultipleUpload = albumService.uploadMultipleFiles(request);
            return Results.Ok(new { message = result_AlbumMultipleUpload });
        default:
            return Results.BadRequest(new { message = "無此功能" });
    }

});

//// 需要前端參數
//app.MapPost("/api/headMenu", (header_class headerService, MyRequestModel request) => { ... });
app.Run();

// --- 4. 類別定義 (放在檔案最下方) ---
public class MySettings
{
    public string SystemName { get; set; } = string.Empty;
    public string AdminEmail { get; set; } = string.Empty;
    public int MaxRetryAttempts { get; set; }
}

public class FileControl
{
    public string AlbumUploadRootPath { get; set; } = string.Empty;
}