using GeneratorsDataProcessor.Interfaces;
using GeneratorsDataProcessor.Models;

namespace GeneratorsDataProcessor.Implementations;

public class CoalGeneratorService : ICoalGeneratorService
{
	public void ProcessCoalGenerators(List<CoalGenerator> coalGenerators, Factor valueFactor, Factor emissionsFactor, GenerationOutput output)
	{
		foreach (var generator in coalGenerators)
		{
			var totalGeneration = 0.0;
			foreach (var day in generator.Generation.Days)
			{
				totalGeneration += day.Energy * day.Price;
				var dailyEmission = day.Energy * generator.EmissionsRating * emissionsFactor.High;

				// Track the highest emission generator for each day
				output.MaxEmissionGenerators.Days.Add(new EmissionDay
				{
					Name = generator.Name,
					Date = day.Date,
					Emission = dailyEmission
				});

			}
			// Add Actual Heat Rate
			output.ActualHeatRates.HeatRates.Add(new ActualHeatRate
			{
				Name = generator.Name,
				HeatRate = generator.TotalHeatInput / generator.ActualNetGeneration
			});

			totalGeneration *= valueFactor.Medium;
			output.Totals.Generators.Add(new GeneratorTotal
			{
				Name = generator.Name,
				Total = totalGeneration
			});
		}
	}
}
