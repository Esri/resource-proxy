#pragma once

#include "PCalcLib.hpp"
#include "Base/misc/std_def.h"


BEGIN_PCALC_LIB_NAMESPACE

ref class PCalcFactory;
ref class CalculationResultProcessor;

public interface class ICalculationResultProcessorProxy
{
	Shared::PCalcResultInfo^ Handle(INT32 actionID);
};


private ref class CalculationResultProcessorProxy : public ICalculationResultProcessorProxy
{
public:
	CalculationResultProcessorProxy(PCalcFactory^ factory);
	virtual Shared::PCalcResultInfo^ Handle(INT32 actionID);

private:
	PCalcFactory^ m_Factory;
	System::Collections::Generic::IDictionary<INT32, CalculationResultProcessor^>^ m_Processors;
};

END_PCALC_LIB_NAMESPACE