#pragma once

#include "PCalcFactory.hpp"
#include "PCalcLib.hpp"

BEGIN_PCALC_LIB_NAMESPACE
USING_PRODUCTCALCULATION_NAMESPACE



using namespace FP::Cloud::OnlineRateTable::Common::ProductCalculation;

private ref class ActionResultProcessor
{
public:
	ActionResultProcessor(PCalcFactory^ factory);
	void Handle(ActionResultInfo^ actionResult);

private:
	void SetActionResult(IProductDescParameterPtr &parameter, ActionResultInfo^ actionResult);

private:
	PCalcFactory^ m_Factory;
};

END_PCALC_LIB_NAMESPACE