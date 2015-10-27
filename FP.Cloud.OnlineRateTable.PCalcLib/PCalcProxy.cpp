#include "CalculationResultProcessor.hpp"
#include "CalculationResultProcessorProxy.hpp"
#include "Lock.hpp"
#include "Exceptions.hpp"
#include "PCalcProxy.hpp"

USING_PRODUCTCALCULATION_NAMESPACE
USING_PCALC_LIB_NAMESPACE

using namespace PCalcManagedLib;

//System::Object^ FP::Cloud::OnlineRateTable::PCalcLib::PCalcProxy::m_SyncLock = gcnew System::Object();

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

	if (nullptr != m_Manager)
		delete m_Manager;

	if (nullptr != m_ActionResultProcessor)
		delete m_ActionResultProcessor;

	if (nullptr != m_ProductDescriptionMapper)
		delete m_ProductDescriptionMapper;

	if (nullptr != m_Factory)
		delete m_Factory;
}

PCalcResultInfo^ FP::Cloud::OnlineRateTable::PCalcLib::PCalcProxy::Calculate(EnvironmentInfo^ environment, WeightInfo^ weight)
{
	Lock lock(m_SyncLock);
	int nextAction = 0;

	this->m_EnvironmentProcessor->Handle(environment);
	this->m_Manager->CalculateStart(nextAction);

	// update weight after valid product is available
	PCalcResultInfo^ result = this->m_CalculationResultProcessor->Handle(nextAction);

	// recalculate with changed weight
	this->m_ProductDescriptionMapper->SetWeight(weight);
	this->Calculate(nullptr, (ProductDescriptionInfo^)nullptr);

	// set product description
	result->ProductDescription = this->m_ProductDescriptionMapper->GetProduct();

	return result;
}

PCalcResultInfo^ FP::Cloud::OnlineRateTable::PCalcLib::PCalcProxy::Calculate(EnvironmentInfo^ environment, ProductDescriptionInfo^ product, ActionResultInfo^ actionResult)
{
	Lock lock(m_SyncLock);
	INT32 nextAction = 0;

	this->m_EnvironmentProcessor->Handle(environment);

	//TODO: must be run without start calculation
	this->Calculate(nullptr, product->Weight);

	this->m_ProductDescriptionMapper->SetProduct(product);
	this->m_ActionResultProcessor->Handle(actionResult);

	// calculate
	this->m_Manager->CalculateNext(nextAction);

	//get result;
	PCalcResultInfo^ result = this->m_CalculationResultProcessor->Handle(nextAction);

	// set product description
	result->ProductDescription = this->m_ProductDescriptionMapper->GetProduct();

	return result;
}

PCalcResultInfo^ FP::Cloud::OnlineRateTable::PCalcLib::PCalcProxy::Calculate(EnvironmentInfo^ environment, ProductDescriptionInfo^ product)
{
	Lock lock(m_SyncLock);
	INT32 nextAction = 0;

	this->m_EnvironmentProcessor->Handle(environment);
	this->m_ProductDescriptionMapper->SetProduct(product);

	// calculate
	this->m_Manager->Calculate(nextAction);

	// get result
	PCalcResultInfo^ result = this->m_CalculationResultProcessor->Handle(nextAction);

	//set product description
	result->ProductDescription = this->m_ProductDescriptionMapper->GetProduct();

	return result;

}

void FP::Cloud::OnlineRateTable::PCalcLib::PCalcProxy::Init(String^ amxPath, String^ tablePath)
{
	m_Manager->Create();
	m_Manager->LoadPawn(amxPath);
	m_Manager->LoadProductTable(tablePath);
}


