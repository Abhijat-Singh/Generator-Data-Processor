using GeneratorsDataProcessor.Helpers;
using GeneratorsDataProcessor.Interfaces;
using GeneratorsDataProcessor.Models;
using Microsoft.Extensions.Options;

namespace GeneratorsDataProcessor.Implementations;

public class FileMonitor : IFileMonitor
{
	private readonly FileSystemWatcher _fileSystemWatcher;
	private readonly AppSettings _appSettings;
	private readonly IWindGeneratorService _windGeneratorService;
	private readonly ICoalGeneratorService _coalGeneratorService;
	private readonly IGasGeneratorService _gasGeneratorService;

	// Injecting FileSystemWatcher directly
	public FileMonitor(FileSystemWatcher fileSystemWatcher, AppSettings appSettings, IWindGeneratorService windGeneratorService, ICoalGeneratorService coalGeneratorService,
		IGasGeneratorService gasGeneratorService)
	{
		_fileSystemWatcher = fileSystemWatcher;
		_appSettings = appSettings;
		_windGeneratorService = windGeneratorService;
		_coalGeneratorService = coalGeneratorService;
		_gasGeneratorService = gasGeneratorService;

		// Configure the watcher
		_fileSystemWatcher.Path = appSettings.InputFilePath;
		_fileSystemWatcher.Filter = "*.xml"; // Only watch for XML files
		_fileSystemWatcher.NotifyFilter = NotifyFilters.FileName | NotifyFilters.LastWrite;
		_fileSystemWatcher.Created += OnFileCreated;
	}

	private void OnFileCreated(object sender, FileSystemEventArgs e)
	{
		// Process the new file
		Console.WriteLine($"New file detected: {e.Name}");

		string outputFilePath = Path.Combine(_appSettings.OutputFilePath, Path.GetFileNameWithoutExtension(e.Name) + "-Result.xml");

		try
		{
			// Deserialize the input XML files
			GenerationReport generationReport = CommonHelper.DeserializeXml<GenerationReport>(Path.Combine(_appSettings.InputFilePath, e.Name));
			ReferenceData referenceData = CommonHelper.DeserializeXml<ReferenceData>(_appSettings.ReferenceDataFilePath);

			// Process the data
			GenerationOutput output = ProcessData(generationReport, referenceData);

			// Serialize the output to XML
			CommonHelper.SerializeXml(output, outputFilePath);
			Console.WriteLine($"Processing complete. Output saved to {outputFilePath}");
		}
		catch (Exception ex)
		{
			Console.WriteLine($"Error processing file: {ex.Message}");
		}
	}

	private GenerationOutput ProcessData(GenerationReport generationReport, ReferenceData referenceData)
	{
		var output = new GenerationOutput
		{
			Totals = new Totals
			{
				Generators = new List<GeneratorTotal>()
			},
			MaxEmissionGenerators = new MaxEmissionGenerators
			{
				Days = new List<EmissionDay>()
			},
			ActualHeatRates = new ActualHeatRates
			{
				HeatRates = new List<ActualHeatRate>()
			}
		};

		// Process each generator type: Wind, Gas, Coal
		_windGeneratorService.ProcessWindGenerators(generationReport.Wind.WindGenerators, referenceData.Factors.ValueFactor, output);
		_gasGeneratorService.ProcessGasGenerators(generationReport.Gas.GasGenerators, referenceData.Factors.ValueFactor, referenceData.Factors.EmissionsFactor, output);
		_coalGeneratorService.ProcessCoalGenerators(generationReport.Coal.CoalGenerators, referenceData.Factors.ValueFactor, referenceData.Factors.EmissionsFactor, output);

		output.MaxEmissionGenerators.Days = output.MaxEmissionGenerators.Days.GroupBy(x => x.Date)
											.Select(x =>
											{
												var highest = x.OrderByDescending(x => x.Emission).First();
												return new EmissionDay
												{
													Date = x.Key,
													Name = highest.Name,
													Emission = highest.Emission,
												};
											}
											)
											.ToList();

		return output;
	}

	public void StartMonitoring()
	{
		_fileSystemWatcher.EnableRaisingEvents = true;
		Console.WriteLine("Monitoring input folder for new XML files...");
	}

	public void StopMonitoring()
	{
		_fileSystemWatcher.EnableRaisingEvents = false;
		Console.WriteLine("Stopped monitoring.");
	}
}
