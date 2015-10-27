#include "ActionResultProcessor.hpp"
#include "ProductCalculation/IProductDescParameter.hpp"
#include "Exceptions.hpp"

USING_PRODUCTCALCULATION_NAMESPACE

FP::Cloud::OnlineRateTable::PCalcLib::ActionResultProcessor::ActionResultProcessor(PCalcFactory^ factory)
	: m_Factory(factory)
{

}

void FP::Cloud::OnlineRateTable::PCalcLib::ActionResultProcessor::Handle(ActionResultInfo^ actionResult)
{
	IProductDescParameterPtr parameter = m_Factory->GetProdDesc()->AccessCurrProduct();
	this->SetActionResult(parameter, actionResult);
}

void FP::Cloud::OnlineRateTable::PCalcLib::ActionResultProcessor::SetActionResult(IProductDescParameterPtr &parameter, ActionResultInfo^ actionResult)
{
	ProductCalculation::ActionResultType target;
	ProductCalculation::ResultValueVectorType results;

	for each(AnyInfo^ current in actionResult->Results)
	{
		switch (current->AnyType)
		{
			case EAnyType::INT32:
				results.push_back(System::Convert::ToInt32(current->AnyValue));
				break;
			case EAnyType::UINT32:
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


