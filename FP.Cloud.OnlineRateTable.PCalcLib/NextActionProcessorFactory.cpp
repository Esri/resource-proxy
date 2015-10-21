#include "NextActionProcessorFactory.hpp"
#include "NextActionShowMenuProcessor.hpp"
#include "ProductCalculation/ActionDefs.hpp"
#include "PCalcFactory.hpp"

BEGIN_PCALC_LIB_NAMESPACE
USING_PRODUCTCALCULATION_NAMESPACE

FP::Cloud::OnlineRateTable::PCalcLib::NextActionProcessorFactory::NextActionProcessorFactory(PCalcFactory^ factory)
	: m_Factory(factory)
{

}

PCalcResultInfo^ FP::Cloud::OnlineRateTable::PCalcLib::NextActionProcessorFactory::Process(INT32 actionID)
{
	NextActionProcessor^ strategy;

	switch (actionID)
	{
		case ACTION_SHOW_MENU:
		{
			strategy = gcnew NextActionShowMenuProcessor(m_Factory);
			break;
		}
		default:
			throw gcnew System::InvalidOperationException();
	}

	return strategy->GetResult();
}

END_PCALC_LIB_NAMESPACE
