#include "CalculationResultShowMenuProcessor.hpp"
#include "DisplayTypeVisitor.hpp"

using namespace FP::Cloud::OnlineRateTable::Common::ProductCalculation;

FP::Cloud::OnlineRateTable::PCalcLib::CalculationResultShowMenuProcessor::CalculationResultShowMenuProcessor(PCalcFactory^ factory) : CalculationResultProcessor(factory)
{

}

void FP::Cloud::OnlineRateTable::PCalcLib::CalculationResultShowMenuProcessor::SetDescription(PCalcResultInfo^ resultInfo)
{
	ProductCalculation::MenuDescType showMenu = this->Factory->GetActionMgr()->GetActionShowMenu();

	ShowMenuDescriptionInfo^ info = gcnew ShowMenuDescriptionInfo();
	info->DescriptionTitle = boost::apply_visitor(DisplayTypeVisitor(), showMenu.m_Title);
	info->AdditionalInfo = boost::apply_visitor(DisplayTypeVisitor(), showMenu.m_AdditionalInfo);

	for (auto const &softkey : showMenu.m_SoftKeyList)
	{
		System::String^ description = boost::apply_visitor(DisplayTypeVisitor(), softkey.m_SoftkeyDescription);
		BYTE attr(softkey.m_Attribute);
		info->MenuEntries->Add(description);		
	}
	//std::vector<ProductCalculation::SoftKeyType>::const_iterator pos;
	//for (pos = showMenu.m_SoftKeyList.begin(); pos != showMenu.m_SoftKeyList.end(); ++pos)
	//{
	//	System::String^ description = boost::apply_visitor(DisplayTypeVisitor(), (*pos).m_SoftkeyDescription);
	//	info->MenuEntries->Add(description);
	//}

	resultInfo->QueryType = EQueryType::ShowMenu;
	resultInfo->QueryDescription = info->ToTransferDescription();
}

