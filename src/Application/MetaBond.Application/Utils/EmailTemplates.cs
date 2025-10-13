namespace MetaBond.Application.Utils;

public static class EmailTemplates
{
    public static string GetPasswordRecoveryEmailHtml(string to, string verificationCode)
    {
        var digits = string.Join("", verificationCode.Select(c => $"<td class=\"code-digit\">{c}</td>"));
        return $@"
			<!DOCTYPE html>
			<html lang=""es"">
			<head>
			<meta charset=""UTF-8"">
			<meta name=""viewport"" content=""width=device-width, initial-scale=1.0"">
			<title>Recuperación de Contraseña</title>
			<style>
			    @import url('https://fonts.googleapis.com/css2?family=Poppins:wght@400;600;700&display=swap');
			    body {{ margin:0; padding:0; font-family:'Poppins',sans-serif; background-color:#f9f9f9; color:#333; }}
			    table {{ border-collapse:collapse; width:100%; }}
			    .container {{ max-width:650px; margin:0 auto; background-color:#fff; border-radius:20px; overflow:hidden; }}
			    .header {{ background: linear-gradient(135deg,#4CAF50 0%,#2E7D32 100%); text-align:center; padding:30px 20px; }}
			    .header h1 {{ color:#fff; margin:0; font-size:28px; }}
			    .content {{ padding:40px 30px; }}
			    .greeting {{ font-size:22px; font-weight:600; margin-bottom:20px; }}
			    .message {{ font-size:16px; color:#555; margin-bottom:30px; line-height:1.8; }}
			    .code-container {{ background:#f5f5f5; border-radius:15px; padding:25px 15px; text-align:center; margin:30px 0; }}
			    .code-title {{ font-size:16px; color:#555; margin-bottom:15px; text-transform:uppercase; }}
			    .code-digit {{ width:45px; height:60px; background:linear-gradient(135deg,#3498db 0%,#2980b9 100%); color:white; font-size:28px; font-weight:bold; text-align:center; vertical-align:middle; border-radius:10px; }}
			    .info-box {{ background:#FFF8E1; border-left:4px solid #FF9800; padding:15px; margin:25px 0; border-radius:5px; }}
			    .info-box-title {{ font-weight:600; color:#FF9800; margin-bottom:5px; }}
			    .button-container {{ text-align:center; margin:35px 0 25px; }}
			    .footer {{ background: linear-gradient(135deg,#FF9800 0%,#F57C00 100%); text-align:center; padding:25px 20px; color:white; }}
			    @media only screen and (max-width:600px) {{
			        .code-digit {{ width:35px; height:50px; font-size:24px; }}
			        .greeting {{ font-size:20px; }}
			        .header h1 {{ font-size:24px; }}
			    }}
			</style>
			</head>
			<body>
			<table class=""container"" cellpadding=""0"" cellspacing=""0"">
			    <tr>
			        <td class=""header"">
			            <h1>MetaBond</h1>
			        </td>
			    </tr>
			    <tr>
			        <td class=""content"">
			            <p class=""greeting"">¡Hola, {to}!</p>
			            <p class=""message"">Recibimos una solicitud para restablecer tu contraseña. Para continuar con este proceso, usa el siguiente código de verificación:</p>

			            <table class=""code-container"" cellpadding=""0"" cellspacing=""0"">
			                <tr>
			                    <td class=""code-title"" colspan=""{verificationCode.Length}"">Tu código de verificación</td>
			                </tr>
			                <tr>
			                    {digits}
			                </tr>
			            </table>

			            <div class=""info-box"">
			                <p class=""info-box-title"">Importante</p>
			                <p>Este código expirará en 10 minutos. Si no solicitaste un cambio de contraseña, puedes ignorar este correo.</p>
			            </div>

			            <div class=""help-section"">
			                <p>Si tienes problemas para restablecer tu contraseña o no solicitaste este cambio, contáctanos a <a href=""mailto:soporte@metaBond.com"" style=""color:#1976D2; font-weight:600;"">soporte@metabond.com</a></p>
			            </div>
			        </td>
			    </tr>
			    <tr>
			        <td class=""footer"">
			            <p>© 2025 MetaBond</p>
			            <p>Creciendo juntos, aprendiendo siempre</p>
			        </td>
			    </tr>
			</table>
			</body>
			</html>";
    }

    public static string ConfirmAccountEmailHtml(string verificationCode)
    {
        // var digits = string.Join(Environment.NewLine,
        //     verificationCode.Select(c => $"<div class=\"code-digit\">{c}</div>"));

        var digits = string.Join("", verificationCode.Select(c => $"<td class=\"code-digit\">{c}</td>"));
        return $@"
			<!DOCTYPE html>
			<html lang=""es"">
			<head>
			<meta charset=""UTF-8"">
			<meta name=""viewport"" content=""width=device-width, initial-scale=1.0"">
			<title>Confirmación de Cuenta</title>
			<style>
			    @import url('https://fonts.googleapis.com/css2?family=Poppins:wght@400;600;700&display=swap');
			    body {{ margin:0; padding:0; font-family:'Poppins',sans-serif; background-color:#f9f9f9; color:#333; }}
			    table {{ border-collapse:collapse; width:100%; }}
			    .container {{ max-width:650px; margin:0 auto; background-color:#fff; border-radius:20px; overflow:hidden; }}
			    .header {{ background: linear-gradient(135deg,#4CAF50 0%,#2E7D32 100%); text-align:center; padding:30px 20px; }}
			    .header h1 {{ color:#fff; margin:0; font-size:28px; }}
			    .content {{ padding:40px 30px; }}
			    .greeting {{ font-size:22px; font-weight:600; margin-bottom:20px; }}
			    .message {{ font-size:16px; color:#555; margin-bottom:30px; line-height:1.8; }}
			    .code-container {{ background:#f5f5f5; border-radius:15px; padding:25px 15px; text-align:center; margin:30px 0; }}
			    .code-title {{ font-size:16px; color:#555; margin-bottom:15px; text-transform:uppercase; }}
			    .code-digit {{ width:45px; height:60px; background:linear-gradient(135deg,#3498db 0%,#2980b9 100%); color:white; font-size:28px; font-weight:bold; text-align:center; vertical-align:middle; border-radius:10px; }}
			    .info-box {{ background:#E8F5E8; border-left:4px solid #4CAF50; padding:15px; margin:25px 0; border-radius:5px; }}
			    .info-box-title {{ font-weight:600; color:#4CAF50; margin-bottom:5px; }}
			    .welcome-section {{ background:#E3F2FD; border-radius:10px; padding:20px; margin-top:30px; }}
			    .welcome-title {{ font-weight:600; color:#1976D2; margin-bottom:10px; }}
			    .help-section {{ background:#FFF3E0; border-radius:10px; padding:20px; margin-top:20px; }}
			    .help-title {{ font-weight:600; color:#FF9800; margin-bottom:10px; }}
			    .footer {{ background: linear-gradient(135deg,#FF9800 0%,#F57C00 100%); text-align:center; padding:25px 20px; color:white; }}
			    @media only screen and (max-width:600px) {{
			        .code-digit {{ width:35px; height:50px; font-size:24px; }}
			        .greeting {{ font-size:20px; }}
			        .header h1 {{ font-size:24px; }}
			    }}
			</style>
			</head>
			<body>
			<table class=""container"" cellpadding=""0"" cellspacing=""0"">
			    <tr>
			        <td class=""header"">
			            <h1>MetaBond</h1>
			        </td>
			    </tr>
			    <tr>
			        <td class=""content"">
			            <p class=""greeting"">¡Bienvenido a nuestra comunidad!</p>
			            <p class=""message"">¡Gracias por registrarte! Para completar la activación de tu cuenta y comenzar tu viaje de aprendizaje, confirma tu dirección de correo electrónico usando el siguiente código:</p>

			            <table class=""code-container"" cellpadding=""0"" cellspacing=""0"">
			                <tr>
			                    <td class=""code-title"" colspan=""{verificationCode.Length}"">Código de confirmación</td>
			                </tr>
			                <tr>
			                    {digits}
			                </tr>
			            </table>

			            <div class=""info-box"">
			                <p class=""info-box-title"">¡Ya casi estás listo!</p>
			                <p>Este código expirará en 10 minutos. Una vez confirmada tu cuenta, tendrás acceso completo a todos nuestros recursos de aprendizaje.</p>
			            </div>

			            <div class=""welcome-section"">
			                <p class=""welcome-title"">¿Qué puedes hacer después?</p>
			                <ul style=""margin:10px 0; padding-left:20px; color:#555;"">
			                    <li>Conectar con comunidades basado en tus interes</li>
			                    <li>Conocer personas de tú área de estudio</li>
			                </ul>
			            </div>

			            <div class=""help-section"">
			                <p class=""help-title"">¿Necesitas ayuda?</p>
			                <p>Si tienes problemas para confirmar tu cuenta o no solicitaste este registro, contáctanos a <a href=""mailto:soporte@metabond.com"" style=""color:#FF9800; font-weight:600;"">soporte@metabond.com</a></p>
			            </div>
			        </td>
			    </tr>
			    <tr>
			        <td class=""footer"">
			            <p>© 2025 MetaBond</p>
			            <p>Creciendo juntos, aprendiendo siempre</p>
			        </td>
			    </tr>
			</table>
			</body>
			</html>";
    }
}