using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using MvcCoreLinqToSql.Models;
using System.Data;

namespace MvcCoreLinqToSql.Repositories
{
    public class RepositoryEnfermos : Controller
    {
        private DataTable tableEnfermos;
        private SqlConnection cn;
        private SqlCommand com;

        public RepositoryEnfermos()
        {
            string connectionString = @"Data Source=LOCALHOST\DEVELOPER;Initial Catalog=HOSPITAL;Persist Security Info=True;User ID=SA;Encrypt=True;Trust Server Certificate=True";
            this.cn = new SqlConnection(connectionString);
            this.com = new SqlCommand();
            this.com.Connection = this.cn;
            string sql = "select * from enfermo";
            SqlDataAdapter ad = new SqlDataAdapter(sql, this.cn);
            this.tableEnfermos = new DataTable();
            ad.Fill(this.tableEnfermos);
        }

        public List<Enfermo> GetEnfermos()
        {
            var consulta = from datos in this.tableEnfermos.AsEnumerable() select datos;
            List<Enfermo> enfermos = new List<Enfermo>();
            foreach(var row in consulta)
            {
                Enfermo enf = new Enfermo
                {
                    Inscripcion = row.Field<string>("inscripcion"),
                    Apellido = row.Field<string>("apellido"),
                    Direccion = row.Field<string>("direccion"),
                    FechaNacimiento = row.Field<DateTime>("FECHA_NAC"),
                    S = row.Field<string>("S"),
                    NSS = row.Field<string>("NSS")
                };
                enfermos.Add(enf);
            }
            return enfermos;
        }

        public Enfermo FindEnfermo(string inscripcion)
        {
            var consulta = from datos in this.tableEnfermos.AsEnumerable()
                           where datos.Field<string>("inscripcion") == inscripcion
                           select datos;
            var row = consulta.First();
            Enfermo enfermo = new Enfermo();
            enfermo.Inscripcion = row.Field<string>("inscripcion");
            enfermo.Apellido = row.Field<string>("apellido");
            enfermo.Direccion = row.Field<string>("direccion");
            enfermo.FechaNacimiento = row.Field<DateTime>("fecha_nac");
            enfermo.S = row.Field<string>("S");
            enfermo.NSS = row.Field<string>("NSS");
            return enfermo;
        }

        public async Task DeleteEmpleado(string inscripcion)
        {
            string sql = "delete from enfermo where inscripcion=@inscripcion";
            this.com.Parameters.AddWithValue("@inscripcion", inscripcion);
            this.com.CommandType = CommandType.Text;
            this.com.CommandText = sql;
            await this.cn.OpenAsync();
            await this.com.ExecuteNonQueryAsync();
            await this.cn.CloseAsync();
        }
    }
}
