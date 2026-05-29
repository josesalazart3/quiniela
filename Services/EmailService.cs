using Resend;
using Quiniela.Services.Interfaces;

namespace Quiniela.Services
{
    public class EmailService : IEmailService
    {
        private readonly IResend _resend;
        private readonly IConfiguration _config;

        public EmailService(IConfiguration config)
        {
            _config = config;
            var apiKey = _config["Resend:ApiKey"] ?? throw new InvalidOperationException("Resend:ApiKey no configurada");
            _resend = ResendClient.Create(apiKey);
        }

        public async Task SendInvitacionLigaAsync(string emailDestino, string nombreLiga, string token)
        {
            var frontendUrl = _config["Cors:AllowedOrigin"] ?? "https://frontend-quiniela.vercel.app";
            var link = $"{frontendUrl}/invitacion/responder?token={token}";

            var message = new EmailMessage
            {
                From = "onboarding@resend.dev",
                To = { emailDestino },
                Subject = $"Te invitaron a unirte a la liga: {nombreLiga}",
                HtmlBody = $@"
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
            };

            await _resend.EmailSendAsync(message);
        }

        public async Task SendAprobacionMiembroAsync(string emailDestino, string nombreLiga)
        {
            var frontendUrl = _config["Cors:AllowedOrigin"] ?? "https://frontend-quiniela.vercel.app";

            var message = new EmailMessage
            {
                From = "onboarding@resend.dev",
                To = { emailDestino },
                Subject = $"¡Fuiste aprobado en la liga: {nombreLiga}!",
                HtmlBody = $@"
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
            };

            await _resend.EmailSendAsync(message);
        }

        public async Task SendRecuperacionPasswordAsync(string emailDestino, string token)
        {
            var frontendUrl = _config["Cors:AllowedOrigin"] ?? "https://frontend-quiniela.vercel.app";
            var link = $"{frontendUrl}/recuperar-password?token={token}";

            var message = new EmailMessage
            {
                From = "onboarding@resend.dev",
                To = { emailDestino },
                Subject = "Recuperación de contraseña — Quiniela Mundial 2026",
                HtmlBody = $@"
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
            };

            await _resend.EmailSendAsync(message);
        }
    }
}
