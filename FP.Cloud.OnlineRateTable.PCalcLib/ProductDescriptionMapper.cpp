#include "Exceptions.hpp"
#include "ProductDescriptionMapper.hpp"
#include "DisplayObjectTypeVisitor.hpp"
#include "PCalcFactory.hpp"

using namespace System;
using namespace System::Collections::Generic;

FP::Cloud::OnlineRateTable::PCalcLib::ProductDescriptionMapper::ProductDescriptionMapper(PCalcFactory^ factory)
	: m_Factory(factory)
{
}

void FP::Cloud::OnlineRateTable::PCalcLib::ProductDescriptionMapper::SetProduct(Shared::ProductDescriptionInfo^ product)
{
	if (nullptr == product)
		return;

	PT::IProductDescParameterPtr parameter = m_Factory->GetProdDesc()->AccessCurrProduct();

	parameter->SetProductID(product->ProductId);
	parameter->SetProductCode(product->ProductCode);
	parameter->SetRateVersion(product->RateVersion);
	parameter->SetWeightClass(product->WeightClass);
	parameter->SetState(boost::numeric_cast<BYTE>((int)product->State));

	this->SetWeight(parameter, product->Weight);
	this->SetPostage(parameter, product->Postage);
	this->SetAttributes(parameter, product->Attributes);

	parameter->ResetWeightChanged();
}

void FP::Cloud::OnlineRateTable::PCalcLib::ProductDescriptionMapper::SetWeight(Shared::WeightInfo^ weight)
{
	PT::IProductDescParameterPtr parameter = m_Factory->GetProdDesc()->AccessCurrProduct();
	this->SetWeight(parameter, weight);
}

Shared::ProductDescriptionInfo^ FP::Cloud::OnlineRateTable::PCalcLib::ProductDescriptionMapper::GetProduct()
{
	PT::IProductDescParameterPtr parameter = m_Factory->GetProdDesc()->AccessCurrProduct();
	Shared::ProductDescriptionInfo^ product = gcnew Shared::ProductDescriptionInfo();
	
	product->ProductCode = parameter->GetProductCode();
	product->ProductId = parameter->GetProductID();
	product->RateVersion = parameter->GetRateVersion();
	product->WeightClass = parameter->GetWeightClass();	
	product->State  = (Shared::EProductDescriptionState)parameter->GetState();
	
	this->SetWeight(product, parameter->GetWeight());
	this->SetPostage(product, parameter->GetPostageValue());
	this->SetReadyModeSelection(product, parameter->GetRMProductSelection());
	this->SetAttributes(product, parameter->GetAttributes());	

	return product;
}

void FP::Cloud::OnlineRateTable::PCalcLib::ProductDescriptionMapper::SetWeight(Shared::ProductDescriptionInfo^ target, const PT::WeightType &source)
{

	Shared::EWeightUnit unit;
	INT32 value = source.Value;

	switch (source.Unit)
	{
		case PT::UNIT_TENTH_GRAM:
			unit = Shared::EWeightUnit::TenthGram;
			break;
		case PT::UNIT_GRAM:
			unit = Shared::EWeightUnit::Gram;
			break;
		case PT::UNIT_HUNDREDTH_OUNCE:
			unit = Shared::EWeightUnit::HoundrethOunce;
			break;
		case PT::UNIT_TENTH_OUNCE:
			unit = Shared::EWeightUnit::HoundrethOunce;
			break;
		default:
			throw gcnew FP::Cloud::OnlineRateTable::PCalcLib::PCalcLibException("Unknown unit type");
	}

	Shared::WeightInfo^ weight = gcnew Shared::WeightInfo();
	weight->WeightUnit = unit;
	weight->WeightValue = value;

	target->Weight = weight;
}

void FP::Cloud::OnlineRateTable::PCalcLib::ProductDescriptionMapper::SetWeight(PT::IProductDescParameterPtr &target, Shared::WeightInfo^ source)
{
	BYTE unit;
	INT32 value = source->WeightValue;

	switch (source->WeightUnit)
	{
		case Shared::EWeightUnit::TenthGram:
			unit = PT::UNIT_TENTH_GRAM;
			break;
		case Shared::EWeightUnit::Gram:
			unit = PT::UNIT_GRAM;
			break;
		case Shared::EWeightUnit::HoundrethOunce:
			unit = PT::UNIT_HUNDREDTH_OUNCE;
			break;
		case Shared::EWeightUnit::TenthOunce:
			unit = PT::UNIT_TENTH_OUNCE;
			break;
		default:
			throw gcnew PCalcLibException("Unknown unit type");
	}

	PT::WeightType weightType(value, unit);
	target->SetWeight(weightType);
}

void FP::Cloud::OnlineRateTable::PCalcLib::ProductDescriptionMapper::SetPostage(Shared::ProductDescriptionInfo^ target, const PT::PostageValueType &source)
{
	Shared::PostageInfo^ postage = gcnew Shared::PostageInfo();
	postage->PostageValue = source.first;
	postage->PostageDecimals = source.second;

	target->Postage = postage;
}

void FP::Cloud::OnlineRateTable::PCalcLib::ProductDescriptionMapper::SetPostage(PT::IProductDescParameterPtr &target, Shared::PostageInfo^ source)
{
	PT::PostageValueType postageValueType;

	if (nullptr != source)
	{
		std::make_pair(boost::numeric_cast<INT32>(source->PostageValue), boost::numeric_cast<BYTE>(source->PostageDecimals));
	}

	target->SetPostageValue(postageValueType);
}

void FP::Cloud::OnlineRateTable::PCalcLib::ProductDescriptionMapper::SetReadyModeSelection(Shared::ProductDescriptionInfo^ target, const PT::RmProdSelectType &source)
{	
	List<System::String^>^ selections = gcnew List<System::String^>();

	for (auto const &displayObject : source)
	{
		String^ value = boost::apply_visitor(DisplayObjectTypeVisitor(m_Factory->FactoryPtr), displayObject);
		selections->Add(value);
	}

	target->ReadyModeSelection = selections;	
}

void FP::Cloud::OnlineRateTable::PCalcLib::ProductDescriptionMapper::SetAttributes(Shared::ProductDescriptionInfo^ target, const PT::AttributeListType &source)
{
	List<Shared::AttributeInfo^>^ attributes = gcnew List<Shared::AttributeInfo^>();

	for (auto const &current : source)
	{
		Shared::AttributeInfo^ info = gcnew Shared::AttributeInfo();
		info->Key = current.first;
		info->Values = gcnew List<int>();

		for (auto const &value : current.second)
		{
			info->Values->Add(value);
		}

		attributes->Add(info);
	}

	target->Attributes = attributes;	
}

void FP::Cloud::OnlineRateTable::PCalcLib::ProductDescriptionMapper::SetAttributes(PT::IProductDescParameterPtr &target, System::Collections::Generic::List<Shared::AttributeInfo^>^ source)
{
	
	PT::AttributeListType attributes;

	if(nullptr != source)
	{
		for each (Shared::AttributeInfo^ attribute in source)
		{
			INT32 id(attribute->Key);
			std::vector<INT32> values;
	
			for each (INT32 value in attribute->Values)
			{
				values.push_back(value);
			}
	
			attributes.insert(std::make_pair(id, values));
		}
	}
	
	target->SetAttributes(attributes);
}

