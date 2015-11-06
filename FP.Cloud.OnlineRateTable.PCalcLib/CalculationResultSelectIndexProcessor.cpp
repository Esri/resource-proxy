#include "CalculationResultSelectIndexProcessor.hpp"
#include "PCalcFactory.hpp"
#include "ProductCalculation/ActionDefs.hpp"
#include "DisplayTypeVisitor.hpp"

using namespace System;
using namespace System::Collections::Generic;


FP::Cloud::OnlineRateTable::PCalcLib::CalculationResultSelectIndexProcessor::CalculationResultSelectIndexProcessor(PCalcFactory^ factory)
	: CalculationResultProcessor(factory)
{
}

void FP::Cloud::OnlineRateTable::PCalcLib::CalculationResultSelectIndexProcessor::SetDescription(Shared::PCalcResultInfo^ resultInfo)
{
	PT::SelectIndexDescType selectIndexDescType = Factory->GetActionMgr()->GetActionSelectIndex();
	Shared::SelectIndexDescriptionInfo^ info = gcnew Shared::SelectIndexDescriptionInfo();

	info->DescriptionTitle = boost::apply_visitor(DisplayTypeVisitor(), selectIndexDescType.m_Title);
	info->IndexEntries = gcnew List<String^>();
	for (auto &current : selectIndexDescType.m_ListOfMessages)
	{
		String^ entry = boost::apply_visitor(DisplayTypeVisitor(), current);
		info->IndexEntries->Add(entry);
	}

	resultInfo->QueryDescription = info->ToTransferDescription();
	resultInfo->QueryType = Shared::EQueryType::SelectIndex;
}
