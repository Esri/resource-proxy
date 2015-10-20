#pragma once

#include "QueryCreationStrategy.hpp"
#include "PCalcLib.hpp"

BEGIN_PCALC_LIB_NAMESPACE

using namespace FP::Cloud::OnlineRateTable::Common::ProductCalculation;

private ref class QueryShowMenuCreationStrategy : public QueryCreationStrategy
{
public:
	QueryShowMenuCreationStrategy(PCalcFactory^ factory) : QueryCreationStrategy(factory)
	{
	}

	virtual void SetQueryDescription(PCalcResultInfo^ resultInfo) override
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
		resultInfo->QueryDescription = info;
	}

};

END_PCALC_LIB_NAMESPACE
