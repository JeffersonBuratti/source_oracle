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

            // Adiciona os serviços necessários para controllers
            builder.Services.AddControllers();

            // Configura o Swagger para gerar a documentação da API
            builder.Services.AddEndpointsApiExplorer();

            // Configuração adicional do Swagger (com título, versão e descrição)
            builder.Services.AddSwaggerGen(options =>
            {
                options.SwaggerDoc("v1", new Microsoft.OpenApi.Models.OpenApiInfo
                {
                    Title = "API de Planos",
                    Version = "v1",
                    Description = "API para consultar planos e suas tabelas de preço"
                });
            });

            var app = builder.Build();

            // Configura o pipeline de requisições HTTP
            if (app.Environment.IsDevelopment())
            {
                // Ativa o Swagger no ambiente de desenvolvimento
                app.UseSwagger();

                // Configura o Swagger UI para ser acessado na raiz da aplicação
                app.UseSwaggerUI(c =>
                {
                    // Define o ponto de entrada para a documentação do Swagger
                    c.SwaggerEndpoint("/swagger/v1/swagger.json", "API de Planos v1");

                    // A URL do Swagger UI será a raiz
                    c.RoutePrefix = string.Empty;  // Swagger UI estará acessível na raiz
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
