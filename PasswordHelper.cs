using System;
using System.Security.Cryptography;

namespace MusicChange
{
	public static class PasswordHelper
	{
		// 返回格式： iterations.saltBase64.hashBase64
		public static string HashPassword(string password, int iterations = 10000)
		{
			if (password == null)
				throw new ArgumentNullException( nameof( password ) );
			using var rng = RandomNumberGenerator.Create();
			byte[] salt = new byte[16];
			rng.GetBytes( salt );

			using var pbkdf2 = new Rfc2898DeriveBytes( password, salt, iterations, HashAlgorithmName.SHA256 );
			byte[] hash = pbkdf2.GetBytes( 32 );

			return $"{iterations}.{Convert.ToBase64String( salt )}.{Convert.ToBase64String( hash )}";
		}

		// 验证密码（可选）
		public static bool VerifyPassword(string password, string stored)
		{
			if (password == null || stored == null)
				return false;
			var parts = stored.Split( '.' );
			if (parts.Length != 3)
				return false;
			int iterations = int.Parse( parts[0] );
			var salt = Convert.FromBase64String( parts[1] );
			var hash = Convert.FromBase64String( parts[2] );

			using var pbkdf2 = new Rfc2898DeriveBytes( password, salt, iterations, HashAlgorithmName.SHA256 );
			byte[] computed = pbkdf2.GetBytes( hash.Length );
			//return CryptographicOperations.FixedTimeEquals( computed, hash );
			return CryptoHelper.FixedTimeEquals( computed, hash );
		}
	}
}