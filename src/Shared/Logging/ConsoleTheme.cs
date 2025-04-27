using Serilog;
using Serilog.Sinks.SystemConsole.Themes;

public static class ConsoleTheme { 
    public static AnsiConsoleTheme Colorful { get; } = new AnsiConsoleTheme(
        new Dictionary<ConsoleThemeStyle, string> {
            [ConsoleThemeStyle.Text] = ConsoleColors.BrightWhite,      
            [ConsoleThemeStyle.SecondaryText] = ConsoleColors.DarkGray, 
            [ConsoleThemeStyle.SecondaryText] = ConsoleColors.DarkGray, 
            [ConsoleThemeStyle.TertiaryText] = ConsoleColors.Gray, 
            [ConsoleThemeStyle.Invalid] = ConsoleColors.BrightYellow,     
            [ConsoleThemeStyle.Null] = ConsoleColors.Blue,                
            [ConsoleThemeStyle.Name] = ConsoleColors.Cyan,                
            [ConsoleThemeStyle.String] = ConsoleColors.BrightGreen,      
            [ConsoleThemeStyle.Number] = ConsoleColors.Magenta,           
            [ConsoleThemeStyle.Boolean] = ConsoleColors.Blue,             
            [ConsoleThemeStyle.Scalar] = ConsoleColors.Cyan,             
            [ConsoleThemeStyle.LevelVerbose] = ConsoleColors.DarkGray,    
            [ConsoleThemeStyle.LevelDebug] = ConsoleColors.Magenta,       
            [ConsoleThemeStyle.LevelInformation] = ConsoleColors.BrightGreen, 
            [ConsoleThemeStyle.LevelWarning] = ConsoleColors.BrightYellow, 
            [ConsoleThemeStyle.LevelError] = $"{ConsoleColors.BrightRed}{ConsoleColors.BackgroundBlack}", 
        });
    
    public static LoggerConfiguration WithColorfulTheme(this LoggerConfiguration config) {
        return config.WriteTo.Console(
            theme: Colorful,
            outputTemplate: "[{Timestamp:HH:mm:ss} {Level:u3}] {Message:lj}{NewLine}{Exception}"
        );
    }
}