#include "CalculationResultProcessor.hpp"
#include "CalculationResultProcessorProxy.hpp"
#include "Lock.hpp"
#include "Exceptions.hpp"
#include "PCalcProxy.hpp"

USING_PRODUCTCALCULATION_NAMESPACE
USING_PCALC_LIB_NAMESPACE

using namespace PCalcManagedLib;

FP::Cloud::OnlineRateTable::PCalcLib::PCalcProxy::PCalcProxy()
	: m_Factory(gcnew PCalcFactory())
	, m_Manager(gcnew PCalcManager(m_Factory))
	, m_CalculationResultProcessor(gcnew CalculationResultProcessorProxy(m_Factory))
	, m_ActionResultProcessor(gcnew ActionResultProcessor(m_Factory))
	, m_EnvironmentProcessor(gcnew EnvironmentProcessor(m_Factory))
	, m_ProductDescriptionMapper(gcnew ProductDescriptionMapper(m_Factory))
{
}

FP::Cloud::OnlineRateTable::PCalcLib::PCalcProxy::~PCalcProxy()
{
	this->!PCalcProxy();
}

FP::Cloud::OnlineRateTable::PCalcLib::PCalcProxy::!PCalcProxy()
{
	if (nullptr != m_EnvironmentProcessor)
		delete m_EnvironmentProcessor;

	if (nullptr != m_CalculationResultProcessor)
		delete m_CalculationResultProcessor;

	if(nullptr != m_Manager)
		delete m_Manager;

	if (nullptr != m_ActionResultProcessor)
		delete m_ActionResultProcessor;

	if (nullptr != m_Factory)
		delete m_Factory;
}

PCalcResultInfo^ FP::Cloud::OnlineRateTable::PCalcLib::PCalcProxy::Calculate(EnvironmentInfo^ environment, WeightInfo^ weight)
{
	Lock lock(this);
	int nextAction = 0;

	this->m_EnvironmentProcessor->Handle(environment);
	this->m_Manager->CalculateStart(nextAction);

	PCalcResultInfo^ result = this->m_CalculationResultProcessor->Handle(nextAction, nullptr);

	// set weight after first step
	this->m_ActionResultProcessor->Handle(weight);
	this->m_Manager->CalculateWeightChanged();

	// now we can map the product description
	result->ProductDescription = m_ProductDescriptionMapper->Map();
	return result;
}

PCalcResultInfo^ FP::Cloud::OnlineRateTable::PCalcLib::PCalcProxy::Calculate(EnvironmentInfo^ environment, ProductDescriptionInfo^ product, ActionResultInfo^ actionResult)
{
	Lock lock(this);
	INT32 nextAction = 0;

	this->Calculate(environment, product->Weight);

	this->m_EnvironmentProcessor->Handle(environment);
	this->m_ActionResultProcessor->Handle(product, actionResult);
	this->m_Manager->CalculateNext(nextAction);

	PCalcResultInfo^ result = this->m_CalculationResultProcessor->Handle(nextAction, nullptr);

	result->ProductDescription = this->m_ProductDescriptionMapper->Map();
	return result;
}

void FP::Cloud::OnlineRateTable::PCalcLib::PCalcProxy::Init(String^ amxPath, String^ tablePath)
{
	m_Manager->Create();
	m_Manager->LoadPawn(amxPath);
	m_Manager->LoadProductTable(tablePath);
}

