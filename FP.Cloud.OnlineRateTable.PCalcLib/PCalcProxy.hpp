#pragma once

#include "PCalcLib.hpp"
#include "IPCalcProxy.hpp"
#include "PCalcFactory.hpp"
#include "PCalcManager.hpp"
#include "NextActionProcessorFactory.hpp"

namespace ProductCalculation
{
	struct ActionResultType;
}

BEGIN_PCALC_LIB_NAMESPACE

private ref class PCalcProxy : System::MarshalByRefObject, public IPCalcProxy
{
public:
	PCalcProxy();
	~PCalcProxy();

	virtual PCalcResultInfo^ Calculate(EnvironmentInfo^ environment, WeightInfo^ weight);
	virtual PCalcResultInfo^ Calculate(EnvironmentInfo^ environment, ProductDescriptionInfo^ product, ActionResultInfo^ actionResult);

	property PCalcManager^ Manager { PCalcManager^ get() { return m_Manager; }}
	property PCalcFactory^ Factory { PCalcFactory^ get() { return m_Factory; }}
	property NextActionProcessorFactory^ Processor { NextActionProcessorFactory^ get() { return m_Processor; }}

protected:
	!PCalcProxy();

	void SetProductDescription(ProductDescriptionInfo^ product, ActionResultInfo^ actionResult);
	void SetActionResult(ProductCalculation::ActionResultType &target, ActionResultInfo^ actionResult);
	void SetEnvironment(EnvironmentInfo^ environment);
	void SetWeight(WeightInfo^ weight);

private:
	PCalcFactory^ m_Factory;
	PCalcManager^ m_Manager;
	NextActionProcessorFactory^ m_Processor;
};

END_PCALC_LIB_NAMESPACE




