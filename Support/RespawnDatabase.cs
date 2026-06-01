using System;
using System.Threading.Tasks;
using Microsoft.Data.SqlClient;
using Respawn;

namespace KDPM_ReqnrollSeleniumAutomationTesting_Nhom5.Support
{
    internal class TestDatabaseCheckpoint
    {
    private const string ConnectionString = @"Data Source=(LocalDB)\MSSQLLocalDB;AttachDbFilename=C:\Users\Minh\Desktop\PublishFolder\App_Data\NHA_THUOC_CNPM.mdf;Integrated Security=True;";

        private Respawner _respawner;

        public async Task InitializeCheckpointAsync()
        {
            if (_respawner != null) return;
            using var connection = new SqlConnection(ConnectionString);
            await connection.OpenAsync();

            // Cấu hình các bảng muốn giữ lại hoặc bỏ qua khi reset
            _respawner = await Respawner.CreateAsync(connection, new RespawnerOptions
            {
                TablesToIgnore = new Respawn.Graph.Table[]
                {
                "DANH_MUC_THUOC",
                "DIA_DIEM_NHA_THUOC",
                "LO_THUOC",
                "LOAI_THUOC",
                "PHAN_QUYEN",
                "PHUONG_XA",
                "QUAN_HUYEN",
                "THUOC",
                "TINH_THANH",
                },
                SchemasToInclude = new[] { "dbo" }
            });
        }

        public async Task ResetDatabaseAsync()
        {
            if (_respawner == null)
            {
                await InitializeCheckpointAsync();
            }

            using var connection = new SqlConnection(ConnectionString);
            await connection.OpenAsync();

            await _respawner.ResetAsync(connection);
        }
    }
}


