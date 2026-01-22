using MelonLoader;

namespace Empire.Debug
{
	public enum LogLevel
	{
		Info,
		Warning,
		Error,
		Debug
	}

	public static class DebugLogger
	{
		public static void Log(string message, string category = "")
		{
			if (!EmpireMod.DebugMode.Value)
				return;

			string prefix = GetPrefix(LogLevel.Info, category);
			MelonLogger.Msg($"{prefix}{message}");
		}

		public static void LogWarning(string message, string category = "")
		{
			if (!EmpireMod.DebugMode.Value)
				return; 
			
			string prefix = GetPrefix(LogLevel.Warning, category);
			MelonLogger.Warning($"{prefix}{message}");
		}

		public static void LogError(string message, string category = "")
		{
			if (!EmpireMod.DebugMode.Value)
				return; 
			
			string prefix = GetPrefix(LogLevel.Error, category);
			MelonLogger.Error($"{prefix}{message}");
		}

		public static void LogDebug(string message, string category = "")
		{
			if (!EmpireMod.DebugMode.Value)
				return; 
			
			string prefix = GetPrefix(LogLevel.Debug, category);
			MelonLogger.Msg($"{prefix}{message}");
		}

		private static string GetPrefix(LogLevel level, string category)
		{
			string prefix = $"[{level.ToString().ToUpper()}]";
			if (!string.IsNullOrEmpty(category)) prefix += $"[{category}] ";
			return prefix;
		}
	}
}