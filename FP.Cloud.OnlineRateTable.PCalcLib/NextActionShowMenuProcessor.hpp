#pragma once

#include "NextActionProcessor.hpp"
#include "PCalcLib.hpp"

BEGIN_PCALC_LIB_NAMESPACE

using namespace FP::Cloud::OnlineRateTable::Common::ProductCalculation;

private ref class NextActionShowMenuProcessor : public NextActionProcessor
{
public:
	NextActionShowMenuProcessor(PCalcFactory^ factory);
	virtual void SetDescription(PCalcResultInfo^ resultInfo) override;
};

END_PCALC_LIB_NAMESPACE




