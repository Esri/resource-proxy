#pragma once

#include "PCalcLib.hpp"
#include "IPCalcProxy.hpp"
#include "PCalcFactory.hpp"
#include "PCalcManager.hpp"
#include "CalculationResultProcessorProxy.hpp"
#include "ProductDescriptionMapper.hpp"
#include "ActionResultProcessor.hpp"
#include "EnvironmentProcessor.hpp"
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

private:
	PCalcFactory^ m_Factory;
	PCalcManager^ m_Manager;
	CalculationResultProcessorProxy^ m_CalculationResultProcessor;
	ActionResultProcessor^ m_ActionResultProcessor;
	EnvironmentProcessor^ m_EnvironmentProcessor;
	ProductDescriptionMapper^ m_ProductDescriptionMapper;
};

END_PCALC_LIB_NAMESPACE




