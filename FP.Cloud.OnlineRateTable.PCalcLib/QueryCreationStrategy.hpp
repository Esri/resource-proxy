#pragma once

#include "PCalcFactory.hpp"
#include "PCalcLib.hpp"

BEGIN_PCALC_LIB_NAMESPACE

using namespace FP::Cloud::OnlineRateTable::Common::ProductCalculation;

private ref class QueryCreationStrategy abstract
{
public:
	void SetResult(PCalcResultInfo^ resultInfo)
	{
		ProductCalculation::IProductDescParameterPtr parameter = m_Factory->GetProdDesc()->AccessCurrProduct();
		ProductDescriptionInfo^ product = gcnew ProductDescriptionInfo();

		WeightInfo^ weight = gcnew WeightInfo();
		weight->WeightUnit = (EWeightUnit)parameter->GetWeight().Unit;
		weight->WeightValue = parameter->GetWeight().Value;

		product->ProductCode = parameter->GetProductCode();
		product->ProductId = parameter->GetProductID();
		product->Weight = weight;

		resultInfo->ProductDescription = product;

		SetQueryDescription(resultInfo);
	}


protected:
	QueryCreationStrategy(PCalcFactory^ factory);
	virtual void SetQueryDescription(PCalcResultInfo^ resultInfo) abstract;

	property PCalcFactory^ Factory
	{
		PCalcFactory^ get() { return m_Factory; }
	}

private:
	PCalcFactory^ m_Factory;
};


END_PCALC_LIB_NAMESPACE