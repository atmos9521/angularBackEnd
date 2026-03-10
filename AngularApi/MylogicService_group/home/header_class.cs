using System.Text;
using Microsoft.Data.SqlClient;

namespace AngularApi.MylogicService_group.home
{
    public class header_class
    {
        private readonly string _connectionString;

        public header_class(IConfiguration configuration)
        {
            // Program.cs記得要 註冊你的自定義 Service!!!! 參考 builder.Services.AddScoped<MyLogicService>();
            _connectionString = configuration.GetConnectionString("DefaultConnection") ?? "";
        }

        public async Task<List<SYS_MENU>> header_MenuAsync()
        {
            var SYS_MENU = new List<SYS_MENU>();
            try
            {
                // 使用標準 ADO.NET 寫法
                using (var conn = new SqlConnection(_connectionString))
                {
                    await conn.OpenAsync();
                    StringBuilder sql = new StringBuilder();
                    sql.AppendLine("SELECT");
                    sql.AppendLine(" Menu_Id");
                    sql.AppendLine(",Parent_Id");
                    sql.AppendLine(",Label");
                    sql.AppendLine(",Icon");
                    sql.AppendLine(",RouterLink");
                    sql.AppendLine(",Sort_Order");
                    sql.AppendLine("FROM Sys_Menus");
                    sql.AppendLine("WHERE Is_Active = 'Y'");

                    using (var cmd = new SqlCommand(sql.ToString(), conn))
                    {
                        // 2. 使用 Parameters.AddWithValue 對應變數與實際的值
                        // 這樣 SqlClient 會自動幫你處理跳脫字元，確保安全
                        //cmd.Parameters.AddWithValue("@productId", id);

                        using (var reader = await cmd.ExecuteReaderAsync())
                        {
                            while (await reader.ReadAsync())
                            {
                                SYS_MENU.Add(new SYS_MENU
                                {
                                    Menu_Id = reader.IsDBNull(0) ? 0 : reader.GetInt32(0),
                                    Parent_Id = reader.IsDBNull(1) ? 0 : reader.GetInt32(1),
                                    Label = reader.IsDBNull(2) ? "" : reader.GetString(2),
                                    Icon = reader.IsDBNull(3) ? "" : reader.GetString(3),
                                    RouterLink = reader.IsDBNull(4) ? "" : reader.GetString(4),
                                    Sort_Order = reader.IsDBNull(5) ? 0 : reader.GetInt32(5)
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


            return SYS_MENU;
        }
    }

    public class SYS_MENU
    {
        // 加上 ? 代表這是一個可以為 Null 的字串
        public int Menu_Id { get; set; }
        public int? Parent_Id { get; set; }
        public string? Label { get; set; } = string.Empty;
        public string? Icon { get; set; } = string.Empty;
        public string? RouterLink { get; set; } = string.Empty;
        public int? Sort_Order { get; set; } = 0;
    }
}
