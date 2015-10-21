#pragma once

#include "PCalcLib.hpp"

BEGIN_PCALC_LIB_NAMESPACE

ref class PCalcFactory;

using namespace FP::Cloud::OnlineRateTable::Common::ProductCalculation;

private ref class NextActionProcessorFactory
{
public:
	NextActionProcessorFactory(PCalcFactory^ factory);
	PCalcResultInfo^ Process(INT32 actionID);

private:
	PCalcFactory^ m_Factory;
};

END_PCALC_LIB_NAMESPACE