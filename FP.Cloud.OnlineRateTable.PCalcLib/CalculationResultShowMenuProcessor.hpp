#pragma once

#include "CalculationResultProcessor.hpp"
#include "PCalcLib.hpp"

BEGIN_PCALC_LIB_NAMESPACE

using namespace FP::Cloud::OnlineRateTable::Common::ProductCalculation;

private ref class CalculationResultShowMenuProcessor : public CalculationResultProcessor
{
public:
	CalculationResultShowMenuProcessor(PCalcFactory^ factory);
	virtual void SetDescription(PCalcResultInfo^ resultInfo) override;
};

END_PCALC_LIB_NAMESPACE




