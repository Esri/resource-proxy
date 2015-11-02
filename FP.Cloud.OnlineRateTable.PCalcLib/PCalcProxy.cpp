#include "CalculationResultProcessor.hpp"
#include "CalculationResultProcessorProxy.hpp"
#include "Lock.hpp"
#include "Exceptions.hpp"
#include "PCalcProxy.hpp"
#include "ActionResultProcessor.hpp"
#include "EnvironmentProcessor.hpp"
#include "ProductDescriptionMapper.hpp"
#include "PCalcFactory.hpp"
#include "PCalcManager.hpp"

using namespace System;

FP::Cloud::OnlineRateTable::PCalcLib::PCalcProxy::PCalcProxy(IPCalcManager^ manager, IActionResultProcessor^ actionResultProcessor, IEnvironmentProcessor^ environmentProcessor, ICalculationResultProcessor^ calculationResultProcessor, IProductDescriptionMapper^ mapper)
	: m_Manager(manager)
	, m_CalculationResultProcessor(calculationResultProcessor)
	, m_ActionResultProcessor(actionResultProcessor)
	, m_EnvironmentProcessor(environmentProcessor)
	, m_ProductDescriptionMapper(mapper)
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

	m_EnvironmentProcessor = nullptr;
	m_CalculationResultProcessor = nullptr;
	m_Manager = nullptr;
	m_ActionResultProcessor = nullptr;
	m_ProductDescriptionMapper = nullptr;
}

Shared::PCalcResultInfo^ FP::Cloud::OnlineRateTable::PCalcLib::PCalcProxy::Start(Shared::EnvironmentInfo^ environment, Shared::WeightInfo^ weight)
{
	//Lock lock(m_SyncLock);
	int nextAction = 0;

	// set state
	this->m_EnvironmentProcessor->Handle(environment);

	// calculate first step
	this->m_Manager->CalculateStart(nextAction);

	// get the result from first calculation step.	
	Shared::PCalcResultInfo^ result = this->m_CalculationResultProcessor->Handle(nextAction);

	// at start, the lib will discard the product desc parameter and we lost all changes.
	// this behavior makes it necessary to recalculate the product with changed weight.	
	this->m_ProductDescriptionMapper->SetWeight(weight);
	this->Calculate(nullptr, (Shared::ProductDescriptionInfo^)nullptr);

	// set product description
	result->ProductDescription = this->m_ProductDescriptionMapper->GetProduct();

	return result;
}

Shared::PCalcResultInfo^ FP::Cloud::OnlineRateTable::PCalcLib::PCalcProxy::Calculate(Shared::EnvironmentInfo^ environment, Shared::ProductDescriptionInfo^ product, Shared::ActionResultInfo^ actionResult)
{
	//Lock lock(m_SyncLock);
	INT32 nextAction = 0;

	this->m_EnvironmentProcessor->Handle(environment);

	//TODO: must be run without start calculation
	this->m_Manager->CalculateStart(nextAction);

	this->m_ProductDescriptionMapper->SetProduct(product);
	this->m_ActionResultProcessor->Handle(actionResult);

	// calculate
	this->m_Manager->CalculateNext(nextAction);

	//get result;
	Shared::PCalcResultInfo^ result = this->m_CalculationResultProcessor->Handle(nextAction);
	result->ProductDescription = this->m_ProductDescriptionMapper->GetProduct();

	return result;
}

Shared::PCalcResultInfo^ FP::Cloud::OnlineRateTable::PCalcLib::PCalcProxy::Calculate(Shared::EnvironmentInfo^ environment, Shared::ProductDescriptionInfo^ product)
{
	//Lock lock(m_SyncLock);
	INT32 nextAction = 0;

	// set state
	this->m_EnvironmentProcessor->Handle(environment);
	this->m_ProductDescriptionMapper->SetProduct(product);

	// calculate
	this->m_Manager->Calculate(nextAction);

	// get result
	Shared::PCalcResultInfo^ result = this->m_CalculationResultProcessor->Handle(nextAction);
	result->ProductDescription = this->m_ProductDescriptionMapper->GetProduct();

	return result;
}

Shared::PCalcResultInfo^ FP::Cloud::OnlineRateTable::PCalcLib::PCalcProxy::Back(Shared::EnvironmentInfo^ environment, Shared::ProductDescriptionInfo^ product)
{
	//Lock lock(m_SyncLock);
	INT32 nextAction = 0;

	// set state
	this->m_EnvironmentProcessor->Handle(environment);
	this->m_ProductDescriptionMapper->SetProduct(product);
	
	// calculate
	this->m_Manager->CalculateBack(nextAction);

	// get result
	Shared::PCalcResultInfo^ result = this->m_CalculationResultProcessor->Handle(nextAction);
	result->ProductDescription = this->m_ProductDescriptionMapper->GetProduct();

	return result;
}

void FP::Cloud::OnlineRateTable::PCalcLib::PCalcProxy::Init(String^ amxPath, String^ tablePath)
{
	m_Manager->Create();
	m_Manager->LoadPawn(amxPath);
	m_Manager->LoadProductTable(tablePath);
}


