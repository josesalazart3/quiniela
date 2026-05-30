
using MailKit.Net.Smtp;
using MailKit.Security;
using MimeKit

namespace Quiniela.Services
{
    public interface IEmailService
    {
        Task SendInvitacionLigaAsync(string emailDestino, string nombreLiga, string token);
        Task SendAprobacionMiembroAsync(string emailDestino, string nombreLiga);
        Task SendRecuperacionPasswordAsync(string emailDestino, string token);
    }

    public class EmailService(IResend resend, IConfiguration config) : IEmailService
    {
        private readonly IResend _resend = resend;
        private readonly IConfiguration _config = config;

        private const string From = "Quiniela Mundial 2026 <noreply@uniondeprofesionales.com>"; //emaaaaaaaaaaaaaaaaaaaaaail

        private async Task SendEmailAsync(string emailDestino, string subject, string htmlBody)
        {
            var message = new EmailMessage
            {
                From = From,
                Subject = subject,
                HtmlBody = htmlBody
            };
            message.To.Add(emailDestino);

            await _resend.EmailSendAsync(message);
        }

        public async Task SendInvitacionLigaAsync(string emailDestino, string nombreLiga, string token)
        {
            var frontendUrl = _config["Cors:AllowedOrigin"] ?? "https://frontend-quiniela.vercel.app";
            var link = $"{frontendUrl}/invitacion/responder?token={token}";

            await SendEmailAsync(
                emailDestino,
                $"Te invitaron a unirte a la liga: {nombreLiga}",
                $@"
                    <h2>¡Tienes una invitación!</h2>
                    <p>Te han invitado a unirte a la liga <strong>{nombreLiga}</strong> en Quiniela Mundial 2026.</p>
                    <p>Haz clic en el siguiente enlace para aceptar o rechazar la invitación:</p>
                    <a href='{link}' style='
                        display: inline-block;
                        padding: 12px 24px;
                        background-color: #4F46E5;
                        color: white;
                        text-decoration: none;
                        border-radius: 6px;
                        font-weight: bold;
                    '>Ver invitación</a>
                    <p style='color: #666; font-size: 12px; margin-top: 16px;'>
                        Si no puedes hacer clic, copia este link: {link}
                    </p>
                    <p style='color: #666; font-size: 12px;'>
                        Este link expira en 7 días.
                    </p>
                "
            );
        }

        public async Task SendAprobacionMiembroAsync(string emailDestino, string nombreLiga)
        {
            var frontendUrl = _config["Cors:AllowedOrigin"] ?? "https://frontend-quiniela.vercel.app";

            await SendEmailAsync(
                emailDestino,
                $"¡Fuiste aprobado en la liga: {nombreLiga}!",
                $@"
                    <h2>¡Bienvenido a la liga!</h2>
                    <p>El administrador de la liga <strong>{nombreLiga}</strong> ha aprobado tu solicitud.</p>
                    <p>Ya puedes empezar a hacer tus predicciones.</p>
                    <a href='{frontendUrl}' style='
                        display: inline-block;
                        padding: 12px 24px;
                        background-color: #4F46E5;
                        color: white;
                        text-decoration: none;
                        border-radius: 6px;
                        font-weight: bold;
                    '>Ir a Quiniela</a>
                "
            );
        }

        public async Task SendRecuperacionPasswordAsync(string emailDestino, string token)
        {
            var frontendUrl = _config["Cors:AllowedOrigin"] ?? "https://frontend-quiniela.vercel.app";
            var link = $"{frontendUrl}/recuperar-password?token={token}";

            await SendEmailAsync(
                emailDestino,
                "Recuperación de contraseña — Quiniela Mundial 2026",
                $@"
                    <h2>Recuperación de contraseña</h2>
                    <p>Recibimos una solicitud para restablecer la contraseña de tu cuenta.</p>
                    <p>Haz clic en el siguiente enlace para crear una nueva contraseña:</p>
                    <a href='{link}' style='
                        display: inline-block;
                        padding: 12px 24px;
                        background-color: #4F46E5;
                        color: white;
                        text-decoration: none;
                        border-radius: 6px;
                        font-weight: bold;
                    '>Restablecer contraseña</a>
                    <p style='color: #666; font-size: 12px; margin-top: 16px;'>
                        Si no puedes hacer clic, copia este link: {link}
                    </p>
                    <p style='color: #666; font-size: 12px;'>
                        Este link expira en 1 hora. Si no solicitaste esto, ignora este correo.
                    </p>
                "
            );
        }
    }
}
