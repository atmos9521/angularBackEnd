using System;
using System.Text;
using Microsoft.Data.SqlClient;

namespace AngularApi.MylogicService_group.users.Album.show_albums;

public class show_albums
{
    private readonly string _connectionString;
    public show_albums(IConfiguration configuration)
    {
        // Program.cs記得要 註冊你的自定義 Service!!!! 參考 builder.Services.AddScoped<MyLogicService>();
        _connectionString = configuration.GetConnectionString("DefaultConnection") ?? "";
    }

    public async Task<List<albumGroup>> album_Group()
    {
        var albumGroup = new List<albumGroup>();
        try
        {
            // 使用標準 ADO.NET 寫法
            using (var conn = new SqlConnection(_connectionString))
            {
                await conn.OpenAsync();
                StringBuilder sql = new StringBuilder();
                sql.AppendLine("SELECT               ");
                sql.AppendLine(" Group_Id            ");
                sql.AppendLine(",Parent_Id           ");
                sql.AppendLine(",Group_Name          ");
                sql.AppendLine(",Icon                ");
                sql.AppendLine(",Sort_Order          ");
                sql.AppendLine("FROM Album_Group     ");
                sql.AppendLine("WHERE Is_Active = 'Y'");
                sql.AppendLine("ORDER BY Sort_Order  ");

                using (var cmd = new SqlCommand(sql.ToString(), conn))
                {
                    // 2. 使用 Parameters.AddWithValue 對應變數與實際的值
                    // 這樣 SqlClient 會自動幫你處理跳脫字元，確保安全
                    //cmd.Parameters.AddWithValue("@productId", id);

                    using (var reader = await cmd.ExecuteReaderAsync())
                    {
                        while (await reader.ReadAsync())
                        {
                            albumGroup.Add(new albumGroup
                            {
                                Group_Id = reader.IsDBNull(0) ? 0 : reader.GetInt32(0),
                                Parent_Id = reader.IsDBNull(1) ? 0 : reader.GetInt32(1),
                                Group_Name = reader.IsDBNull(2) ? "" : reader.GetString(2),
                                Icon = reader.IsDBNull(3) ? "" : reader.GetString(3),
                                Sort_Order = reader.IsDBNull(4) ? 0 : reader.GetInt32(4)
                            });
                        }
                    }
                }
            }
        }
        catch (System.Exception ex)
        {
            string err_msg = ex.Message;
        }


        return albumGroup;
    }
}
public class albumGroup
{
    // 加上 ? 代表這是一個可以為 Null 的字串
    public int Group_Id { get; set; }
    public int? Parent_Id { get; set; }
    public string? Icon { get; set; } = string.Empty;
    public string? Group_Name { get; set; } = string.Empty;
    public int? Sort_Order { get; set; } = 0;
}