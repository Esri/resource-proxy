#include "CalculationResultProcessor.hpp"

USING_PRODUCTCALCULATION_NAMESPACE
BEGIN_PCALC_LIB_NAMESPACE

FP::Cloud::OnlineRateTable::PCalcLib::CalculationResultProcessor::CalculationResultProcessor(PCalcFactory^ factory)
	: m_Factory(factory)
{
}

PCalcResultInfo^ FP::Cloud::OnlineRateTable::PCalcLib::CalculationResultProcessor::Handle(ProductDescriptionMapper^ productDescriptionProcessor)
{
	PCalcResultInfo^ result = gcnew PCalcResultInfo();

	if (nullptr != productDescriptionProcessor)
	{
		result->ProductDescription = productDescriptionProcessor->Map();
	}
	SetDescription(result);

	return result;
}

END_PCALC_LIB_NAMESPACE
