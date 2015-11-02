#include "PCalcManager.hpp"
#include "PCalcProxyContext.hpp"
#include "Exceptions.hpp"
#include "PCalcFactory.hpp"
#include "ActionResultProcessor.hpp"
#include "EnvironmentProcessor.hpp"
#include "CalculationResultProcessorProxy.hpp"
#include "ProductDescriptionMapper.hpp"

BEGIN_PCALC_LIB_NAMESPACE

PCalcProxyContext::PCalcProxyContext(System::String^ amxPath, System::String^ tablePath, ... array<System::String^>^ additionalFiles)
	: m_Factory(gcnew PCalcFactory())
	, m_Proxy(gcnew PCalcProxy(gcnew PCalcManager(m_Factory), gcnew ActionResultProcessor(m_Factory), gcnew EnvironmentProcessor(m_Factory), gcnew CalculationResultProcessorProxy(m_Factory), gcnew ProductDescriptionMapper(m_Factory)))
	, m_AmxPath(amxPath)
	, m_TablePath(tablePath)
	, m_IsInitialized(false)
	, m_Lock(nullptr)
{
}

PCalcProxyContext::PCalcProxyContext(IPCalcManager^ manager, IEnvironmentProcessor^ envProcessor, IActionResultProcessor^ actionProcessor, ICalculationResultProcessor^ calcProcessor, IProductDescriptionMapper^ mapper, System::String^ amxPath, System::String^ tablePath, ... array<System::String^>^ additionalFiles)
	: m_Proxy(gcnew PCalcProxy(manager, actionProcessor, envProcessor, calcProcessor, mapper))
	, m_Factory(nullptr)
	, m_AmxPath(amxPath)
	, m_TablePath(tablePath)
	, m_IsInitialized(false)
	, m_Lock(nullptr)
{

}

void FP::Cloud::OnlineRateTable::PCalcLib::PCalcProxyContext::Init()
{
	if (m_IsInitialized)
		return;

	m_Lock = gcnew Lock(m_SyncLock);
	PCalcProxy^ proxy = dynamic_cast<PCalcProxy^>(m_Proxy);

	if (nullptr != proxy)
	{
		try
		{
			proxy->Init(m_AmxPath, m_TablePath);
		}
		catch (System::Runtime::InteropServices::SEHException^ ex)
		{
			throw gcnew PCalcLibException(ex->Message, ex);
		}
	}

	m_IsInitialized = true;
}

FP::Cloud::OnlineRateTable::PCalcLib::PCalcProxyContext::~PCalcProxyContext(void)
{
	if (nullptr != m_Proxy)
	{
		delete m_Proxy;
		m_Proxy = nullptr;
	}

	m_AmxPath = nullptr;
	m_TablePath = nullptr;

	this->!PCalcProxyContext();
}

FP::Cloud::OnlineRateTable::PCalcLib::PCalcProxyContext::!PCalcProxyContext(void)
{
	if (nullptr != m_Factory)
	{
		delete m_Factory;
		m_Factory = nullptr;
	}

	if (nullptr != m_Lock)
	{
		delete m_Lock;
		m_Lock = nullptr;
	}
}

END_PCALC_LIB_NAMESPACE

