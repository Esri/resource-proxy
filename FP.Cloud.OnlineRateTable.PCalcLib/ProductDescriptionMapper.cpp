#include "Exceptions.hpp"
#include "ProductDescriptionMapper.hpp"

void SetWeight(ProductDescriptionInfo^ product, IProductDescParameterPtr parameter)
{
	EWeightUnit unit;
	INT32 value = parameter->GetWeight().Value;

	switch (parameter->GetWeight().Unit)
	{
		case UNIT_TENTH_GRAM:
			unit = EWeightUnit::TenthGram;
			break;
		case UNIT_GRAM:
			unit = EWeightUnit::Gram;
			break;
		case UNIT_HUNDREDTH_OUNCE:
			unit = EWeightUnit::HoundrethOunce;
			break;
		case UNIT_TENTH_OUNCE:
			unit = EWeightUnit::HoundrethOunce;
			break;
		default:
			throw gcnew FP::Cloud::OnlineRateTable::PCalcLib::PCalcLibException("Unknown unit type");
	}

	WeightInfo^ weight = gcnew WeightInfo();
	weight->WeightUnit = unit;
	weight->WeightValue = value;

	product->Weight = weight;
}

void SetPostage(ProductDescriptionInfo^ product, IProductDescParameterPtr parameter)
{
	PostageInfo^ postage = gcnew PostageInfo();	
	postage->PostageValue = parameter->GetPostageObject().GetAmount();
	postage->PostageDecimals = parameter->GetPostageObject().GetDecimals();

	product->Postage = postage;
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
	SetPostage(product, parameter);

	return product;
}

END_PCALC_LIB_NAMESPACE


