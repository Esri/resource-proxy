#pragma once

#include "CalculationResultProcessor.hpp"

BEGIN_PCALC_LIB_NAMESPACE

private ref class CalculationResultSelectValueProcessor : public CalculationResultProcessor
{
public:
	CalculationResultSelectValueProcessor(PCalcFactory^ factory);
	virtual void SetDescription(Shared::PCalcResultInfo^ resultInfo) override;
};

END_PCALC_LIB_NAMESPACE