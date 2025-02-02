using GeneratorsDataProcessor.Models;

namespace GeneratorsDataProcessor.Interfaces;

public interface IWindGeneratorService
{
	void ProcessWindGenerators(List<WindGenerator> windGenerators, Factor valueFactor, GenerationOutput output);
}
