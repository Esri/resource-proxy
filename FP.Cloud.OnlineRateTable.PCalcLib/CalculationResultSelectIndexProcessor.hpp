#pragma once

#include "CalculationResultProcessor.hpp"

BEGIN_PCALC_LIB_NAMESPACE

private ref class CalculationResultSelectIndexProcessor : CalculationResultProcessor
{
public:
	CalculationResultSelectIndexProcessor(PCalcFactory^ factory);
	virtual void SetDescription(Shared::PCalcResultInfo^ resultInfo) override;
};

END_PCALC_LIB_NAMESPACE
