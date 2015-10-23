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

	void Handle(ProductDescriptionInfo^ product, ActionResultInfo^ actionResult);
	void Handle(WeightInfo^ weight);

private:
	IProductDescParameterPtr SetProduct(ProductDescriptionInfo^ product);
	void SetActionResult(IProductDescParameterPtr &parameter, ActionResultInfo^ actionResult);
	void SetWeight(IProductDescParameterPtr &parameter, WeightInfo^ weight);

private:
	PCalcFactory^ m_Factory;
};

END_PCALC_LIB_NAMESPACE