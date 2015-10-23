#include "CalculationResultShowMenuProcessor.hpp"

using namespace FP::Cloud::OnlineRateTable::Common::ProductCalculation;

FP::Cloud::OnlineRateTable::PCalcLib::CalculationResultShowMenuProcessor::CalculationResultShowMenuProcessor(PCalcFactory^ factory) : CalculationResultProcessor(factory)
{

}

void FP::Cloud::OnlineRateTable::PCalcLib::CalculationResultShowMenuProcessor::SetDescription(PCalcResultInfo^ resultInfo)
{
	ProductCalculation::MenuDescType showMenu = this->Factory->GetActionMgr()->GetActionShowMenu();

	ShowMenuDescriptionInfo^ info = gcnew ShowMenuDescriptionInfo();
	info->DescriptionTitle = PCalcManagedLib::ConvertWStringToNetString(boost::get<std::wstring>(showMenu.m_Title));
	info->AdditionalInfo = PCalcManagedLib::ConvertWStringToNetString(boost::get<std::wstring>(showMenu.m_AdditionalInfo));

	std::vector<ProductCalculation::SoftKeyType>::const_iterator pos;
	for (pos = showMenu.m_SoftKeyList.begin(); pos != showMenu.m_SoftKeyList.end(); ++pos)
	{
		System::String^ description = PCalcManagedLib::ConvertWStringToNetString(boost::get<std::wstring>((*pos).m_SoftkeyDescription));
		info->MenuEntries->Add(description);
	}

	resultInfo->QueryType = EQueryType::ShowMenu;
	resultInfo->QueryDescription = info->ToTransferDescription();
}

