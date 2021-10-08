using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MotorSQL.Data;
using MotorSQL.Models;
using Microsoft.Extensions.Configuration;
using System.Data;
using Microsoft.Data.SqlClient;

namespace MotorSQL.DB
{
    public class SQL
    {
        //private readonly ApplicationDbContext _context;
        private readonly string _connectionString;

        public SQL(string connectionString)
        {
            //_context = context;
            _connectionString = connectionString;
        }

        public DataSet ConsultaGeneral(PeticionSQL consulta, ref MensajeRespuesta respuesta)
        {
            DataSet ds = new DataSet();
            using (SqlConnection conn = new SqlConnection(_connectionString))
            {
                conn.Open();
                using (SqlCommand cmd = new SqlCommand("SP_ConsultaSQL", conn))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.Add(new SqlParameter() { ParameterName = "@e_script", SqlDbType = SqlDbType.VarChar, SqlValue = consulta.Script, Direction = ParameterDirection.Input });
                    cmd.Parameters.Add(new SqlParameter() { ParameterName = "@s_codigo", SqlDbType = SqlDbType.Int, SqlValue = 0, Direction = ParameterDirection.InputOutput });
                    cmd.Parameters.Add(new SqlParameter() { ParameterName = "@s_mensaje", SqlDbType = SqlDbType.VarChar, Size = 250, SqlValue = "", Direction = ParameterDirection.InputOutput });

                    using (SqlDataAdapter da = new SqlDataAdapter(cmd))
                    {
                        da.Fill(ds);
                        respuesta.Codigo = int.Parse(cmd.Parameters["@s_codigo"].Value.ToString().Trim());
                        respuesta.Mensaje = cmd.Parameters["@s_mensaje"].Value.ToString().Trim();
                        respuesta.CantidadTablas = ds.Tables.Count;
                        conn.Close();
                    }
                }
            }
            return ds;
        }

    }
}
