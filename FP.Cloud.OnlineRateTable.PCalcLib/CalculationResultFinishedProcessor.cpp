#include "CalculationResultFinishedProcessor.hpp"
#include "PCalcFactory.hpp"

BEGIN_PCALC_LIB_NAMESPACE

CalculationResultFinishedProcessor::CalculationResultFinishedProcessor(PCalcFactory^ factory) 
	: CalculationResultProcessor(factory)
{

}

void FP::Cloud::OnlineRateTable::PCalcLib::CalculationResultFinishedProcessor::SetDescription(Shared::PCalcResultInfo^ resultInfo)
{
	resultInfo->QueryType = Shared::EQueryType::None;
	resultInfo->QueryDescription = nullptr;
}

END_PCALC_LIB_NAMESPACE

