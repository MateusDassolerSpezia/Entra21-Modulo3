using System.Collections.Generic;
using System.Threading;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Mvc;
using System.Linq;

// 1.
public class Usuário
{
    public string nome { get; set; }
    public string senha { get; set; }
}

public class Program
{
    // 2.
    private static List<Usuário> usuarios = new List<Usuário>();

    public static void Main(string[] args)
    {
        // 3. 
        var builder = WebApplication.CreateBuilder(args);
        var app = builder.Build();

        app.MapGet("/", () => "API de Usuários com más práticas. Por favor, use GET em vez de POST.");

        // 4. 
        app.MapGet("/adicionar", ([FromQuery] string nome, [FromQuery] string senha) =>
        {
            try
            {
                // 5. 
                if (string.IsNullOrEmpty(nome) || string.IsNullOrEmpty(senha))
                {
                    return Results.BadRequest("Nome e senha não podem ser nulos ou vazios.");
                }

                usuarios.Add(new Usuário { nome = nome, senha = senha });
                
                // 6. 
                Thread.Sleep(1000); 

                // 7. 
                return Results.Ok("Usuário adicionado!");
            }
            // 8. 
            catch (System.Exception ex)
            {
                // 9. 
                return Results.Ok($"Erro ao adicionar usuário! Mensagem: {ex.Message}");
            }
        });

        // 10. 
        app.MapGet("/remover", ([FromQuery] string nome) =>
        {
            var usuario = usuarios.FirstOrDefault(u => u.nome == nome);
            if (usuario != null)
            {
                usuarios.Remove(usuario);
                // 11.
                return Results.Ok("Usuário removido.");
            }
            // 12
            return Results.Ok("Usuário não encontrado.");
        });

        app.MapGet("/listar", () => usuarios);
        
        app.Run();
    }
}
