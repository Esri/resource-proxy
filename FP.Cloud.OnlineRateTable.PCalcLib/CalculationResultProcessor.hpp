#pragma once

#include "PCalcFactory.hpp"
#include "ProductDescriptionMapper.hpp"
#include "PCalcLib.hpp"

BEGIN_PCALC_LIB_NAMESPACE

using namespace FP::Cloud::OnlineRateTable::Common::ProductCalculation;

private ref class CalculationResultProcessor abstract
{
public:
	PCalcResultInfo^ Handle();

protected:
	CalculationResultProcessor(PCalcFactory^ factory);
	virtual void SetDescription(PCalcResultInfo^ resultInfo) abstract;

	property PCalcFactory^ Factory { PCalcFactory^ get() { return m_Factory; } }

private:
	PCalcFactory^ m_Factory;
};


END_PCALC_LIB_NAMESPACE