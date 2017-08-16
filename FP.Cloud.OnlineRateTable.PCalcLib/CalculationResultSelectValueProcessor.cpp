#include "CalculationResultSelectValueProcessor.hpp"
#include "PCalcFactory.hpp"
#include "DisplayTypeVisitor.hpp"
#include "ProductCalculation/ActionDefs.hpp"

using namespace System;
using namespace System::Collections::Generic;

FP::Cloud::OnlineRateTable::PCalcLib::CalculationResultSelectValueProcessor::CalculationResultSelectValueProcessor(PCalcFactory^ factory)
	: CalculationResultProcessor(factory)
{

}

void FP::Cloud::OnlineRateTable::PCalcLib::CalculationResultSelectValueProcessor::SetDescription(Shared::PCalcResultInfo^ resultInfo)
{
	PT::SelectValueDescType selectValueDescType = Factory->GetActionMgr()->GetActionSelectValue();
	Shared::SelectValueDescriptionInfo^ info = gcnew Shared::SelectValueDescriptionInfo();

	info->DescriptionTitle = boost::apply_visitor(DisplayTypeVisitor(), selectValueDescType.m_Title);
	info->ValueEntries = gcnew List<Shared::ValueEntryInfo^>();
	for (auto &current : selectValueDescType.m_ListOfEntries)
	{
		Shared::ValueEntryInfo^ entry = gcnew Shared::ValueEntryInfo();
		entry->EntryMessage = boost::apply_visitor(DisplayTypeVisitor(), current.m_Message);
		entry->EntryValue = current.m_Value;
		info->ValueEntries->Add(entry);
	}

	resultInfo->QueryType = Shared::EQueryType::SelectValue;
	resultInfo->QueryDescription = info->ToTransferDescription();
}
