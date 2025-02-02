using GeneratorsDataProcessor.Models;

namespace GeneratorsDataProcessor.Interfaces;

public interface ICoalGeneratorService
{
	void ProcessCoalGenerators(List<CoalGenerator> coalGenerators, Factor valueFactor, Factor emissionsFactor, GenerationOutput output);
}
