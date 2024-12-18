namespace Mamkin.In.WebApi.Extensions;

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

            if (parts.Length < 2)
                continue;
            
            Environment.SetEnvironmentVariable(
                variable: parts[0], 
                value: string.Join("=", parts.Skip(1)));
        }

        configuration.AddEnvironmentVariables();
    }
}