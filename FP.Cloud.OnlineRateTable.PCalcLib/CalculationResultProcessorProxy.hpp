#pragma once

#include "PCalcLib.hpp"

BEGIN_PCALC_LIB_NAMESPACE

ref class PCalcFactory;
ref class CalculationResultProcessor;

using namespace FP::Cloud::OnlineRateTable::Common::ProductCalculation;

private ref class CalculationResultProcessorProxy
{
public:
	CalculationResultProcessorProxy(PCalcFactory^ factory);
	PCalcResultInfo^ Handle(INT32 actionID);

private:
	PCalcFactory^ m_Factory;
	System::Collections::Generic::IDictionary<INT32, CalculationResultProcessor^>^ m_Processors;
};

END_PCALC_LIB_NAMESPACE