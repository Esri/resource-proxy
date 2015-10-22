#pragma once

#include "PCalcLib.hpp"

BEGIN_PCALC_LIB_NAMESPACE

ref class PCalcFactory;
ref class NextActionProcessor;

using namespace FP::Cloud::OnlineRateTable::Common::ProductCalculation;

private ref class NextActionProcessorProxy
{
public:
	NextActionProcessorProxy(PCalcFactory^ factory);
	PCalcResultInfo^ Handle(INT32 actionID);

private:
	PCalcFactory^ m_Factory;
	System::Collections::Generic::IDictionary<INT32, NextActionProcessor^>^ m_Processors;
};

END_PCALC_LIB_NAMESPACE