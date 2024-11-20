using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;  // Para ler a configuração do appsettings.json
using Oracle.ManagedDataAccess.Client;
using Oracle_Consummer.Models;  // Importando o namespace onde as classes TabelaPreco e FaixaEtaria estão definidas
using System.Collections.Generic;
using System;

namespace Oracle_Consummer.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TabelaController : ControllerBase
    {
        private readonly string _connectionString;

        // Injeção de dependência para o IConfiguration, para ler a string de conexão do appsettings.json
        public TabelaController(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("OracleDbConnection"); // Lê a string de conexão configurada no appsettings.json
        }

        // Método para obter as tabelas e faixas etárias de um plano específico
        [HttpGet("{nnumEplan}")]
        public IActionResult GetTabelasDePreco(int nnumEplan)
        {
            try
            {
                var tabelas = new List<TabelaPreco>(); // Lista de tabelas de preço associadas a um plano

                using (var conn = new OracleConnection(_connectionString))
                {
                    conn.Open();

                    // Consulta SQL para obter as tabelas e faixas etárias de um plano específico
                    using (var cmd = new OracleCommand(@"
                        SELECT 
                            F.NNUMETPLA,
                            P.CDESCTPLA,
                            F.NNUMEFETA,
                            F.NMINIFETA,
                            F.NMAXIFETA,
                            M.NVALOMANU
                        FROM 
                            hssfeta F
                        JOIN 
                            hsstpla P ON F.NNUMETPLA = P.NNUMETPLA
                        JOIN 
                            hssmanu M ON F.NNUMEFETA = M.NNUMEFETA
                        WHERE 
                            F.NNUMEPLAN = :nnumEplan
                        ORDER BY 
                            F.NNUMETPLA, F.NNUMEFETA", conn))
                    {
                        // Adiciona o parâmetro para a consulta
                        cmd.Parameters.Add(":nnumEplan", OracleDbType.Int32).Value = nnumEplan;

                        using (var reader = cmd.ExecuteReader())
                        {
                            TabelaPreco tabelaAtual = null;

                            while (reader.Read())
                            {
                                // Verifica se a tabela já existe na lista
                                if (tabelaAtual == null || tabelaAtual.Nnumetpla != reader["NNUMETPLA"].ToString())
                                {
                                    if (tabelaAtual != null)
                                    {
                                        tabelas.Add(tabelaAtual); // Adiciona a tabela anterior à lista
                                    }

                                    // Cria uma nova tabela
                                    tabelaAtual = new TabelaPreco
                                    {
                                        Nnumetpla = reader["NNUMETPLA"].ToString(),
                                        CdescTpla = reader["CDESCTPLA"].ToString(),
                                        FaixasEtarias = new List<FaixaEtaria>()
                                    };
                                }

                                // Adiciona a faixa etária à tabela de preço
                                tabelaAtual.FaixasEtarias.Add(new FaixaEtaria
                                {
                                    Nnumefeta = reader["NNUMEFETA"].ToString(),
                                    Nminifeta = Convert.ToInt32(reader["NMINIFETA"]),
                                    Nmaxifeta = Convert.ToInt32(reader["NMAXIFETA"]),
                                    Nvalomanu = Convert.ToDecimal(reader["NVALOMANU"])
                                });
                            }

                            // Adiciona a última tabela
                            if (tabelaAtual != null)
                            {
                                tabelas.Add(tabelaAtual);
                            }
                        }
                    }
                }

                return Ok(tabelas); // Retorna as tabelas com faixas etárias
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Erro ao acessar o banco de dados: {ex.Message}");
            }
        }
    }
}
