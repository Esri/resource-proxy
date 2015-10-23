#pragma once

#include "PCalcLib.hpp"
#include "IPCalcProxy.hpp"
#include "PCalcFactory.hpp"
#include "PCalcManager.hpp"
#include "NextActionProcessorProxy.hpp"
#include "ActionResultProcessor.hpp"
#include <stdio.h>

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
	void SetEnvironment(EnvironmentInfo^ environment);

private:
	PCalcFactory^ m_Factory;
	PCalcManager^ m_Manager;
	NextActionProcessorProxy^ m_NextActionProcessor;
	ActionResultProcessor^ m_ActionResultProcessor;
};

END_PCALC_LIB_NAMESPACE




