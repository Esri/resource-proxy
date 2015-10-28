#include "ActionResultProcessor.hpp"
#include "ProductCalculation/IProductDescParameter.hpp"
#include "Exceptions.hpp"
#include "PCalcFactory.hpp"

FP::Cloud::OnlineRateTable::PCalcLib::ActionResultProcessor::ActionResultProcessor(PCalcFactory^ factory)
	: m_Factory(factory)
{

}

void FP::Cloud::OnlineRateTable::PCalcLib::ActionResultProcessor::Handle(Shared::ActionResultInfo^ actionResult)
{
	PT::IProductDescParameterPtr parameter = m_Factory->GetProdDesc()->AccessCurrProduct();
	this->SetActionResult(parameter, actionResult);
}

void FP::Cloud::OnlineRateTable::PCalcLib::ActionResultProcessor::SetActionResult(PT::IProductDescParameterPtr &parameter, Shared::ActionResultInfo^ actionResult)
{
	PT::ActionResultType target;
	PT::ResultValueVectorType results;

	for each(Shared::AnyInfo^ current in actionResult->Results)
	{
		switch (current->AnyType)
		{
			case Shared::EAnyType::INT32:
				results.push_back(System::Convert::ToInt32(current->AnyValue));
				break;
			case Shared::EAnyType::UINT32:
				results.push_back(System::Convert::ToUInt32(current->AnyValue));
				break;
			default:
				throw gcnew PCalcLibException(System::String::Format("Unexpected any type {0}", current->ToString()));
		}
	}

	target.ID = (int)actionResult->Action;
	target.Label = actionResult->Label;
	target.Result = results;

	parameter->SetActionResult(target);
}


