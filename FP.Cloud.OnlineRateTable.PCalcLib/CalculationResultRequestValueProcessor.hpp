#pragma once

#include "CalculationResultProcessor.hpp"
#include "PCalcLib.hpp"

BEGIN_PCALC_LIB_NAMESPACE


private ref class CalculationResultRequestValueProcessor : public CalculationResultProcessor
{
public:
	CalculationResultRequestValueProcessor(PCalcFactory^ factory);
	virtual void SetDescription(Shared::PCalcResultInfo^ resultInfo) override;
};

END_PCALC_LIB_NAMESPACE