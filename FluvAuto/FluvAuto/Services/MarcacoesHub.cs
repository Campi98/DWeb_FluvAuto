using FluvAuto.Data;
using FluvAuto.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;

namespace FluvAuto.Services
{
   /// <summary>
   /// Gestor do Signal R, para notificações de marcações em tempo real
   /// </summary>
   public class MarcacoesHub : Hub
   {
      private readonly ApplicationDbContext _context;

      public MarcacoesHub(ApplicationDbContext context)
      {
         _context = context;
      }

      /// <summary>
      /// quando há uma ligação ao Signal R, este é o PRIMEIRO método a ser executado
      /// </summary>
      /// <returns></returns>
      public override async Task OnConnectedAsync()
      {
         var userName = Context.User?.Identity?.Name;
         var connectionId = Context.ConnectionId;

         // Adicionar conexão a um grupo específico do utilizador para garantir que todas as conexões recebem notificações
         if (!string.IsNullOrEmpty(userName))
         {
            var userGroupName = $"User_{userName}";
            await Groups.AddToGroupAsync(connectionId, userGroupName);
         }

         await base.OnConnectedAsync();
      }

      /// <summary>
      /// Notificar utilizador específico sobre uma nova marcação
      /// </summary>
      /// <param name="marcacaoId">ID da marcação criada</param>
      /// <param name="userName">Nome do utilizador a notificar</param>
      [Authorize]
      public async Task NotificarNovaMarcacao(int marcacaoId, string userName)
      {
         try
         {
            var marcacao = await _context.Marcacoes
               .Include(m => m.Viatura)
               .ThenInclude(v => v!.Cliente)
               .FirstOrDefaultAsync(m => m.MarcacaoId == marcacaoId);

            if (marcacao != null)
            {
               var dadosMarcacao = new
               {
                  Id = marcacao.MarcacaoId,
                  Data = marcacao.DataPrevistaInicioServico.ToString("yyyy-MM-dd"),
                  Hora = marcacao.DataPrevistaInicioServico.ToString("HH:mm"),
                  Servico = marcacao.Servico,
                  Estado = marcacao.Estado,
                  Cliente = marcacao.Viatura?.Cliente?.Nome ?? "Cliente não definido"
               };

               // Notificar TODAS as conexões do utilizador específico usando SignalR padrão
               await Clients.User(userName).SendAsync("NovaMarcacaoAdicionada", dadosMarcacao);
            }
         }
         catch (Exception ex)
         {
            // Erro na notificação - continuar sem interromper
         }
      }

      /// <summary>
      /// Atualizar estado de uma marcação e notificar utilizadores
      /// </summary>
      /// <param name="marcacaoId">ID da marcação</param>
      /// <param name="novoEstado">Novo estado da marcação</param>
      [Authorize]
      public async Task AtualizarEstadoMarcacao(int marcacaoId, string novoEstado)
      {
         try
         {
            var marcacao = await _context.Marcacoes.FindAsync(marcacaoId);
            if (marcacao != null)
            {
               marcacao.Estado = novoEstado;
               await _context.SaveChangesAsync();

               // Notificar todos os utilizadores sobre a mudança
               await Clients.All.SendAsync("EstadoMarcacaoAtualizado", new
               {
                  Id = marcacaoId,
                  NovoEstado = novoEstado
               });
            }
         }
         catch (Exception ex)
         {
            // Erro na atualização - continuar sem interromper
         }
      }

      /// <summary>
      /// Executado quando um utilizador se desconecta
      /// </summary>
      public override async Task OnDisconnectedAsync(Exception? exception)
      {
         var userName = Context.User?.Identity?.Name;
         var connectionId = Context.ConnectionId;
         
         // Remover conexão do grupo do utilizador
         if (!string.IsNullOrEmpty(userName))
         {
            var userGroupName = $"User_{userName}";
            await Groups.RemoveFromGroupAsync(connectionId, userGroupName);
         }
         
         await base.OnDisconnectedAsync(exception);
      }
   }

   /// <summary>
   /// Extensões para IHubContext para facilitar notificações customizadas
   /// </summary>
   public static class HubContextExtensions
   {
      /// <summary>
      /// Notificar todas as conexões de um utilizador específico usando grupos
      /// </summary>
      public static async Task NotificarTodasConexoesUtilizador(this IHubContext<MarcacoesHub> hubContext, 
         string userName, string method, object data)
      {
         // Usar grupos para garantir que todas as conexões do utilizador recebem a notificação
         var userGroupName = $"User_{userName}";
         await hubContext.Clients.Group(userGroupName).SendAsync(method, data);
         
         // Também tentar o método padrão como fallback
         await hubContext.Clients.User(userName).SendAsync(method, data);
      }
   }
}
