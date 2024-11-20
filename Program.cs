using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.IdentityModel.Tokens;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using System.Text;

namespace source_oracle
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Adiciona os serviços necessários para controllers
            builder.Services.AddControllers();

            // Configura a autenticação JWT
            var key = Encoding.ASCII.GetBytes("SuaChaveSecretaAqui");  // Substitua pela sua chave secreta

            builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
                .AddJwtBearer(options =>
                {
                    options.TokenValidationParameters = new TokenValidationParameters
                    {
                        ValidateIssuer = true,
                        ValidateAudience = true,
                        ValidateLifetime = true,
                        ValidateIssuerSigningKey = true,
                        ValidIssuer = "SeuIssuer", // O issuer que você usa
                        ValidAudience = "SuaAudience", // A audiência do seu token
                        IssuerSigningKey = new SymmetricSecurityKey(key) // A chave secreta
                    };
                });

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

                // Adiciona a documentação do JWT ao Swagger (opcional)
                options.AddSecurityDefinition("Bearer", new Microsoft.OpenApi.Models.OpenApiSecurityScheme
                {
                    In = Microsoft.OpenApi.Models.ParameterLocation.Header,
                    Name = "Authorization",
                    Type = Microsoft.OpenApi.Models.SecuritySchemeType.ApiKey,
                    Scheme = "Bearer",
                    Description = "Utilize o token JWT"
                });
                options.AddSecurityRequirement(new Microsoft.OpenApi.Models.OpenApiSecurityRequirement
                {
                    {
                        new Microsoft.OpenApi.Models.OpenApiSecurityScheme
                        {
                            Reference = new Microsoft.OpenApi.Models.OpenApiReference
                            {
                                Type = Microsoft.OpenApi.Models.ReferenceType.SecurityScheme,
                                Id = "Bearer"
                            }
                        },
                        new string[] {}
                    }
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

            // Ativa a autenticação
            app.UseAuthentication();  // Habilita a autenticação (deve vir antes de UseAuthorization)

            // Outros middlewares
            app.UseHttpsRedirection();
            app.UseAuthorization();  // Habilita a autorização
            app.MapControllers();

            app.Run();
        }
    }
}
