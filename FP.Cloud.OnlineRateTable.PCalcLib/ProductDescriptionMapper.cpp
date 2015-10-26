#include "ProductDescriptionMapper.hpp"

void SetWeight(ProductDescriptionInfo^ product, IProductDescParameterPtr parameter)
{
	WeightInfo^ weight = gcnew WeightInfo();
	weight->WeightUnit = (EWeightUnit)parameter->GetWeight().Unit;
	weight->WeightValue = parameter->GetWeight().Value;

	product->Weight = weight;
}

USING_PRODUCTCALCULATION_NAMESPACE
BEGIN_PCALC_LIB_NAMESPACE

ProductDescriptionMapper::ProductDescriptionMapper(PCalcFactory^ factory)
	: m_Factory(factory)
{

}

ProductCalculation::IProductDescParameterPtr FP::Cloud::OnlineRateTable::PCalcLib::ProductDescriptionMapper::Map(ProductDescriptionInfo^ product)
{
	ProductCalculation::IProductDescParameterPtr parameter = m_Factory->GetProdDesc()->AccessCurrProduct();
	return parameter;
}

ProductDescriptionInfo^ FP::Cloud::OnlineRateTable::PCalcLib::ProductDescriptionMapper::Map()
{
	ProductCalculation::IProductDescParameterPtr parameter = m_Factory->GetProdDesc()->AccessCurrProduct();
	ProductDescriptionInfo^ product = gcnew ProductDescriptionInfo();

	product->ProductCode = parameter->GetProductCode();
	product->ProductId = parameter->GetProductID();
	product->RateVersion = parameter->GetRateVersion();
	product->WeightClass = parameter->GetWeightClass();	

	SetWeight(product, parameter);
	return product;
}

END_PCALC_LIB_NAMESPACE


