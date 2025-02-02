using GeneratorsDataProcessor.Interfaces;
using GeneratorsDataProcessor.Models;

namespace GeneratorsDataProcessor.Implementations;

public class GasGeneratorService : IGasGeneratorService
{
	public void ProcessGasGenerators(List<GasGenerator> gasGenerators, Factor valueFactor, Factor emissionsFactor, GenerationOutput output)
	{
		foreach (var generator in gasGenerators)
		{
			var totalGeneration = 0.0;
			foreach (var day in generator.Generation.Days)
			{
				totalGeneration += day.Energy * day.Price;
				var dailyEmission = day.Energy * generator.EmissionsRating * emissionsFactor.Medium;


				// Track the highest emission generator for each day
				output.MaxEmissionGenerators.Days.Add(new EmissionDay
				{
					Name = generator.Name,
					Date = day.Date,
					Emission = dailyEmission
				});
			}

			totalGeneration *= valueFactor.Medium;
			output.Totals.Generators.Add(new GeneratorTotal
			{
				Name = generator.Name,
				Total = totalGeneration
			});
		}
	}
}
