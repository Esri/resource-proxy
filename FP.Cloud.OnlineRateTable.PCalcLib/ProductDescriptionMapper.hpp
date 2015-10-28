#pragma once

#include "PCalcLib.hpp"
#include "ProductCalculation/IProductDescParameter.hpp"

BEGIN_PCALC_LIB_NAMESPACE

ref class PCalcFactory;

private ref class ProductDescriptionMapper
{
public:
	ProductDescriptionMapper(PCalcFactory^ factory);

	Shared::ProductDescriptionInfo^ GetProduct();
	void SetProduct(Shared::ProductDescriptionInfo^ product);
	void SetWeight(Shared::WeightInfo^ weight);

private:
	void SetWeight(Shared::ProductDescriptionInfo^ target, const PT::WeightType &source);
	void SetWeight(PT::IProductDescParameterPtr &target, Shared::WeightInfo^ source);
	void SetPostage(Shared::ProductDescriptionInfo^ target, const PT::PostageValueType &source);
	void SetPostage(PT::IProductDescParameterPtr &target, Shared::PostageInfo^ source);
	void SetReadyModeSelection(Shared::ProductDescriptionInfo^ target, const PT::RmProdSelectType &source);
	void SetAttributes(Shared::ProductDescriptionInfo^ target, const PT::AttributeListType &source);
	void SetAttributes(PT::IProductDescParameterPtr &target, System::Collections::Generic::List<Shared::AttributeInfo^>^ source);

private:
	PCalcFactory^ m_Factory;
};

END_PCALC_LIB_NAMESPACE