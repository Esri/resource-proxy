#pragma once

#include "PCalcLib.hpp"

BEGIN_PCALC_LIB_NAMESPACE

using namespace FP::Cloud::OnlineRateTable::Common::ProductCalculation;

public interface class IPCalcProxy
{
	PCalcResultInfo^ Calculate(EnvironmentInfo^ environment, WeightInfo^ weight);
	PCalcResultInfo^ Calculate(EnvironmentInfo^ environment, ProductDescriptionInfo^ product, ActionResultInfo^ actionResult);
};

END_PCALC_LIB_NAMESPACE

