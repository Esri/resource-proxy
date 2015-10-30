#include "CalculationResultProcessorProxy.hpp"
#include "CalculationResultShowMenuProcessor.hpp"
#include "CalculationResultRequestValueProcessor.hpp"
#include "CalculationResultFinishedProcessor.hpp"
#include "CalculationResultDisplayProcessor.hpp"
#include "CalculationResultSelectIndexProcessor.hpp"
#include "CalculationResultSelectValueProcessor.hpp"
#include "CalculationResultProcessor.hpp"
#include "ProductDescriptionMapper.hpp"
#include "ProductCalculation/ActionDefs.hpp"
#include "Exceptions.hpp"
#include "PCalcFactory.hpp"

USING_PRODUCTCALCULATION_NAMESPACE
BEGIN_PCALC_LIB_NAMESPACE

CalculationResultProcessorProxy::CalculationResultProcessorProxy(FP::Cloud::OnlineRateTable::PCalcLib::PCalcFactory^ factory)
	: m_Factory(factory)
	, m_Processors(gcnew System::Collections::Generic::Dictionary<INT32, CalculationResultProcessor^>())
{
	m_Processors->Add(ACTION_NONE, gcnew CalculationResultFinishedProcessor(m_Factory));
	m_Processors->Add(ACTION_DISPLAY, gcnew CalculationResultDisplayProcessor(m_Factory));
	m_Processors->Add(ACTION_SHOW_MENU, gcnew CalculationResultShowMenuProcessor(m_Factory));
	m_Processors->Add(ACTION_SELECT_INDEX, gcnew CalculationResultSelectIndexProcessor(m_Factory));
	m_Processors->Add(ACTION_SELECT_VALUE, gcnew CalculationResultSelectValueProcessor(m_Factory));
	m_Processors->Add(ACTION_REQUEST_VALUE, gcnew CalculationResultRequestValueProcessor(m_Factory));
	m_Processors->Add(ACTION_TEST_IMPRINT, gcnew CalculationResultFinishedProcessor(m_Factory));
}

PCalcResultInfo^ CalculationResultProcessorProxy::Handle(INT32 actionID)
{
	CalculationResultProcessor^ processor;

	if (m_Processors->TryGetValue(actionID, processor))
	{
		return processor->Handle();
	}
	else
	{
		throw gcnew PCalcLibException(System::String::Format("Processor for next action {0} not found", actionID));
	}
}

END_PCALC_LIB_NAMESPACE
