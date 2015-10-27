#pragma once

#include "PCalcLib.hpp"
#include "PCalcFactory.hpp"
#include "ProductCalculation/IProductDescParameter.hpp"

USING_PRODUCTCALCULATION_NAMESPACE
BEGIN_PCALC_LIB_NAMESPACE

private ref class ProductDescriptionMapper
{
public:
	ProductDescriptionMapper(PCalcFactory^ factory);

	ProductDescriptionInfo^ GetProduct();
	void SetProduct(ProductDescriptionInfo^ product);
	void SetWeight(WeightInfo^ weight);

private:
	void SetWeight(ProductDescriptionInfo^ target, const WeightType &source);
	void SetWeight(IProductDescParameterPtr &target, WeightInfo^ source);
	void SetPostage(ProductDescriptionInfo^ target, const PostageValueType &source);
	void SetReadyModeSelection(ProductDescriptionInfo^ target, const RmProdSelectType &source);
	void SetAttributes(ProductDescriptionInfo^ targetl, const AttributeListType &source);

private:
	PCalcFactory^ m_Factory;
};

END_PCALC_LIB_NAMESPACE