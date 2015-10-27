#include "Exceptions.hpp"
#include "ProductDescriptionMapper.hpp"
#include "DisplayObjectTypeVisitor.hpp"

USING_PRODUCTCALCULATION_NAMESPACE
using namespace System;
using namespace System::Collections::Generic;

FP::Cloud::OnlineRateTable::PCalcLib::ProductDescriptionMapper::ProductDescriptionMapper(PCalcFactory^ factory)
	: m_Factory(factory)
{
}

void FP::Cloud::OnlineRateTable::PCalcLib::ProductDescriptionMapper::SetProduct(ProductDescriptionInfo^ product)
{
	if (nullptr == product)
		return;

	IProductDescParameterPtr parameter = m_Factory->GetProdDesc()->AccessCurrProduct();

	parameter->SetProductID(product->ProductId);
	parameter->SetProductCode(product->ProductCode);
	parameter->SetRateVersion(product->RateVersion);
	parameter->SetWeightClass(product->WeightClass);
	parameter->SetState(boost::numeric_cast<BYTE>((int)product->State));

	this->SetWeight(parameter, product->Weight);
}

void FP::Cloud::OnlineRateTable::PCalcLib::ProductDescriptionMapper::SetWeight(WeightInfo^ weight)
{
	IProductDescParameterPtr parameter = m_Factory->GetProdDesc()->AccessCurrProduct();
	this->SetWeight(parameter, weight);
}

ProductDescriptionInfo^ FP::Cloud::OnlineRateTable::PCalcLib::ProductDescriptionMapper::GetProduct()
{
	IProductDescParameterPtr parameter = m_Factory->GetProdDesc()->AccessCurrProduct();
	ProductDescriptionInfo^ product = gcnew ProductDescriptionInfo();

	product->ProductCode = parameter->GetProductCode();
	product->ProductId = parameter->GetProductID();
	product->RateVersion = parameter->GetRateVersion();
	product->WeightClass = parameter->GetWeightClass();	
	product->State  = (EProductDescriptionState)parameter->GetState();		
	
	this->SetWeight(product, parameter->GetWeight());
	this->SetPostage(product, parameter->GetPostageValue());
	this->SetReadyModeSelection(product, parameter->GetRMProductSelection());
	this->SetAttributes(product, parameter->GetAttributes());

	return product;
}

void FP::Cloud::OnlineRateTable::PCalcLib::ProductDescriptionMapper::SetWeight(ProductDescriptionInfo^ target, const WeightType &source)
{

	EWeightUnit unit;
	INT32 value = source.Value;

	switch (source.Unit)
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

	target->Weight = weight;
}

void FP::Cloud::OnlineRateTable::PCalcLib::ProductDescriptionMapper::SetWeight(IProductDescParameterPtr &target, WeightInfo^ source)
{
	BYTE unit;
	INT32 value = source->WeightValue;

	switch (source->WeightUnit)
	{
		case EWeightUnit::TenthGram:
			unit = UNIT_TENTH_GRAM;
			break;
		case EWeightUnit::Gram:
			unit = UNIT_GRAM;
			break;
		case EWeightUnit::HoundrethOunce:
			unit = UNIT_HUNDREDTH_OUNCE;
			break;
		case EWeightUnit::TenthOunce:
			unit = UNIT_TENTH_OUNCE;
			break;
		default:
			throw gcnew PCalcLibException("Unknown unit type");
	}

	WeightType weightType(value, unit);
	target->SetWeight(weightType);
}

void FP::Cloud::OnlineRateTable::PCalcLib::ProductDescriptionMapper::SetPostage(ProductDescriptionInfo^ target, const PostageValueType &source)
{
	PostageInfo^ postage = gcnew PostageInfo();
	postage->PostageValue = source.first;
	postage->PostageDecimals = source.second;

	target->Postage = postage;
}

void FP::Cloud::OnlineRateTable::PCalcLib::ProductDescriptionMapper::SetReadyModeSelection(ProductDescriptionInfo^ target, const ProductCalculation::RmProdSelectType &source)
{	
	List<System::String^>^ selections = gcnew List<System::String^>();

	for (auto const &displayObject : source)
	{
		String^ value = boost::apply_visitor(DisplayObjectTypeVisitor(m_Factory->FactoryPtr), displayObject);
		selections->Add(value);
	}

	target->ReadyModeSelection = selections;	
}

void FP::Cloud::OnlineRateTable::PCalcLib::ProductDescriptionMapper::SetAttributes(ProductDescriptionInfo^ target, const AttributeListType &source)
{
	List<AttributeInfo^>^ attributes = gcnew List<AttributeInfo^>();

	for (auto const &current : source)
	{
		AttributeInfo^ info = gcnew AttributeInfo();
		info->Key = current.first;
		info->Values = gcnew List<int>();

		for (auto const &second : current.second)
		{
			info->Values->Add(second);
		}

		attributes->Add(info);
	}

	target->Attributes = attributes;
}

