using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Swashbuckle.AspNetCore.SwaggerGen;
using Swashbuckle.AspNetCore.SwaggerUI;

namespace source_oracle
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Adiciona os servi�os necess�rios para controllers
            builder.Services.AddControllers();

            // Configura o Swagger para gerar a documenta��o da API
            builder.Services.AddEndpointsApiExplorer();

            // Configura��o adicional do Swagger (com t�tulo, vers�o e descri��o)
            builder.Services.AddSwaggerGen(options =>
            {
                options.SwaggerDoc("v1", new Microsoft.OpenApi.Models.OpenApiInfo
                {
                    Title = "API de Planos",
                    Version = "v1",
                    Description = "API para consultar planos e suas tabelas de pre�o"
                });
            });

            var app = builder.Build();

            // Configura o pipeline de requisi��es HTTP
            if (app.Environment.IsDevelopment())
            {
                // Ativa o Swagger no ambiente de desenvolvimento
                app.UseSwagger();

                // Configura o Swagger UI para ser acessado na raiz da aplica��o
                app.UseSwaggerUI(c =>
                {
                    // Define o ponto de entrada para a documenta��o do Swagger
                    c.SwaggerEndpoint("/swagger/v1/swagger.json", "API de Planos v1");

                    // A URL do Swagger UI ser� a raiz
                    c.RoutePrefix = string.Empty;  // Swagger UI estar� acess�vel na raiz
                });
            }

            // Outros middlewares
            app.UseHttpsRedirection();
            app.UseAuthorization();
            app.MapControllers();

            app.Run();
        }
    }
}
