#include "NextActionProcessorProxy.hpp"
#include "NextActionShowMenuProcessor.hpp"
#include "ProductCalculation/ActionDefs.hpp"
#include "PCalcFactory.hpp"

BEGIN_PCALC_LIB_NAMESPACE
USING_PRODUCTCALCULATION_NAMESPACE

FP::Cloud::OnlineRateTable::PCalcLib::NextActionProcessorProxy::NextActionProcessorProxy(PCalcFactory^ factory)
	: m_Factory(factory)
	, m_Processors(gcnew System::Collections::Generic::Dictionary<INT32, NextActionProcessor^>())
{
	m_Processors->Add(ACTION_SHOW_MENU, gcnew NextActionShowMenuProcessor(m_Factory));
}

PCalcResultInfo^ FP::Cloud::OnlineRateTable::PCalcLib::NextActionProcessorProxy::Handle(INT32 actionID)
{
	NextActionProcessor^ processor;

	if(m_Processors->TryGetValue(actionID, processor))
	{
		return processor->Handle();
	}
	else
	{
		throw gcnew System::InvalidOperationException(System::String::Format("Processor for next action {0} not found", actionID));
	}
}

END_PCALC_LIB_NAMESPACE
