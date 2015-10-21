#include "NextActionProcessor.hpp"

using namespace FP::Cloud::OnlineRateTable::Common::ProductCalculation;

FP::Cloud::OnlineRateTable::PCalcLib::NextActionProcessor::NextActionProcessor(PCalcFactory^ factory)
	: m_Factory(factory)
{
}

PCalcResultInfo^ FP::Cloud::OnlineRateTable::PCalcLib::NextActionProcessor::GetResult()
{
	PCalcResultInfo^ result = gcnew PCalcResultInfo();
	ProductCalculation::IProductDescParameterPtr parameter = m_Factory->GetProdDesc()->AccessCurrProduct();
	ProductDescriptionInfo^ product = gcnew ProductDescriptionInfo();

	WeightInfo^ weight = gcnew WeightInfo();
	weight->WeightUnit = (EWeightUnit)parameter->GetWeight().Unit;
	weight->WeightValue = parameter->GetWeight().Value;

	product->ProductCode = parameter->GetProductCode();
	product->ProductId = parameter->GetProductID();
	product->Weight = weight;

	result->ProductDescription = product;
	SetDescription(result);

	return result;
}
