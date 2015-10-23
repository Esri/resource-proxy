#include "CalculationResultFinishedProcessor.hpp"


BEGIN_PCALC_LIB_NAMESPACE
USING_PRODUCTCALCULATION_NAMESPACE

CalculationResultFinishedProcessor::CalculationResultFinishedProcessor(PCalcFactory^ factory) 
	: CalculationResultProcessor(factory)
{

}

void FP::Cloud::OnlineRateTable::PCalcLib::CalculationResultFinishedProcessor::SetDescription(PCalcResultInfo^ resultInfo)
{
	resultInfo->QueryType = EQueryType::None;
	resultInfo->QueryDescription = nullptr;
}

END_PCALC_LIB_NAMESPACE

