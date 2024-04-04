namespace Chipseky.MamkinInvestor.WebApi.Extensions;

public static class DotEnvExtension
{
    public static void LoadDotEnv(this ConfigurationManager configuration)
    {
        var root = Directory.GetCurrentDirectory();
        var filePath = Path.Combine(root, ".env");
        
        if (!File.Exists(filePath))
            return;

        foreach (var line in File.ReadAllLines(filePath))
        {
            var parts = line.Split(
                '=',
                StringSplitOptions.RemoveEmptyEntries);

            if (parts.Length != 2)
                continue;
            
            Environment.SetEnvironmentVariable(parts[0], parts[1]);
        }

        configuration.AddEnvironmentVariables();
    }
}