using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations.Schema;

namespace BaseProject.DAO.Models
{
	public class AspNetUser : IdentityUser
	{
		[InverseProperty("IdAspNetUserNavigation")]
		public virtual Usuario Usuario { get; set; }
	}

	public static class UserExtensions
	{

		public static AspNetUser[] ObterTodos(
			this UserManager<AspNetUser> userManager
		)
		{
			return userManager.Users.Include(x => x.Usuario).ToArray();
		}

		public static AspNetUser ObterPorId(
			this UserManager<AspNetUser> userManager,
			string id,
			string includeProperties = "Usuario"
		)
		{
			var query = userManager.Users.AsQueryable();

			foreach (var includeProperty in includeProperties.Split
			(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries))
			{
				query = query.Include(includeProperty);
			}

			return query.FirstOrDefault(x => x.Id == id);
		}

		public static AspNetUser ObterPorEmail(
			this UserManager<AspNetUser> userManager,
			string email,
			string includeProperties = ""
		)
		{
			var query = userManager.Users.AsQueryable();

			foreach (var includeProperty in includeProperties.Split
			(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries))
			{
				query = query.Include(includeProperty);
			}

			return query.FirstOrDefault(x => x.Email == email);
		}

		public static AspNetUser ObterPorUserName(
			this UserManager<AspNetUser> userManager,
			string userName,
			string includeProperties = ""
		)
		{
			var query = userManager.Users.AsQueryable();

			foreach (var includeProperty in includeProperties.Split
			(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries))
			{
				query = query.Include(includeProperty);
			}

			return query.FirstOrDefault(x => x.UserName == userName);
		}

		public static AspNetUser ObterPorIdUsuario(
			this UserManager<AspNetUser> userManager,
			int idUsuario,
			string includeProperties = ""
		)
		{
			var query = userManager.Users.AsQueryable();

			foreach (var includeProperty in includeProperties.Split
			(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries))
			{
				query = query.Include(includeProperty);
			}

			return query.FirstOrDefault(x => x.Usuario.Id == idUsuario);
		}

	}

}