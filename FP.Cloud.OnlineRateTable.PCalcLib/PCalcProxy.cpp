#include "NextActionProcessor.hpp"
#include "NextActionProcessorProxy.hpp"
#include "Lock.hpp"
#include "Exceptions.hpp"
#include "PCalcProxy.hpp"

USING_PRODUCTCALCULATION_NAMESPACE
USING_PCALC_LIB_NAMESPACE

using namespace PCalcManagedLib;

FP::Cloud::OnlineRateTable::PCalcLib::PCalcProxy::PCalcProxy()
	: m_Factory(gcnew PCalcFactory())
	, m_Manager(gcnew PCalcManager(m_Factory))
	, m_NextActionProcessor(gcnew NextActionProcessorProxy(m_Factory))
	, m_ActionResultProcessor(gcnew ActionResultProcessor(m_Factory))
{
}

FP::Cloud::OnlineRateTable::PCalcLib::PCalcProxy::~PCalcProxy()
{
	this->!PCalcProxy();
}

FP::Cloud::OnlineRateTable::PCalcLib::PCalcProxy::!PCalcProxy()
{
	if (nullptr != m_NextActionProcessor)
		delete m_NextActionProcessor;

	if(nullptr != m_Manager)
		delete m_Manager;

	if(nullptr != m_Factory)
		delete m_Factory;

	if (nullptr != m_ActionResultProcessor)
		delete m_ActionResultProcessor;
}

PCalcResultInfo^ FP::Cloud::OnlineRateTable::PCalcLib::PCalcProxy::Calculate(EnvironmentInfo^ environment, WeightInfo^ weight)
{
	Lock lock(this);
	int nextAction = 0;

	this->SetEnvironment(environment);
	this->m_ActionResultProcessor->Handle(weight);
	this->m_Manager->CalculateStart(nextAction);

	return this->m_NextActionProcessor->Handle(nextAction);
}

PCalcResultInfo^ FP::Cloud::OnlineRateTable::PCalcLib::PCalcProxy::Calculate(EnvironmentInfo^ environment, ProductDescriptionInfo^ product, ActionResultInfo^ actionResult)
{
	Lock lock(this);
	INT32 nextAction = 0;

	this->SetEnvironment(environment);
	this->m_ActionResultProcessor->Handle(product, actionResult);
	this->m_Manager->CalculateNext(nextAction);

	return this->m_NextActionProcessor->Handle(nextAction);
}

void FP::Cloud::OnlineRateTable::PCalcLib::PCalcProxy::SetEnvironment(EnvironmentInfo^ environment)
{
	IPCalcConfigurationPtr config = this->m_Factory->GetConfig();
	IPropertiesPtr properties = config->AccessCurrentProperties();

	properties->SetValue(EnvironmentProperty::REQUEST_ORIGIN_ZIP_CODE, PCalcManagedLib::ConvertToString(environment->SenderZipCode));
	properties->SetValue(EnvironmentProperty::REQUEST_LANG, PCalcManagedLib::ConvertToString(environment->Culture));
	config->ChangeProperties(properties);

	this->m_Manager->Initialize();
}

void FP::Cloud::OnlineRateTable::PCalcLib::PCalcProxy::Init(String^ amxPath, String^ tablePath)
{
	m_Manager->Create();
	m_Manager->LoadPawn(amxPath);
	m_Manager->LoadProductTable(tablePath);
}

