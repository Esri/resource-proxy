#pragma once

#include "PCalcLib.hpp"
#include "IPCalcProxy.hpp"
#include "PCalcFactory.hpp"
#include "PCalcManager.hpp"
#include "NextActionProcessorProxy.hpp"

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

	virtual void Init(System::String^ amxPath, System::String^ tablePath);

protected:
	!PCalcProxy();

	property PCalcManager^ Manager { PCalcManager^ get() { return m_Manager; }}
	property PCalcFactory^ Factory { PCalcFactory^ get() { return m_Factory; }}
	property NextActionProcessorProxy^ NextActionProcessor { NextActionProcessorProxy^ get() { return m_NextActionProcessor; }}

	void SetProductDescription(ProductDescriptionInfo^ product, ActionResultInfo^ actionResult);
	void SetActionResult(ProductCalculation::ActionResultType &target, ActionResultInfo^ actionResult);
	void SetEnvironment(EnvironmentInfo^ environment);
	void SetWeight(WeightInfo^ weight);

private:
	PCalcFactory^ m_Factory;
	PCalcManager^ m_Manager;
	NextActionProcessorProxy^ m_NextActionProcessor;
};

END_PCALC_LIB_NAMESPACE




