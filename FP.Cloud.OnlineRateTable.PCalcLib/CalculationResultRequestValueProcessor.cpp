#include "CalculationResultRequestValueProcessor.hpp"
#include "DisplayTypeVisitor.hpp"
#include "Convert.hpp"
#include "PCalcFactory.hpp"
#include "ProductCalculation/ProductDescriptionDefs.hpp"

USING_PRODUCTCALCULATION_NAMESPACE

FP::Cloud::OnlineRateTable::PCalcLib::CalculationResultRequestValueProcessor::CalculationResultRequestValueProcessor(PCalcFactory^ factory) 
	: CalculationResultProcessor(factory)
{

}

void FP::Cloud::OnlineRateTable::PCalcLib::CalculationResultRequestValueProcessor::SetDescription(Shared::PCalcResultInfo^ resultInfo)
{
	PT::RequestValueDescType requestValue = this->Factory->GetActionMgr()->GetActionRequestValue();
	Shared::RequestDescriptionInfo^ result = gcnew Shared::RequestDescriptionInfo();

	result->DescriptionTitle = boost::apply_visitor(DisplayTypeVisitor(), requestValue.m_Title);;
	result->Label = requestValue.m_LabelID;
	result->StatusMessage = boost::apply_visitor(DisplayTypeVisitor(), requestValue.m_Message);
	result->DisplayFormat = Convert::ToString(requestValue.m_DisplayFormat);

	resultInfo->QueryDescription = result->ToTransferDescription();
	resultInfo->QueryType = EQueryType::RequestValue;
}
