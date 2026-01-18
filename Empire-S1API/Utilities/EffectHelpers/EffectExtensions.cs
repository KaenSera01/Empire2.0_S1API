namespace Empire.Utilities.EffectHelpers
{
	public static class EffectExtensions
	{
		public static EffectInfo? GetEffect(this string name)
			=> EffectRegistry.ByName.TryGetValue(name, out var e) ? e : null;
	}
}