using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using AspNetCoreIdentity.Models;
using Microsoft.AspNetCore.Authorization;
using AspNetCoreIdentity.Extensions;
using KissLog;
using System;

namespace AspNetCoreIdentity.Controllers {

	[Authorize]
	public class HomeController: Controller {

		private readonly ILogger _logger;

		public HomeController(ILogger logger) {
			_logger = logger;
		}

		/*Nessa classe o controller está como [Authorize], isso bloqueia todo mundo acessar essa controller (Views)
		 * porém a Home precisa ser acessado por todos, então liberados com o [AllowAnonymous]
		*/

		[AllowAnonymous] // permite todo mundo acessar
		public IActionResult Index() {
			_logger.Trace("Usuario acessou a home!");

			return View();
		}

		//[Authorize] // só funciona para usuários que fizerem login
		public IActionResult Privacy() {


			throw new System.Exception("ERRO");
			return View();
		}

		/*Autorização é diferente de autenticação
		 * Auenticação: Verifica se o usuário está logado
		 * Autorização: Verifica se o usuário tem permissão de acessar
		*/
		[Authorize(Roles = "Admin, Gestor")]
		public IActionResult Secret() {

			try {

				throw new Exception("Algo deu errado!");

			}catch(Exception ex) {
				_logger.Error(ex);
				throw;
			}

			return View();
		}

		[Authorize(Policy = "PodeExcluir")]
		public IActionResult SecretClaim() {

			return View("Secret");
		}

		[Authorize(Policy = "PodeEscrever")]
		public IActionResult SecretClaimGravar() {

			return View("Secret");
		}

		[ClaimsAuthorize("Produtos", "Ler")]
		public IActionResult ClaimsCustom() {

			return View("Secret");
		}

		[Route("erro/{id:length(3,3)}")]
		public IActionResult Error(int id) {

			var modelErro = new ErrorViewModel();
			if(id == 500) {
				modelErro.Mensagem = "Ocorreu um erro! Tente novamente mais tarde ou contate noso suporte.";
				modelErro.Titulo = "Ocorreu um erro!";
				modelErro.ErrorCode = id;
			} else if (id == 404) {

				modelErro.Mensagem = "A página que está procurando não existe! <br />Em caso de dúvidas entre em contato com nosso suporte";
				modelErro.Titulo = "Ocorreu um erro!";
				modelErro.ErrorCode = id;

			} else if(id == 403) {

				modelErro.Mensagem = "Você não tem permissão para fazer isto.";
				modelErro.Titulo = "Acesso negado";
				modelErro.ErrorCode = id;

			} else {

				return StatusCode(404);
			}

			return View("Error", modelErro);
		}
	}
}
