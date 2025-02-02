using GeneratorsDataProcessor.Models;

namespace GeneratorsDataProcessor.Interfaces;

public interface IGasGeneratorService
{
	void ProcessGasGenerators(List<GasGenerator> gasGenerators, Factor valueFactor, Factor emissionsFactor, GenerationOutput output);
}
