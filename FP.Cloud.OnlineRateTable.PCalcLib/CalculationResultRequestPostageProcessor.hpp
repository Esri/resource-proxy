#pragma once

#include "CalculationResultProcessor.hpp"

BEGIN_PCALC_LIB_NAMESPACE

private ref class CalculationResultRequestPostageProcessor : public CalculationResultProcessor
{
public:
	CalculationResultRequestPostageProcessor(PCalcFactory^ factory)
		: CalculationResultProcessor(factory)
	{
	}

	virtual void SetDescription(Shared::PCalcResultInfo^ resultInfo) override;
};

END_PCALC_LIB_NAMESPACE
