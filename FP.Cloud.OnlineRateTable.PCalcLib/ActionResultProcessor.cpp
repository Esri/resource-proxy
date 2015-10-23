#include "ActionResultProcessor.hpp"
#include "ProductCalculation/IProductDescParameter.hpp"

USING_PRODUCTCALCULATION_NAMESPACE

FP::Cloud::OnlineRateTable::PCalcLib::ActionResultProcessor::ActionResultProcessor(PCalcFactory^ factory)
	: m_Factory(factory)
{

}

void FP::Cloud::OnlineRateTable::PCalcLib::ActionResultProcessor::Handle(ProductDescriptionInfo^ product, ActionResultInfo^ actionResult)
{
	IProductDescParameterPtr parameter = this->SetProduct(product);
	this->SetActionResult(parameter, actionResult);
}

void FP::Cloud::OnlineRateTable::PCalcLib::ActionResultProcessor::Handle(WeightInfo^ weight)
{
	IProductDescParameterPtr parameter = m_Factory->GetProdDesc()->AccessCurrProduct();
	this->SetWeight(parameter, weight);
}

ProductCalculation::IProductDescParameterPtr FP::Cloud::OnlineRateTable::PCalcLib::ActionResultProcessor::SetProduct(ProductDescriptionInfo^ product)
{

	IProductDescParameterPtr parameter = m_Factory->GetProdDesc()->AccessCurrProduct();

	parameter->SetProductCode(product->ProductCode);
	parameter->SetProductID(product->ProductId);

	this->SetWeight(parameter, product->Weight);

	return parameter;
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
			default:
				break;
		}
	}

	target.ID = (int)actionResult->Action;
	target.Label = actionResult->Label;
	target.Result = results;

	parameter->SetActionResult(target);
}

void FP::Cloud::OnlineRateTable::PCalcLib::ActionResultProcessor::SetWeight(IProductDescParameterPtr &parameter, WeightInfo^ changedWeight)
{
	//TODO wwaitz - in my mind, weight should be normalized
	WeightType weight(changedWeight->WeightValue, (BYTE)changedWeight->WeightUnit);
	parameter->SetWeight(weight);
}

