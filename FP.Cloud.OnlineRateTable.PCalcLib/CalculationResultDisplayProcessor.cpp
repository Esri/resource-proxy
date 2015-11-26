#include "CalculationResultDisplayProcessor.hpp"
#include "PCalcFactory.hpp"
#include "ProductCalculation/ActionDefs.hpp"
#include "DisplayTypeVisitor.hpp"

FP::Cloud::OnlineRateTable::PCalcLib::CalculationResultDisplayProcessor::CalculationResultDisplayProcessor(PCalcFactory^ factory)
	: CalculationResultProcessor(factory)
{

}

void FP::Cloud::OnlineRateTable::PCalcLib::CalculationResultDisplayProcessor::SetDescription(Shared::PCalcResultInfo^ resultInfo)
{
	PT::DisplayDescType displayDescType = Factory->GetActionMgr()->GetActionDisplay();

	Shared::DescriptionInfo^ info = gcnew Shared::DescriptionInfo();
	info->DescriptionTitle = boost::apply_visitor(DisplayTypeVisitor(), displayDescType.m_Message);
	
	resultInfo->QueryType = Shared::EQueryType::ShowDisplay;
	resultInfo->QueryDescription = info->ToTransferDescription();	
}
