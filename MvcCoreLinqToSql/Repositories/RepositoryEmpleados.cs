using Microsoft.Data.SqlClient;
using MvcCoreLinqToSql.Models;
using System.Data;
using System.Security.Cryptography;

namespace MvcCoreLinqToSql.Repositories
{
    public class RepositoryEmpleados
    {
        //SOLO TENDREMOS UNA TABLA A NIVEL DE CLASE
        private DataTable tablaEmpleados;
        public RepositoryEmpleados()
        {
            string connectionString = @"Data Source=LOCALHOST\DEVELOPER;Initial Catalog=HOSPITAL;Persist Security Info=True;User ID=SA;Encrypt=True;Trust Server Certificate=True";
            string sql = "select * from emp";
            //CREAMOS EL ADAPTADOR PUENTE ENTRE SQL SERVER Y LINQ
            SqlDataAdapter ad = new SqlDataAdapter(sql,connectionString);
            this.tablaEmpleados = new DataTable();
            //TRAEMOS LOS DATOS PARA LINQ
            ad.Fill(this.tablaEmpleados);
        }

        //METODO PARA RECUPERAR TODOS LOS EMPLEADOS
        public List<Empleado> GetEmpleados()
        {
            //LOS CONSULTAS SE ALMACENAN EN GENERICOS
            var consulta = from datos in this.tablaEmpleados.AsEnumerable() select datos;
            //AHORA MISMO TENEMOS DENTRO DE CONSULTA LA INFORMACION
            //DE LOS EMPLEADOS
            //LOS DATOS VIENEN EN FORMATO TABLA, CADA ELEMENTO
            //DE UNA TABLA ES UNA FILA. (DaraRow)
            //DEBEMOS RECORRER LAS FILAS, EXTRAERLAS Y CONVERTIRLAS
            //A NUESTRO MODEL Empleado
            List<Empleado> empleados = new List<Empleado>();
            //RECORREMOS CADA FILA DE LA CONSULTA
            foreach (var row in consulta)
            {
                //PARA EXTRAER DATOS DE UN DataRow
                //DataRow.Fill<tipodata>("COLUMNA")
                Empleado emp = new Empleado();
                emp.IdEmpleado = row.Field<int>("EMP_NO");
                emp.Apellido = row.Field<string>("APELLIDO");
                emp.Oficio = row.Field<string>("OFICIO");
                emp.Salario = row.Field<int>("SALARIO");
                emp.IdDepartamento = row.Field<int>("DEPT_NO");
                empleados.Add(emp);
            }
            return empleados;
        }
        public Empleado FindEmpleado(int idEmpleado)
        {
            var consulta = from datos in this.tablaEmpleados.AsEnumerable() 
                           where datos.Field<int>("emp_no") == idEmpleado 
                           select datos;
            //NOSOTROS SABEMOS QUE ESTA CONSULTA DEVUELVE UNA FILA
            //Linq SIEMPRE DEVUELVE UN CONJUNTO
            //DENTRO DE ESTE CONJUNTO TENEMOS METODOS LAMBDA
            //PARA HACER CONSULTAS
            //POR EJEMPLO, PODRIAMOS CONTAR, PODRIAMOS SABER EL MAXIMO O RECUPERAR EL PRIMER ELEMENTO DEL CONJUNTO
            var row = consulta.First();
            Empleado empleado = new Empleado();
            empleado.IdEmpleado = row.Field<int>("emp_no");
            empleado.Apellido = row.Field<string>("APELLIDO");
            empleado.Oficio = row.Field<string>("OFICIO");
            empleado.Salario = row.Field<int>("SALARIO");
            empleado.IdDepartamento = row.Field<int>("DEPT_NO");
            return empleado;
        }

        public List<Empleado> GetEmpleadoOficioSalario(string oficio, int salario)
        {
            var consulta = from datos in this.tablaEmpleados.AsEnumerable()
                           where datos.Field<string>("OFICIO") == oficio && datos.Field<int>("SALARIO") >= salario select datos;
            if (consulta.Count() == 0)
            {
                //SIEMPRE DEVOLVEMOS NULL
                return null;
            }
            else
            {
                List<Empleado> empleados = new List<Empleado>();
                foreach (var row in consulta)
                {
                    Empleado emp = new Empleado
                    {
                        IdEmpleado = row.Field<int>("EMP_NO"),
                        Apellido = row.Field<string>("APELLIDO"),
                        Oficio = row.Field<string>("OFICIO"),
                        Salario = row.Field<int>("salario"),
                        IdDepartamento = row.Field<int>("dept_no")
                    };
                    empleados.Add(emp);
                }

                return empleados;
            }
        }
         
        public ResumenEmpleados GetEmpleadosOficio(string oficio)
        {
            var consulta = from datos in this.tablaEmpleados.AsEnumerable()
                           where datos.Field<string>("oficio") == oficio
                           //orderby datos.Field<string>("oficio")
                           select datos;
            //SI NO EXISTEN REGISTROS, DEBEMOS CONTROLARLO
            //QUIERO ORDENAR EMPLEADOS POR SU SALARIO
            if (consulta.Count() == 0)
            {
                //VALORES NEUTROS
                ResumenEmpleados model = new ResumenEmpleados();
                model.Empleados = null;
                model.MaximoSalario = 0;
                model.MediaSalarial = 0;
                model.Personas = 0;
                return model;
            }
            else
            {
                consulta = consulta.OrderBy(z => z.Field<int>("salario"));
                int personas = consulta.Count();
                int maximo = consulta.Max(x => x.Field<int>("salario"));
                double media = consulta.Average(x => x.Field<int>("salario"));
                List<Empleado> empleados = new List<Empleado>();
                foreach (var row in consulta)
                {
                    Empleado emp = new Empleado
                    {
                        IdEmpleado = row.Field<int>("EMP_NO"),
                        Apellido = row.Field<string>("APELLIDO"),
                        Oficio = row.Field<string>("OFICIO"),
                        Salario = row.Field<int>("salario"),
                        IdDepartamento = row.Field<int>("dept_no")
                    };
                    empleados.Add(emp);
                }
                ResumenEmpleados model = new ResumenEmpleados();
                model.Empleados = empleados;
                model.MaximoSalario = maximo;
                model.MediaSalarial = media;
                model.Personas = personas;
                return model;
            }
              
        }

        public List<string> GetOficios()
        {
            var consulta = (from datos in this.tablaEmpleados.AsEnumerable() 
                           select datos.Field<string>("oficio")).Distinct();
            //AHORA MISMO YA TENEMOS LO QUE NECESITAMOS, UN CONJUNTO DE STRING
            //LA NORMAL SUELE SER DEVOLVER LA COLECCION GENERICA List<T>
            return consulta.ToList();
        }
    }
}
