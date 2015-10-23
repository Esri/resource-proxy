#include "CalculationResultProcessorProxy.hpp"
#include "CalculationResultShowMenuProcessor.hpp"
#include "CalculationResultRequestValueProcessor.hpp"
#include "CalculationResultFinishedProcessor.hpp"
#include "ProductCalculation/ActionDefs.hpp"
#include "Exceptions.hpp"
#include "PCalcFactory.hpp"

BEGIN_PCALC_LIB_NAMESPACE
USING_PRODUCTCALCULATION_NAMESPACE

FP::Cloud::OnlineRateTable::PCalcLib::CalculationResultProcessorProxy::CalculationResultProcessorProxy(PCalcFactory^ factory)
	: m_Factory(factory)
	, m_Processors(gcnew System::Collections::Generic::Dictionary<INT32, CalculationResultProcessor^>())
{
	m_Processors->Add(ACTION_SHOW_MENU, gcnew CalculationResultShowMenuProcessor(m_Factory));
	m_Processors->Add(ACTION_REQUEST_VALUE, gcnew CalculationResultRequestValueProcessor(m_Factory));
	m_Processors->Add(ACTION_TEST_IMPRINT, gcnew CalculationResultFinishedProcessor(m_Factory));
}

PCalcResultInfo^ FP::Cloud::OnlineRateTable::PCalcLib::CalculationResultProcessorProxy::Handle(INT32 actionID)
{
	CalculationResultProcessor^ processor;

	if(m_Processors->TryGetValue(actionID, processor))
	{
		return processor->Handle();
	}
	else
	{
		throw gcnew PCalcLibException(System::String::Format("Processor for next action {0} not found", actionID));
	}
}

END_PCALC_LIB_NAMESPACE
