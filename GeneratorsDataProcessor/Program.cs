using GeneratorsDataProcessor.Models;
using GeneratorsDataProcessor.Implementations;
using GeneratorsDataProcessor.Interfaces;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace GeneratorsDataProcessor;
public class Program
{
	static void Main(string[] args)
	{
		// Step 1: Setup configuration from appsettings.json
		var configuration = new ConfigurationBuilder()
			.SetBasePath(Directory.GetCurrentDirectory()) // Set base path to current directory
			.AddJsonFile("appsettings.json", optional: false, reloadOnChange: true) // Add appsettings.json
			.Build(); // Build the configuration

		var appSettings = new AppSettings()
		{
			InputFilePath = configuration["AppSettings:InputFilePath"],
			OutputFilePath = configuration["AppSettings:OutputFilePath"],
			ReferenceDataFilePath = configuration["AppSettings:ReferenceDataFilePath"],
		};

		// Step 2: Configure Dependency Injection
		var serviceProvider = new ServiceCollection()
			.AddSingleton(appSettings)  // Bind AppSettings section
			.AddSingleton<FileSystemWatcher>() // Register FileSystemWatcher as a singleton
			.AddSingleton<IFileMonitor, FileMonitor>() // Register FileMonitor
			.AddTransient<IWindGeneratorService, WindGeneratorService>()
			.AddTransient<ICoalGeneratorService, CoalGeneratorService>()
			.AddTransient<IGasGeneratorService, GasGeneratorService>()
			.BuildServiceProvider();

		// Step 3: Use the DI container to get the IFileMonitor instance
		var fileMonitor = serviceProvider.GetService<IFileMonitor>();
		if (fileMonitor != null)
		{
			fileMonitor.StartMonitoring();
		}

		// Wait for the user to stop the application
		Console.WriteLine("Press Enter to exit...");
		Console.ReadLine();

		// Optionally stop monitoring when exiting
		fileMonitor.StopMonitoring();
	}
}
