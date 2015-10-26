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
	ProductDescriptionInfo^ Map();
	IProductDescParameterPtr Map(ProductDescriptionInfo^ product);

private:
	PCalcFactory^ m_Factory;
};

END_PCALC_LIB_NAMESPACE