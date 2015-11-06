#include "PCalcManager.hpp"
#include "PCalcProxyContext.hpp"
#include "Exceptions.hpp"
#include "PCalcFactory.hpp"
#include "ActionResultProcessor.hpp"
#include "EnvironmentProcessor.hpp"
#include "CalculationResultProcessorProxy.hpp"
#include "ProductDescriptionMapper.hpp"
#include "PCalcProxyPool.hpp"

Lib::PCalcProxyContext::PCalcProxyContext(Shared::EnvironmentInfo^ environment, System::String^ amxPath, System::String^ tablePath, ... array<System::String^>^ additionalFiles)
	: m_Environment(environment)
	, m_Proxy(nullptr)
	, m_AmxPath(amxPath)
	, m_TablePath(tablePath)
	, m_IsInitialized(true)
	, m_Lock(gcnew Lock(m_SyncLock))
{
	m_Proxy = PCalcProxyPool::Instance->Pop(environment);
	if (nullptr == m_Proxy)
	{
		m_Proxy = gcnew PCalcProxy();
		m_IsInitialized = false;
	}
}

Lib::PCalcProxyContext::PCalcProxyContext(Shared::EnvironmentInfo^ environment, IPCalcManager^ manager, IEnvironmentProcessor^ envProcessor, IActionResultProcessor^ actionProcessor, ICalculationResultProcessor^ calcProcessor, IProductDescriptionMapper^ mapper, System::String^ amxPath, System::String^ tablePath, ... array<System::String^>^ additionalFiles)
	: m_Environment(environment)
	, m_Proxy(nullptr)
	, m_AmxPath(amxPath)
	, m_TablePath(tablePath)
	, m_IsInitialized(true)
	, m_Lock(gcnew Lock(m_SyncLock))
{
	m_Proxy = PCalcProxyPool::Instance->Pop(environment);
	if (nullptr == m_Proxy)
	{
		m_Proxy = gcnew PCalcProxy(manager, actionProcessor, envProcessor, calcProcessor, mapper);
		m_IsInitialized = false;
	}
}

void FP::Cloud::OnlineRateTable::PCalcLib::PCalcProxyContext::Init()
{
	if (this->m_IsInitialized)
		return;

	this->m_Proxy->Init(m_AmxPath, m_TablePath);
	this->m_IsInitialized = true;
}

FP::Cloud::OnlineRateTable::PCalcLib::PCalcProxyContext::~PCalcProxyContext(void)
{
	if (nullptr != m_Proxy)
	{
		PCalcProxyPool::Instance->Push(m_Environment, m_Proxy);
	}

	this->m_AmxPath = nullptr;
	this->m_TablePath = nullptr;

	this->!PCalcProxyContext();
}

FP::Cloud::OnlineRateTable::PCalcLib::PCalcProxyContext::!PCalcProxyContext(void)
{
	if (nullptr != this->m_Lock)
	{
		delete m_Lock;
		this->m_Lock = nullptr;
	}
}


