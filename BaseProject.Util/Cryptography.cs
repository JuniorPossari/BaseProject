using System.Security.Cryptography;
using System.Text;

namespace BaseProject.Util
{
	public static class Cryptography
    {
		public static string CreateMD5(string text)
		{
			using MD5 md5 = MD5.Create();

			byte[] textBytes = Encoding.ASCII.GetBytes(text);
			byte[] hashBytes = md5.ComputeHash(textBytes);

			var sb = new StringBuilder();

			for (int i = 0; i < hashBytes.Length; i++)
			{
				sb.Append(hashBytes[i].ToString("X2"));
			}

			return sb.ToString();
		}

		public static string CreateMD5(byte[] bytes)
		{
			using MD5 md5 = MD5.Create();

			byte[] hashBytes = md5.ComputeHash(bytes);

			var sb = new StringBuilder();

			for (int i = 0; i < hashBytes.Length; i++)
			{
				sb.Append(hashBytes[i].ToString("X2"));
			}

			return sb.ToString();
		}

	}
}
