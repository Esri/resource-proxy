#include "QueryCreationStrategy.hpp"

using namespace FP::Cloud::OnlineRateTable::Common::ProductCalculation;

FP::Cloud::OnlineRateTable::PCalcLib::QueryCreationStrategy::QueryCreationStrategy(PCalcFactory^ factory)
	: m_Factory(factory)
{

}

void FP::Cloud::OnlineRateTable::PCalcLib::QueryCreationStrategy::SetResult(PCalcResultInfo^ resultInfo)
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
