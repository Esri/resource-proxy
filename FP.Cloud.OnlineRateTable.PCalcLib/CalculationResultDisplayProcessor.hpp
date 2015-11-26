#pragma once

#include "PCalcLib.hpp"
#include "CalculationResultProcessor.hpp"

BEGIN_PCALC_LIB_NAMESPACE

private ref class CalculationResultDisplayProcessor : CalculationResultProcessor
{
public:
	CalculationResultDisplayProcessor(PCalcFactory^ factory);
	virtual void SetDescription(Shared::PCalcResultInfo^ resultInfo) override;
};

END_PCALC_LIB_NAMESPACE