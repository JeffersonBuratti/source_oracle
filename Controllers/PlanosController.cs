using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Oracle.ManagedDataAccess.Client;
using Oracle_Consummer.Models;
using System.Collections.Generic;

namespace Oracle_Consummer.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PlanosController : ControllerBase
    {
        private readonly string _connectionString;

        // Injeção de dependência para o IConfiguration, para ler o arquivo appsettings.json
        public PlanosController(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("OracleDbConnection"); // Lê a string de conexão
        }

        [HttpGet]
        public IActionResult GetPlanos()
        {
            try
            {
                var planos = new List<Plano>(); // Lista de planos

                using (var conn = new OracleConnection(_connectionString))
                {
                    conn.Open();

                    // Consulta para obter todos os planos
                    using (var cmd = new OracleCommand("SELECT NNUMEPLAN, CCODIPLAN, CDESCPLAN FROM PLANOS_DISPONIVEIS", conn))
                    {
                        using (var reader = cmd.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                planos.Add(new Plano
                                {
                                    Nnumeplan = reader["NNUMEPLAN"].ToString(),
                                    Ccodiplan = reader["CCODIPLAN"].ToString(),
                                    Cdescplan = reader["CDESCPLAN"].ToString()
                                });
                            }
                        }
                    }
                }

                return Ok(planos); // Retorna os planos com seus dados
            }
            catch (OracleException ex)
            {
                return StatusCode(500, $"Erro ao acessar o banco de dados: {ex.Message}");
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Erro inesperado: {ex.Message}");
            }
        }
    }
}
