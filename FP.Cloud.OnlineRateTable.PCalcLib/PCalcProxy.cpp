#include "NextActionProcessor.hpp"
#include "NextActionProcessorFactory.hpp"
#include "Lock.hpp"
#include "PCalcLibException.hpp"
#include "PCalcProxy.hpp"

USING_PRODUCTCALCULATION_NAMESPACE
USING_PCALC_LIB_NAMESPACE

using namespace PCalcManagedLib;

FP::Cloud::OnlineRateTable::PCalcLib::PCalcProxy::PCalcProxy()
	: m_Factory(gcnew PCalcFactory())
	, m_Manager(gcnew PCalcManager(m_Factory))
	, m_Processor(gcnew NextActionProcessorFactory(m_Factory))
{
}

FP::Cloud::OnlineRateTable::PCalcLib::PCalcProxy::~PCalcProxy()
{
	this->!PCalcProxy();
}

FP::Cloud::OnlineRateTable::PCalcLib::PCalcProxy::!PCalcProxy()
{
	delete m_Manager;
	delete m_Factory;
}

PCalcResultInfo^ FP::Cloud::OnlineRateTable::PCalcLib::PCalcProxy::Calculate(EnvironmentInfo^ environment, WeightInfo^ weight)
{
	Lock lock(this);
	int nextAction = 0;

	this->SetEnvironment(environment);
	this->SetWeight(weight);
	this->Manager->CalculateStart(nextAction);

	return this->Processor->Process(nextAction);
}

PCalcResultInfo^ FP::Cloud::OnlineRateTable::PCalcLib::PCalcProxy::Calculate(EnvironmentInfo^ environment, ProductDescriptionInfo^ product, ActionResultInfo^ actionResult)
{
	Lock lock(this);
	INT32 nextAction = 0;

	this->Calculate(environment, product->Weight);
	this->SetProductDescription(product, actionResult);
	this->Manager->CalculateNext(nextAction);

	return this->Processor->Process(nextAction);
}

void FP::Cloud::OnlineRateTable::PCalcLib::PCalcProxy::SetEnvironment(EnvironmentInfo^ environment)
{
	IPCalcConfigurationPtr config = this->Factory->GetConfig();
	IPropertiesPtr properties = config->AccessCurrentProperties();

	properties->SetValue(EnvironmentProperty::REQUEST_ORIGIN_ZIP_CODE, PCalcManagedLib::ConvertToString(environment->SenderZipCode));
	properties->SetValue(EnvironmentProperty::REQUEST_LANG, PCalcManagedLib::ConvertToString(environment->Culture));
	config->ChangeProperties(properties);

	this->Manager->Initialize();
}

void FP::Cloud::OnlineRateTable::PCalcLib::PCalcProxy::SetWeight(WeightInfo^ weight)
{
	ProductCalculation::IProductDescParameterPtr product = this->Factory->GetProdDesc()->AccessCurrProduct();
	ProductCalculation::WeightType newWeight(weight->WeightValue, (BYTE)weight->WeightUnit);
	product->SetWeight(newWeight);
}

void FP::Cloud::OnlineRateTable::PCalcLib::PCalcProxy::SetActionResult(ProductCalculation::ActionResultType &target, ActionResultInfo^ actionResult)
{
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
}

void FP::Cloud::OnlineRateTable::PCalcLib::PCalcProxy::SetProductDescription(ProductDescriptionInfo^ product, ActionResultInfo^ actionResult)
{
	ProductCalculation::ActionResultType target;
	SetActionResult(target, actionResult);

	ProductCalculation::IProductDescParameterPtr parameter = this->Factory->GetProdDesc()->AccessCurrProduct();
	parameter->SetProductCode(product->ProductCode);
	parameter->SetProductID(product->ProductId);
	parameter->SetActionResult(target);
}

