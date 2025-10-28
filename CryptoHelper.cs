using System;

public static class CryptoHelper
{
	/// <summary>
	/// 固定时间比较两个字节数组（兼容旧框架，替代 CryptographicOperations.FixedTimeEquals）
	/// </summary>
	/// <param name="a">第一个字节数组</param>
	/// <param name="b">第二个字节数组</param>
	/// <returns>如果数组长度和内容都相同则返回 true，否则返回 false</returns>
	public static bool FixedTimeEquals(byte[] a, byte[] b)
	{
		// 先检查长度，长度不同直接返回 false
		if (a == null || b == null || a.Length != b.Length)
			return false;

		int result = 0;
		// 遍历所有字节，即使中途发现不匹配也继续比较（固定时间）
		for (int i = 0; i < a.Length; i++) {
			// 用异或操作比较每个字节，结果非0表示不匹配
			result |= a[i] ^ b[i];
		}

		// 若所有字节都匹配，result 为 0，返回 true；否则返回 false
		return result == 0;
	}
}