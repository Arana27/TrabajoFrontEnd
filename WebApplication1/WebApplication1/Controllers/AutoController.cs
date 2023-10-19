using Dapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Data;
using System.Data.SqlClient;
using WebApplication1.Models;

namespace WebApplication1.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AutoController : ControllerBase
    {
        //configurando el entorno para usar la cadena de coneccion , _config es la llave para usar la cadena de conexion
        private IConfiguration _Config;

        public AutoController(IConfiguration Config)
        {
            _Config = Config;
        }

        [HttpGet]
        public async Task<ActionResult<List<Auto>>> GetAllLAuto()
        {
            using var conexion = new SqlConnection(_Config.GetConnectionString("DefaultConnection"));
            conexion.Open();
            var oLibros = conexion.Query<Auto>("LeerAutos", commandType: System.Data.CommandType.StoredProcedure);
            return Ok(oLibros);
        }


        [HttpPost]
        // obteniendo el objeto de usuario de la informacion proporcionada por Swagger
        public async Task<ActionResult<Auto>> CreateAuto(Auto AU)
        {
            try
            {
                using var conexion = new SqlConnection(_Config.GetConnectionString("DefaultConnection"));
                conexion.Open();
                var parametro = new DynamicParameters();
                parametro.Add("@Marca", AU.Marca);
                parametro.Add("@Modelo", AU.Modelo);
                parametro.Add("@Precio", AU.Precio);

                var oLibro = conexion.Query<Auto>("CrearAuto", parametro, commandType: System.Data.CommandType.StoredProcedure);

                // Verificar si la operación fue exitosa (por ejemplo, si oLibro no es nulo)
                if (oLibro != null)
                {

                    var mensaje = "Auto creado exitosamente.";
                    return Ok(mensaje);
                }
                else
                {

                    var mensaje = "No se pudo crear el nuevo Auto.";
                    return BadRequest(new { mensaje });
                }
            }
            catch (Exception ex)
            {

                var mensaje = "Se produjo un error al crear el Auto: " + ex.Message;
                return StatusCode(500, new { mensaje });
            }
        }

        [HttpDelete("{ID}")]
        // obteniendo id del libro a eliminar (este id es de la clase Libros)
        public async Task<ActionResult> DeleteAuto(int ID)
        {   
            //manejo de errores con trycach
            try
            {
                using (var conexion = new SqlConnection(_Config.GetConnectionString("DefaultConnection")))
                {
                    await conexion.OpenAsync();

                    var parametro = new DynamicParameters();
                    parametro.Add("@ID", ID);
                    await conexion.ExecuteAsync("EliminarAuto", parametro, commandType: CommandType.StoredProcedure);

                    return Ok("Auto eliminado correctamente.");
                }
            }
            catch (Exception ex)
            {

                return StatusCode(500, $"Error al eliminar el Auto: {ex.Message}");
            }
        }
    }
}
