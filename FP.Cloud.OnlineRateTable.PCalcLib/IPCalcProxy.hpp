#pragma once

#include "PCalcLib.hpp"

BEGIN_PCALC_LIB_NAMESPACE

public interface class IPCalcProxy
{
	Shared::PCalcResultInfo^ Start(Shared::EnvironmentInfo^ environment, Shared::WeightInfo^ weight);
	Shared::PCalcResultInfo^ Calculate(Shared::EnvironmentInfo^ environment, Shared::ProductDescriptionInfo^ product, Shared::ActionResultInfo^ actionResult);
	Shared::PCalcResultInfo^ Calculate(Shared::EnvironmentInfo^ environment, Shared::ProductDescriptionInfo^ product);
	Shared::PCalcResultInfo^ Back(Shared::EnvironmentInfo^ environment, Shared::ProductDescriptionInfo^ product);
};

END_PCALC_LIB_NAMESPACE

