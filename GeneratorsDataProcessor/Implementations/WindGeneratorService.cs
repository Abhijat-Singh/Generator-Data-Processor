using GeneratorsDataProcessor.Interfaces;
using GeneratorsDataProcessor.Models;

namespace GeneratorsDataProcessor.Implementations;

public class WindGeneratorService : IWindGeneratorService
{
	public void ProcessWindGenerators(List<WindGenerator> windGenerators, Factor valueFactor, GenerationOutput output)
	{
		foreach (var generator in windGenerators)
		{
			var totalGeneration = 0.0;
			foreach (var day in generator.Generation.Days)
			{
				totalGeneration += day.Energy * day.Price;
			}

			totalGeneration = totalGeneration * (generator.Name == "Wind[Offshore]" ? valueFactor.Low : valueFactor.High);

			output.Totals.Generators.Add(new GeneratorTotal
			{
				Name = generator.Name,
				Total = totalGeneration
			});
		}
	}
}
