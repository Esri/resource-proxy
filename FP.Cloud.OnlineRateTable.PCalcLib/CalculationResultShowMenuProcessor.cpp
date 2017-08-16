#include "CalculationResultShowMenuProcessor.hpp"
#include "DisplayTypeVisitor.hpp"
#include "ProductCalculation/ProductDescriptionDefs.hpp"
#include "PCalcFactory.hpp"

using namespace FP::Cloud::OnlineRateTable::Common::ProductCalculation;

FP::Cloud::OnlineRateTable::PCalcLib::CalculationResultShowMenuProcessor::CalculationResultShowMenuProcessor(PCalcFactory^ factory) : CalculationResultProcessor(factory)
{

}

void FP::Cloud::OnlineRateTable::PCalcLib::CalculationResultShowMenuProcessor::SetDescription(PCalcResultInfo^ resultInfo)
{
	PT::MenuDescType showMenu = this->Factory->GetActionMgr()->GetActionShowMenu();

	Shared::ShowMenuDescriptionInfo^ info = gcnew Shared::ShowMenuDescriptionInfo();
	info->DescriptionTitle = boost::apply_visitor(Lib::DisplayTypeVisitor(), showMenu.m_Title);
	info->AdditionalInfo = boost::apply_visitor(Lib::DisplayTypeVisitor(), showMenu.m_AdditionalInfo);

	for (auto const &softkey : showMenu.m_SoftKeyList)
	{
		System::String^ description = boost::apply_visitor(Lib::DisplayTypeVisitor(), softkey.m_SoftkeyDescription);
		BYTE attr(softkey.m_Attribute);
		info->MenuEntries->Add(description);		
	}

	resultInfo->QueryType = Shared::EQueryType::ShowMenu;
	resultInfo->QueryDescription = info->ToTransferDescription();
}

