using System;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Threading.Tasks;

namespace MVCTemplete.Helpers
{
    public class DBHelper : IDisposable
    {
        private readonly string _connectionString;

        public DBHelper()
        {
            _connectionString = ConfigurationManager.ConnectionStrings["SqlConnection"].ConnectionString;
        }

        private SqlConnection GetConnection()
        {
            return new SqlConnection(_connectionString);
        }

        #region Execute Non Query (Insert/Update/Delete)

        public async Task<int> ExecuteAsync(string procedureName, params SqlParameter[] parameters)
        {
            using (SqlConnection con = GetConnection())
            using (SqlCommand cmd = new SqlCommand(procedureName, con))
            {
                cmd.CommandType = CommandType.StoredProcedure;

                if (parameters != null)
                    cmd.Parameters.AddRange(parameters);

                await con.OpenAsync();

                return await cmd.ExecuteNonQueryAsync();
            }
        }

        #endregion

        #region Execute Scalar

        public async Task<object> ExecuteScalarAsync(string procedureName, params SqlParameter[] parameters)
        {
            using (SqlConnection con = GetConnection())
            using (SqlCommand cmd = new SqlCommand(procedureName, con))
            {
                cmd.CommandType = CommandType.StoredProcedure;

                if (parameters != null)
                    cmd.Parameters.AddRange(parameters);

                await con.OpenAsync();

                return await cmd.ExecuteScalarAsync();
            }
        }

        #endregion

        #region Get DataTable

        public async Task<DataTable> GetDataTableAsync(string procedureName, params SqlParameter[] parameters)
        {
            DataTable dt = new DataTable();

            using (SqlConnection con = GetConnection())
            using (SqlCommand cmd = new SqlCommand(procedureName, con))
            {
                cmd.CommandType = CommandType.StoredProcedure;

                if (parameters != null)
                    cmd.Parameters.AddRange(parameters);

                await con.OpenAsync();

                using (SqlDataReader dr = await cmd.ExecuteReaderAsync())
                {
                    dt.Load(dr);
                }
            }

            return dt;
        }

        #endregion

        #region Get DataSet

        public async Task<DataSet> GetDataSetAsync(string procedureName, params SqlParameter[] parameters)
        {
            DataSet ds = new DataSet();

            using (SqlConnection con = GetConnection())
            using (SqlCommand cmd = new SqlCommand(procedureName, con))
            using (SqlDataAdapter da = new SqlDataAdapter(cmd))
            {
                cmd.CommandType = CommandType.StoredProcedure;

                if (parameters != null)
                    cmd.Parameters.AddRange(parameters);

                await Task.Run(() => da.Fill(ds));
            }

            return ds;
        }

        #endregion

        #region Transaction

        public async Task<bool> ExecuteTransactionAsync(params SqlCommand[] commands)
        {
            using (SqlConnection con = GetConnection())
            {
                await con.OpenAsync();

                SqlTransaction tran = con.BeginTransaction();

                try
                {
                    foreach (var cmd in commands)
                    {
                        cmd.Connection = con;
                        cmd.Transaction = tran;

                        await cmd.ExecuteNonQueryAsync();
                    }

                    tran.Commit();
                    return true;
                }
                catch
                {
                    tran.Rollback();
                    throw;
                }
            }
        }

        #endregion

        public void Dispose()
        {
        }
    }
}