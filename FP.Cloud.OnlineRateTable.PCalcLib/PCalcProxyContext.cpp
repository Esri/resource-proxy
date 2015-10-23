#include "PCalcManager.hpp"
#include "PCalcProxyContext.hpp"
#include "Exceptions.hpp"

BEGIN_PCALC_LIB_NAMESPACE

PCalcProxyContext::PCalcProxyContext(System::String^ amxPath, System::String^ tablePath, ... array<System::String^>^ additionalFiles)
	: m_Proxy(nullptr)
	, m_Domain(nullptr)
	, m_AmxPath(amxPath)
	, m_TablePath(tablePath)
{

}

void FP::Cloud::OnlineRateTable::PCalcLib::PCalcProxyContext::Init()
{
	System::AppDomainSetup^ setup = gcnew System::AppDomainSetup();
	setup->ApplicationBase = System::AppDomain::CurrentDomain->SetupInformation->ApplicationBase;
	setup->PrivateBinPath = System::AppDomain::CurrentDomain->SetupInformation->PrivateBinPath;

	m_Domain = System::AppDomain::CreateDomain(System::Guid::NewGuid().ToString(), nullptr, setup);
	m_Proxy = (PCalcProxy^)m_Domain->CreateInstanceAndUnwrap(System::Reflection::Assembly::GetExecutingAssembly()->FullName, PCalcProxy::typeid->FullName);

	try
	{
		m_Proxy->Init(m_AmxPath, m_TablePath);
	}
	catch (System::Runtime::InteropServices::SEHException^ ex)
	{
		throw gcnew PCalcLibException(ex->Message, ex);
	}
}

FP::Cloud::OnlineRateTable::PCalcLib::PCalcProxyContext::~PCalcProxyContext(void)
{
	this->!PCalcProxyContext();
}

FP::Cloud::OnlineRateTable::PCalcLib::PCalcProxyContext::!PCalcProxyContext(void)
{
	if (nullptr != m_Proxy)
	{
		delete m_Proxy;

	}

	if (nullptr != m_Domain)
	{
		System::AppDomain::Unload(m_Domain);
	}

	m_Proxy = nullptr;
	m_Domain = nullptr;
	m_AmxPath = nullptr;
	m_TablePath = nullptr;
}

END_PCALC_LIB_NAMESPACE

