namespace MetaBond.Application.Utils;

public static class EmailTemplates
{
    public static string GetPasswordRecoveryEmailHtml(string to, string verificationCode)
    {
        var digits = string.Join(Environment.NewLine,
            verificationCode.Select(c => $"<div class=\"code-digit\">{c}</div>"));
        return @$"
			<!DOCTYPE html>
			<html lang=""es"">

			<head>
				<meta charset=""UTF-8"">
				<meta name=""viewport"" content=""width=device-width, initial-scale=1.0"">
				<title>Recuperación de Contraseña</title>
				<style>
					@import url('https://fonts.googleapis.com/css2?family=Poppins:wght@400;600;700&display=swap');

					body {{
						font-family: 'Poppins', sans-serif;
						line-height: 1.6;
						color: #333333;
						margin: 0;
						padding: 0;
						background-color: #f9f9f9;
					}}

					.container {{
						max-width: 650px;
						margin: 0 auto;
						background-color: #ffffff;
						border-radius: 20px;
						overflow: hidden;
						box-shadow: 0 10px 30px rgba(0, 0, 0, 0.1);
					}}

					.header {{
						background: linear-gradient(135deg, #4CAF50 0%, #2E7D32 100%);
						padding: 30px 20px;
						text-align: center;
						position: relative;
						overflow: hidden;
					}}

					.header h1 {{
						color: white;
						margin: 0;
						font-size: 28px;
						position: relative;
						z-index: 2;
						text-shadow: 2px 2px 4px rgba(0, 0, 0, 0.2);
					}}

					.header-graphic {{
						position: absolute;
						top: 0;
						left: 0;
						width: 100%;
						height: 100%;
						z-index: 1;
						opacity: 0.3;
					}}

					.header-graphic .circle {{
						position: absolute;
						border-radius: 50%;
						background-color: white;
					}}

					.header-graphic .circle-1 {{
						width: 100px;
						height: 100px;
						top: -30px;
						left: 10%;
					}}

					.header-graphic .circle-2 {{
						width: 80px;
						height: 80px;
						bottom: -20px;
						right: 15%;
					}}

					.header-graphic .circle-3 {{
						width: 60px;
						height: 60px;
						top: 50%;
						right: 5%;
					}}

					.content {{
						padding: 40px 30px;
						position: relative;
					}}

					.greeting {{
						font-size: 22px;
						font-weight: 600;
						margin-bottom: 20px;
						color: #333;
					}}

					.message {{
						font-size: 16px;
						color: #555;
						margin-bottom: 30px;
						line-height: 1.8;
					}}

					.code-container {{
						background: linear-gradient(135deg, #f5f5f5 0%, #e0e0e0 100%);
						border-radius: 15px;
						padding: 25px 15px;
						margin: 30px 0;
						text-align: center;
						position: relative;
						overflow: hidden;
						box-shadow: 0 5px 15px rgba(0, 0, 0, 0.05);
					}}

					.code-title {{
						font-size: 16px;
						color: #555;
						margin-bottom: 15px;
						text-transform: uppercase;
						letter-spacing: 1px;
					}}

					.code {{
						display: flex;
						justify-content: center;
						gap: 10px;
						margin: 0 auto;
						max-width: 360px;
					}}

					.code-digit {{
						width: 45px;
						height: 60px;
						background: linear-gradient(135deg, #3498db 0%, #2980b9 100%);
						border-radius: 10px;
						display: flex;
						align-items: center;
						justify-content: center;
						color: white;
						font-size: 28px;
						font-weight: bold;
						box-shadow: 0 4px 8px rgba(0, 0, 0, 0.2);
						position: relative;
						overflow: hidden;
					}}

					.code-digit::after {{
						content: '';
						position: absolute;
						top: 0;
						left: 0;
						width: 100%;
						height: 30%;
						background-color: rgba(255, 255, 255, 0.2);
						border-radius: 10px 10px 0 0;
					}}

					.info-box {{
						background-color: #FFF8E1;
						border-left: 4px solid #FF9800;
						padding: 15px;
						margin: 25px 0;
						border-radius: 5px;
					}}

					.info-box-title {{
						font-weight: 600;
						color: #FF9800;
						margin: 0 0 5px 0;
						display: flex;
						align-items: center;
					}}

					.info-box-title::before {{
						content: '!';
						display: inline-flex;
						align-items: center;
						justify-content: center;
						width: 20px;
						height: 20px;
						background-color: #FF9800;
						color: white;
						border-radius: 50%;
						margin-right: 10px;
						font-size: 14px;
						font-weight: bold;
					}}

					.button-container {{
						text-align: center;
						margin: 35px 0 25px;
					}}

					.button {{
						display: inline-block;
						background: linear-gradient(135deg, #FF9800 0%, #F57C00 100%);
						color: white;
						text-decoration: none;
						padding: 14px 30px;
						border-radius: 50px;
						font-weight: 600;
						font-size: 16px;
						box-shadow: 0 4px 15px rgba(255, 152, 0, 0.4);
					}}

					.divider {{
						height: 1px;
						background: linear-gradient(to right, transparent, #e0e0e0, transparent);
						margin: 30px 0;
					}}

					.help-section {{
						background-color: #E3F2FD;
						border-radius: 10px;
						padding: 20px;
						margin-top: 30px;
					}}

					.help-title {{
						font-weight: 600;
						color: #1976D2;
						margin: 0 0 10px 0;
						display: flex;
						align-items: center;
					}}

					.help-title::before {{
						content: '?';
						display: inline-flex;
						align-items: center;
						justify-content: center;
						width: 20px;
						height: 20px;
						background-color: #1976D2;
						color: white;
						border-radius: 50%;
						margin-right: 10px;
						font-size: 14px;
						font-weight: bold;
					}}

					.footer {{
						background: linear-gradient(135deg, #FF9800 0%, #F57C00 100%);
						padding: 25px 20px;
						text-align: center;
						color: white;
						position: relative;
						overflow: hidden;
					}}

					.footer-content {{
						position: relative;
						z-index: 2;
					}}

					.social-links {{
						margin-top: 15px;
					}}

					.social-icon {{
						display: inline-block;
						width: 36px;
						height: 36px;
						background-color: rgba(255, 255, 255, 0.2);
						border-radius: 50%;
						margin: 0 5px;
						display: inline-flex;
						align-items: center;
						justify-content: center;
						font-size: 18px;
					}}

					@media only screen and (max-width: 600px) {{
						.container {{
							border-radius: 0;
						}}

						.code {{
							gap: 5px;
						}}

						.code-digit {{
							width: 35px;
							height: 50px;
							font-size: 24px;
						}}

						.header h1 {{
							font-size: 24px;
						}}

						.greeting {{
							font-size: 20px;
						}}
					}}
				</style>
			</head>

			<body>
				<div class=""container"">
					<div class=""header"">
						<div class=""header-graphic"">
							<div class=""circle circle-1""></div>
							<div class=""circle circle-2""></div>
							<div class=""circle circle-3""></div>
						</div>
						<h1>MetaBond</h1>
					</div>

					<div class=""content"">
						<div class=""greeting"">¡Hola, {to}!</div>

						<div class=""message"">
							Recibimos una solicitud para restablecer tu contraseña. Para continuar con este proceso, usa el siguiente código
							de verificación:
						</div>

						<div class=""code-container"">
							<div class=""code-title"">Tu código de verificación</div>
							<div class=""code"">
								{digits}
							</div>
						</div>

						<div class=""info-box"">
							<div class=""info-box-title"">Importante</div>
							<p>Este código expirará en 10 minutos. Si no solicitaste un cambio de contraseña, puedes ignorar este correo.
							</p>
						</div>

						<div class=""divider""></div>

						<div class=""help-section"">
							<div class=""help-title"">¿Necesitas ayuda?</div>
							<p>Si tienes problemas para restablecer tu contraseña o no solicitaste este cambio, contáctanos a <a
									href=""mailto:soporte@metaBond.com""
									style=""color: #1976D2; font-weight: 600;"">soporte@metabond.com</a></p>
						</div>

					</div>

					<div class=""footer"">
						<div class=""footer-content"">
							<p>© 2025 MetaBond</p>
							<p>Creciendo juntos, aprendiendo siempre</p>
						</div>
					</div>
				</div>
			</body>

			</html>
            ";
    }

    public static string ConfirmAccountEmailHtml(string verificationCode)
    {
        var digits = string.Join(Environment.NewLine,
            verificationCode.Select(c => $"<div class=\"code-digit\">{c}</div>"));

        return @$"<!DOCTYPE html>
		<html lang=""es"">

		<head>
			<meta charset=""UTF-8"">
			<meta name=""viewport"" content=""width=device-width, initial-scale=1.0"">
			<title>Confirmación de Cuenta</title>
			<style>
				@import url('https://fonts.googleapis.com/css2?family=Poppins:wght@400;600;700&display=swap');

				body {{
					font-family: 'Poppins', sans-serif;
					line-height: 1.6;
					color: #333333;
					margin: 0;
					padding: 0;
					background-color: #f9f9f9;
				}}

				.container {{
					max-width: 650px;
					margin: 0 auto;
					background-color: #ffffff;
					border-radius: 20px;
					overflow: hidden;
					box-shadow: 0 10px 30px rgba(0, 0, 0, 0.1);
				}}

				.header {{
					background: linear-gradient(135deg, #4CAF50 0%, #2E7D32 100%);
					padding: 30px 20px;
					text-align: center;
					position: relative;
					overflow: hidden;
				}}

				.header h1 {{
					color: white;
					margin: 0;
					font-size: 28px;
					position: relative;
					z-index: 2;
					text-shadow: 2px 2px 4px rgba(0, 0, 0, 0.2);
				}}

				.header-graphic {{
					position: absolute;
					top: 0;
					left: 0;
					width: 100%;
					height: 100%;
					z-index: 1;
					opacity: 0.3;
				}}

				.header-graphic .circle {{
					position: absolute;
					border-radius: 50%;
					background-color: white;
				}}

				.header-graphic .circle-1 {{
					width: 100px;
					height: 100px;
					top: -30px;
					left: 10%;
				}}

				.header-graphic .circle-2 {{
					width: 80px;
					height: 80px;
					bottom: -20px;
					right: 15%;
				}}

				.header-graphic .circle-3 {{
					width: 60px;
					height: 60px;
					top: 50%;
					right: 5%;
				}}

				.content {{
					padding: 40px 30px;
					position: relative;
				}}

				.greeting {{
					font-size: 22px;
					font-weight: 600;
					margin-bottom: 20px;
					color: #333;
				}}

				.message {{
					font-size: 16px;
					color: #555;
					margin-bottom: 30px;
					line-height: 1.8;
				}}

				.code-container {{
					background: linear-gradient(135deg, #f5f5f5 0%, #e0e0e0 100%);
					border-radius: 15px;
					padding: 25px 15px;
					margin: 30px 0;
					text-align: center;
					position: relative;
					overflow: hidden;
					box-shadow: 0 5px 15px rgba(0, 0, 0, 0.05);
				}}

				.code-title {{
					font-size: 16px;
					color: #555;
					margin-bottom: 15px;
					text-transform: uppercase;
					letter-spacing: 1px;
				}}

				.code {{
					display: flex;
					justify-content: center;
					gap: 10px;
					margin: 0 auto;
					max-width: 360px;
				}}

				.code-digit {{
					width: 45px;
					height: 60px;
					background: linear-gradient(135deg, #3498db 0%, #2980b9 100%);
					border-radius: 10px;
					display: flex;
					align-items: center;
					justify-content: center;
					color: white;
					font-size: 28px;
					font-weight: bold;
					box-shadow: 0 4px 8px rgba(0, 0, 0, 0.2);
					position: relative;
					overflow: hidden;
				}}

				.code-digit::after {{
					content: '';
					position: absolute;
					top: 0;
					left: 0;
					width: 100%;
					height: 30%;
					background-color: rgba(255, 255, 255, 0.2);
					border-radius: 10px 10px 0 0;
				}}

				.info-box {{
					background-color: #E8F5E8;
					border-left: 4px solid #4CAF50;
					padding: 15px;
					margin: 25px 0;
					border-radius: 5px;
				}}

				.info-box-title {{
					font-weight: 600;
					color: #4CAF50;
					margin: 0 0 5px 0;
					display: flex;
					align-items: center;
				}}

				.info-box-title::before {{
					content: '✓';
					display: inline-flex;
					align-items: center;
					justify-content: center;
					width: 20px;
					height: 20px;
					background-color: #4CAF50;
					color: white;
					border-radius: 50%;
					margin-right: 10px;
					font-size: 14px;
					font-weight: bold;
				}}

				.button-container {{
					text-align: center;
					margin: 35px 0 25px;
				}}

				.button {{
					display: inline-block;
					background: linear-gradient(135deg, #FF9800 0%, #F57C00 100%);
					color: white;
					text-decoration: none;
					padding: 14px 30px;
					border-radius: 50px;
					font-weight: 600;
					font-size: 16px;
					box-shadow: 0 4px 15px rgba(255, 152, 0, 0.4);
				}}

				.divider {{
					height: 1px;
					background: linear-gradient(to right, transparent, #e0e0e0, transparent);
					margin: 30px 0;
				}}

				.welcome-section {{
					background-color: #E3F2FD;
					border-radius: 10px;
					padding: 20px;
					margin-top: 30px;
				}}

				.welcome-title {{
					font-weight: 600;
					color: #1976D2;
					margin: 0 0 10px 0;
					display: flex;
					align-items: center;
				}}

				.welcome-title::before {{
					content: '🎉';
					display: inline-flex;
					align-items: center;
					justify-content: center;
					width: 20px;
					height: 20px;
					margin-right: 10px;
					font-size: 16px;
				}}

				.help-section {{
					background-color: #FFF3E0;
					border-radius: 10px;
					padding: 20px;
					margin-top: 20px;
				}}

				.help-title {{
					font-weight: 600;
					color: #FF9800;
					margin: 0 0 10px 0;
					display: flex;
					align-items: center;
				}}

				.help-title::before {{
					content: '?';
					display: inline-flex;
					align-items: center;
					justify-content: center;
					width: 20px;
					height: 20px;
					background-color: #FF9800;
					color: white;
					border-radius: 50%;
					margin-right: 10px;
					font-size: 14px;
					font-weight: bold;
				}}

				.footer {{
					background: linear-gradient(135deg, #FF9800 0%, #F57C00 100%);
					padding: 25px 20px;
					text-align: center;
					color: white;
					position: relative;
					overflow: hidden;
				}}

				.footer-content {{
					position: relative;
					z-index: 2;
				}}

				.social-links {{
					margin-top: 15px;
				}}

				.social-icon {{
					display: inline-block;
					width: 36px;
					height: 36px;
					background-color: rgba(255, 255, 255, 0.2);
					border-radius: 50%;
					margin: 0 5px;
					display: inline-flex;
					align-items: center;
					justify-content: center;
					font-size: 18px;
				}}

				@media only screen and (max-width: 600px) {{
					.container {{
						border-radius: 0;
					}}

					.code {{
						gap: 5px;
					}}

					.code-digit {{
						width: 35px;
						height: 50px;
						font-size: 24px;
					}}

					.header h1 {{
						font-size: 24px;
					}}

					.greeting {{
						font-size: 20px;
					}}
				}}
			</style>
		</head>

		<body>
			<div class=""container"">
				<div class=""header"">
					<div class=""header-graphic"">
						<div class=""circle circle-1""></div>
						<div class=""circle circle-2""></div>
						<div class=""circle circle-3""></div>
					</div>
					<h1>MetaBond</h1>
				</div>

				<div class=""content"">
					<div class=""greeting"">¡Bienvenido a nuestra comunidad!</div>

					<div class=""message"">
						¡Gracias por registrarte! Para completar la activación de tu cuenta y comenzar tu viaje de aprendizaje, confirma
						tu dirección de correo electrónico usando el siguiente código:
					</div>

					<div class=""code-container"">
						<div class=""code-title"">Código de confirmación</div>
						<div class=""code"">
							{digits}
						</div>
					</div>

					<div class=""info-box"">
						<div class=""info-box-title"">¡Ya casi estás listo!</div>
						<p>Este código expirará en 10 minutos. Una vez confirmada tu cuenta, tendrás acceso completo a todos nuestros
							recursos de aprendizaje.</p>
					</div>

					<div class=""welcome-section"">
						<div class=""welcome-title"">¿Qué puedes hacer después?</div>
						<ul style=""margin: 10px 0; padding-left: 20px; color: #555;"">
							<li>Conectar con comunidades basado en tus interes</li>
							<li>Conocer personas de tú área de estudio</li>
						</ul>
					</div>

					<div class=""help-section"">
						<div class=""help-title"">¿Necesitas ayuda?</div>
						<p>Si tienes problemas para confirmar tu cuenta o no solicitaste este registro, contáctanos a <a
								href=""mailto:soporte@comunidadaprendizaje.com""
								style=""color: #FF9800; font-weight: 600;"">soporte@metabond.com</a></p>
					</div>
				</div>

				<div class=""footer"">
					<div class=""footer-content"">
						<p>© 2025 MetaBond</p>
						<p>Creciendo juntos, aprendiendo siempre</p>
					</div>
				</div>
			</div>
		</body>

		</html>";
    }
}