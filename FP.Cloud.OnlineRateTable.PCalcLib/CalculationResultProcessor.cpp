#include "CalculationResultProcessor.hpp"
#include "PCalcFactory.hpp"

BEGIN_PCALC_LIB_NAMESPACE

FP::Cloud::OnlineRateTable::PCalcLib::CalculationResultProcessor::CalculationResultProcessor(PCalcFactory^ factory)
	: m_Factory(factory)
{
}

Shared::PCalcResultInfo^ FP::Cloud::OnlineRateTable::PCalcLib::CalculationResultProcessor::Handle()
{
	Shared::PCalcResultInfo^ result = gcnew Shared::PCalcResultInfo();
	SetDescription(result);
	return result;
}

END_PCALC_LIB_NAMESPACE
