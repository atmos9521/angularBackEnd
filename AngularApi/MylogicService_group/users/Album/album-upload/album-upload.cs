using System;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Options;

namespace AngularApi.MylogicService_group.users.Album.album_upload;

public class album_upload
{
    private readonly string _connectionString;
    private readonly string _albumUploadRootPath;

    public album_upload(IConfiguration configuration, IOptions<FileControl> config_FileControl)
    {
        // Program.cs記得要 註冊你的自定義 Service!!!! 參考 builder.Services.AddScoped<MyLogicService>();
        _connectionString = configuration.GetConnectionString("DefaultConnection") ?? "";
        _albumUploadRootPath = config_FileControl.Value.AlbumUploadRootPath ?? "";
    }

    /// <summary>
    /// 單筆檔案上傳
    /// </summary>
    /// <param name="request"></param>
    /// <returns></returns>
    public async Task<IResult> UploadFile(HttpRequest request)
    {
        // 1. 檢查是否為 Multipart 表單
        if (!request.HasFormContentType) return Results.BadRequest("格式錯誤");

        var form = await request.ReadFormAsync();
        var file = form.Files.GetFile("file"); // 這裡的 "file" 要跟 Angular FormData 的 Key 一致

        if (file == null || file.Length == 0) return Results.BadRequest("請選擇檔案");

        // 2. 處理檔案流
        using var ms = new MemoryStream();
        await file.CopyToAsync(ms);
        byte[] fileData = ms.ToArray();

        // 3. 檔案存入 專案下 Album資料夾  (設定存檔路徑 (專案根目錄下的 Album 資料夾))
        // path: D:\willy_test_case\MyBackend\AngularApi\Album
        // 確保資料夾存在
        if (!Directory.Exists(_albumUploadRootPath))
        {
            Directory.CreateDirectory(_albumUploadRootPath);
        }
        // 處理檔名 (避免特殊字元或重複，建議可以加上時間戳記)
        string fileName = Path.GetFileName(file.FileName);
        string fullPath = Path.Combine(_albumUploadRootPath, fileName);
        // 存入檔案系統
        using (var stream = new FileStream(fullPath, FileMode.Create))
        {
            await file.CopyToAsync(stream);
        }

        // 4. 寫入資料庫
        // using (var conn = new SqlConnection(_connectionString))
        // {
        //     await conn.OpenAsync();
        //     var sql = "INSERT INTO Sys_FileUploads (File_Name, Content_Type, File_Data) VALUES (@name, @type, @data)";

        //     using (var cmd = new SqlCommand(sql, conn))
        //     {
        //         cmd.Parameters.AddWithValue("@name", file.FileName);
        //         cmd.Parameters.AddWithValue("@type", file.ContentType);
        //         // 存入 byte[]，SqlClient 會自動對應到 SQL 的 VARBINARY(MAX)
        //         cmd.Parameters.AddWithValue("@data", fileData);

        //         // await cmd.ExecuteNonQueryAsync();
        //     }
        // }

        return Results.Ok(new { message = "檔案上傳成功", fileName = file.FileName });
    }

    /// <summary>
    /// 多筆檔案上傳
    /// </summary>
    /// <param name="request"></param>
    /// <returns></returns>
    public async Task<IResult> uploadMultipleFiles(HttpRequest request)
    {
        var form = await request.ReadFormAsync();
        var files = form.Files; // 取得所有上傳的檔案

        if (files == null || files.Count == 0) return Results.BadRequest(new { message = "未選取任何檔案" });

        foreach (var file in files)
        {
            if (file.Length > 0)
            {
                // 確保資料夾存在
                if (!Directory.Exists(_albumUploadRootPath))
                {
                    Directory.CreateDirectory(_albumUploadRootPath);
                }

                // 產生存放路徑
                string fullPath = Path.Combine(_albumUploadRootPath, file.FileName);

                using (var stream = new FileStream(fullPath, FileMode.Create))
                {
                    await file.CopyToAsync(stream);
                }
            }
        }

        return Results.Ok(new { message = $"成功上傳 {files.Count} 個檔案" });
    }
}
