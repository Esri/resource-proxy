#include "PCalcLibNinjectModule.hpp"
#include "PCalcFactory.hpp"
#include "ActionResultProcessor.hpp"
#include "CalculationResultProcessorProxy.hpp"
#include "ProductDescriptionMapper.hpp"
#include "PCalcLib.hpp"
#include "PCalcProxy.hpp"
#include "IPCalcProxy.hpp"
#include "PCalcManager.hpp"
#include "EnvironmentProcessor.hpp"

void PCalcLibNinjectModule::Load()
{
	this->Bind<Lib::IActionResultProcessor^>()->To<Lib::ActionResultProcessor^>();
	this->Bind<Lib::ICalculationResultProcessorProxy^>()->To<Lib::CalculationResultProcessorProxy^>();
	this->Bind<Lib::IProductDescriptionMapper^>()->To<Lib::ProductDescriptionMapper^>();
	this->Bind<Lib::IPCalcProxy^>()->To<Lib::PCalcProxy^>();
	this->Bind<Lib::IPCalcManager^>()->To<Lib::PCalcManager^>();
	this->Bind<Lib::IEnvironmentProcessor^>()->To<Lib::EnvironmentProcessor^>();
}
