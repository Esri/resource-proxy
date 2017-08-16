#pragma once

#include "CalculationResultProcessor.hpp"
#include "PCalcLib.hpp"

BEGIN_PCALC_LIB_NAMESPACE

private ref class CalculationResultShowMenuProcessor : public CalculationResultProcessor
{
public:
	CalculationResultShowMenuProcessor(PCalcFactory^ factory);
	virtual void SetDescription(Shared::PCalcResultInfo^ resultInfo) override;
};

END_PCALC_LIB_NAMESPACE




