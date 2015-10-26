#include "CalculationResultRequestValueProcessor.hpp"

USING_PRODUCTCALCULATION_NAMESPACE

FP::Cloud::OnlineRateTable::PCalcLib::CalculationResultRequestValueProcessor::CalculationResultRequestValueProcessor(PCalcFactory^ factory) 
	: CalculationResultProcessor(factory)
{

}

void FP::Cloud::OnlineRateTable::PCalcLib::CalculationResultRequestValueProcessor::SetDescription(PCalcResultInfo^ resultInfo)
{
	RequestValueDescType requestValue = this->Factory->GetActionMgr()->GetActionRequestValue();
	RequestDescriptionInfo^ result = gcnew RequestDescriptionInfo();

	result->DescriptionTitle = PCalcManagedLib::ConvertWStringToNetString(boost::get<std::wstring>(requestValue.m_Title));
	result->Label = requestValue.m_LabelID;
	result->StatusMessage = PCalcManagedLib::ConvertWStringToNetString(boost::get<std::wstring>(requestValue.m_Message));
	result->DisplayFormat = PCalcManagedLib::ConvertWStringToNetString(requestValue.m_DisplayFormat);

	resultInfo->QueryDescription = result->ToTransferDescription();
	resultInfo->QueryType = EQueryType::RequestValue;
}
