#pragma once

#include "PCalcLib.hpp"

BEGIN_PCALC_LIB_NAMESPACE

public interface class IPCalcProxy
{
	/// <summary>
	/// Initialize the product calculation and prepare the first calculation step
	/// </summary>
	Shared::PCalcResultInfo^ Start(Shared::EnvironmentInfo^ environment, Shared::WeightInfo^ weight);

	/// <summary>
	/// Calculate the product with the input from the previous calculation step
	/// </summary>
	Shared::PCalcResultInfo^ Calculate(Shared::EnvironmentInfo^ environment, Shared::ProductDescriptionInfo^ product, Shared::ActionResultInfo^ actionResult);

	/// <summary>
	/// Recalculate the product with changed properties e.g new weight.
	/// </summary>
	Shared::PCalcResultInfo^ Calculate(Shared::EnvironmentInfo^ environment, Shared::ProductDescriptionInfo^ product);

	/// <summary>
	/// Calculate the previous product
	/// </summary>
	Shared::PCalcResultInfo^ Back(Shared::EnvironmentInfo^ environment, Shared::ProductDescriptionInfo^ product);
};

END_PCALC_LIB_NAMESPACE

