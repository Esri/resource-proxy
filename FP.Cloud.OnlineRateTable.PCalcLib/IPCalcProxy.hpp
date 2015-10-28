#pragma once

#include "PCalcLib.hpp"

BEGIN_PCALC_LIB_NAMESPACE

using namespace FP::Cloud::OnlineRateTable::Common::ProductCalculation;

public interface class IPCalcProxy
{
	PCalcResultInfo^ Start(EnvironmentInfo^ environment, WeightInfo^ weight);
	PCalcResultInfo^ Calculate(EnvironmentInfo^ environment, ProductDescriptionInfo^ product, ActionResultInfo^ actionResult);
	PCalcResultInfo^ Calculate(EnvironmentInfo^ environment, ProductDescriptionInfo^ product);
	PCalcResultInfo^ Back(EnvironmentInfo^ environment, ProductDescriptionInfo^ product);
};

END_PCALC_LIB_NAMESPACE

